using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRun : MonoBehaviour
{

	// Management of sprites
	private Object[] backgrounds;
	private Object[] props;
	private Object[] chars;

	// Game management
	private GameObject enemyCards;
	private int [] enemyChars;	
	private Agent agent;

	private int NUM_ENEMY_CARDS = 3;
	private int NUM_CLASSES     = 3;
	private int DECK_SIZE       = 4;

	// Rewards
	private float RWD_ACTION_INVALID = -2.0f;
	private float RWD_HAND_LOST      = -1.0f;
	private float RWD_TIE            = -0.1f;
	private float RWD_HAND_WON       =  1.0f;

	// Other UI elements
	private UnityEngine.UI.Text textDeck;

    [Header("ANIMATORS")]
    public Animator targetesAnimator;
    public Animator pantallaAnimator;

    [Header("CARDS")]
    public SpriteRenderer card1Player;
    public SpriteRenderer card2Player;
    public SpriteRenderer card3Player;
    public Sprite cardFox;
    public Sprite cardOpossum;
    public Sprite cardFrog;
    public Sprite cardBase;

    [Header("UI")]
    public Text texttargetaPlayer1;
    public Text texttargetaPlayer2;
    public Text textcountPlayer1;
    public Text textcountPlayer2;
    public Slider trainSlider;
    [Range(1, 50)]
    public int countRounds = 10;


    private int countPlayer1;
    private int countPlayer2;

    // Start is called before the first frame update
    void Start()
    {


        ///////////////////////////////////////
        // Sprite management
        ///////////////////////////////////////

        // Load all prefabs
        backgrounds = Resources.LoadAll("Backgrounds/");
        props       = Resources.LoadAll("Props/");
        chars       = Resources.LoadAll("Chars/");


        ///////////////////////////////////////
        // UI management
        ///////////////////////////////////////
        textDeck = GameObject.Find("TextDeck").GetComponent<UnityEngine.UI.Text>();


        ///////////////////////////////////////
        // Game management
        ///////////////////////////////////////
        enemyCards = GameObject.Find("EnemyCards");
        enemyChars = new int[NUM_ENEMY_CARDS];

        agent      = GameObject.Find("AgentManager").GetComponent<Agent>();

        agent.Initialize();


        ///////////////////////////////////////
        // Start the game
        ///////////////////////////////////////
        StartCoroutine("GenerateTurn");
        
    }

    // Generate a card on a given transform
    // Return the label (0-2) of the card
    private int GenerateCard(Transform parent)
    {

    	int idx = Random.Range(0, backgrounds.Length);
    	Instantiate(backgrounds[idx], parent.position, Quaternion.identity, parent);


    	idx               = Random.Range(0, props.Length);
    	Vector3 position = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), -1.0f);
   	  	Instantiate(props[idx], parent.position+position, Quaternion.identity, parent);

    	idx         = Random.Range(0, chars.Length);
    	position    = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), -2.0f);    	
   	  	Instantiate(chars[idx], parent.position+position, Quaternion.identity, parent);

   	  	// Determine label of the character, return it
   	  	int label = 0;
   	  	if(chars[idx].name.StartsWith("frog")) label = 1;
   	  	else if(chars[idx].name.StartsWith("opossum")) label = 2;

    	return label;
    } 

    // Generate another turn
    IEnumerator GenerateTurn()
    {
        /////////// INIT THE VARIABLES AND DO TRAINING /////////////
        targetesAnimator.SetBool("PantallaIn", false);
        pantallaAnimator.SetBool("PantallaIn", false);
        yield return new WaitForSeconds(1.5f);
        countPlayer1 = 0;
        countPlayer2 = 0;
        trainSlider.value = 0;
        texttargetaPlayer1.text = countPlayer1.ToString();
        texttargetaPlayer2.text = countPlayer2.ToString();
        textcountPlayer1.text = countPlayer1.ToString();
        textcountPlayer2.text = countPlayer2.ToString();
        pantallaAnimator.SetBool("PantallaTrainIn", true);
        yield return new WaitForSeconds(2f);
        for (float turn=0; turn < 100; turn++) {
            
	        ///////////////////////////////////////
	        // Generate enemy cards
	        ///////////////////////////////////////

	    	// Destroy previous sprites (if any) and generate new cards
	    	int c = 0;
	    	foreach(Transform card in enemyCards.transform) {
	    		foreach(Transform sprite in card) {
	    			Destroy(sprite.gameObject);
	    		}
	    		enemyChars[c++] = GenerateCard(card);
	    	}


	        ///////////////////////////////////////
	        // Generate player deck
	        ///////////////////////////////////////
	        int [] deck   = GeneratePlayerDeck();
	        textDeck.text = "Deck: ";
	        foreach(int card in deck)
	        	textDeck.text += card.ToString() + "/";

            yield return new WaitForEndOfFrame();

            int [] action = agent.Play(deck, enemyChars);

	        //textDeck.text += " Action:";
	        //foreach(int a in action)
	        //	textDeck.text += a.ToString() + "/";

            if (action[0] == 0) card1Player.sprite = cardFox;
            if (action[0] == 1) card1Player.sprite = cardFrog;
            if (action[0] == 2) card1Player.sprite = cardOpossum;
            if (action[1] == 0) card2Player.sprite = cardFox;
            if (action[1] == 1) card2Player.sprite = cardFrog;
            if (action[1] == 2) card2Player.sprite = cardOpossum;
            if (action[2] == 0) card3Player.sprite = cardFox;
            if (action[2] == 1) card3Player.sprite = cardFrog;
            if (action[2] == 2) card3Player.sprite = cardOpossum;

            ///////////////////////////////////////
            // Compute reward
            ///////////////////////////////////////
            float reward = ComputeReward(agent.myCards, action);
	        
	        Debug.Log("Turn/reward: " + turn.ToString() + "->" + reward.ToString());
            trainSlider.value = turn / 100;
            agent.GetReward(reward);

            yield return new WaitForSeconds(0.01f);
        }

        /////////// DO ANIMATIONS AND PLAY THE GAME /////////////
        targetesAnimator.SetBool("PantallaIn", false);
        pantallaAnimator.SetBool("PantallaTrainIn", false);
        yield return new WaitForSeconds(2f);
        pantallaAnimator.SetBool("PantallaIn", true);
        yield return new WaitForSeconds(2f);
        //Play the real game
        for (int turn = 0; turn < countRounds; turn++)
        {
            ///////////////////////////////////////
            // Generate enemy cards
            ///////////////////////////////////////

            // Destroy previous sprites (if any) and generate new cards
            int c = 0;
            foreach (Transform card in enemyCards.transform)
            {
                foreach (Transform sprite in card)
                {
                    Destroy(sprite.gameObject);
                }
                enemyChars[c++] = GenerateCard(card);
            }


            ///////////////////////////////////////
            // Generate player deck
            ///////////////////////////////////////
            int[] deck = GeneratePlayerDeck();
            textDeck.text = "Deck: ";
            foreach (int card in deck)
                textDeck.text += card.ToString() + "/";

            yield return new WaitForEndOfFrame();

            int[] action = agent.Play(deck, enemyChars);

            //textDeck.text += " Action:";
            //foreach (int a in action)
            //    textDeck.text += a.ToString() + "/";

            if (action[0] == 0) card1Player.sprite = cardFox;
            if (action[0] == 1) card1Player.sprite = cardFrog;
            if (action[0] == 2) card1Player.sprite = cardOpossum;
            if (action[1] == 0) card2Player.sprite = cardFox;
            if (action[1] == 1) card2Player.sprite = cardFrog;
            if (action[1] == 2) card2Player.sprite = cardOpossum;
            if (action[2] == 0) card3Player.sprite = cardFox;
            if (action[2] == 1) card3Player.sprite = cardFrog;
            if (action[2] == 2) card3Player.sprite = cardOpossum;

            ///////////////////////////////////////
            // Compute reward
            ///////////////////////////////////////
            float reward = ComputeReward(agent.myCards, action);

            Debug.Log("Turn/reward: " + turn.ToString() + "->" + reward.ToString());

            agent.GetReward(reward);

            if (reward == RWD_ACTION_INVALID || reward == RWD_HAND_LOST) countPlayer2++;
            if (reward == RWD_HAND_WON) countPlayer1++;

            texttargetaPlayer1.text = countPlayer1.ToString();
            texttargetaPlayer2.text = countPlayer2.ToString();
            textcountPlayer1.text = countPlayer1.ToString();
            textcountPlayer2.text = countPlayer2.ToString();

            yield return new WaitForSeconds(0.01f);
        }

        /////////// PRESENT FINAL RESULTS /////////////
        pantallaAnimator.SetBool("PantallaIn", false);
        yield return new WaitForSeconds(1.5f);
        targetesAnimator.SetBool("PantallaIn", true);
    }


    public void ButtonPlayGame()
    {
        StartCoroutine("GenerateTurn");
    }

    public void ButtonExitGame()
    {
        Application.Quit();
    }

    // Auxiliary methods
    private int [] GeneratePlayerDeck()
    {
    	int [] deck = new int [DECK_SIZE];

    	for(int i=0; i<DECK_SIZE; i++)
    	{
    		deck[i] = Random.Range(0, NUM_CLASSES);  // high limit is exclusive so [0, NUM_CLASSES-1]
    	}

    	return deck;
    }

    // Compute the result of the turn and return the associated reward 
    // given the cards selected by the agent (action)
   	// deck -> array with the number of cards of each class the player has
   	// action -> array with the class of each card played
    private float ComputeReward(int [] deck, int [] action)
    {
    	// First check if the action is valid given the player's deck
    	foreach(int card in action)
    	{
    		deck[card]--;
            if (deck[card] < 0)
    			return RWD_ACTION_INVALID;
    	}


    	// Second see who wins
    	int score = 0;
    	for(int i=0; i<NUM_ENEMY_CARDS; i++)
    	{
    		if(action[i] != enemyChars[i])
    		{
    			if(action[i] > enemyChars[i] || action[i]==0 && enemyChars[i]==2)	
    				score++;
    			else
    				score--;
    		}
    		
    	}

    	if(score == 0) return RWD_TIE;
    	else if(score > 0) return RWD_HAND_WON;
    	else return RWD_HAND_LOST;
    }
}
