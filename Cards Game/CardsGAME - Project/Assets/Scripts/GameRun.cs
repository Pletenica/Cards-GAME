using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRun : MonoBehaviour
{

	// Management of sprites
	private Object[] backgrounds;
	private Object[] props;
	private Object[] chars;

	// Game management
	private GameObject enemyCards;
	private GameObject agent;
	private string currentChar;	

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
        // Game management
        ///////////////////////////////////////
        enemyCards = GameObject.Find("EnemyCards");

        agent      = GameObject.Find("AgentManager");



        ///////////////////////////////////////
        // Start the game
        ///////////////////////////////////////
        StartCoroutine("GenerateTurn");


        ///////////////////////////////////////
        // Image generation
        ///////////////////////////////////////
    	//renderTexture = gameObject.GetComponent<Camera>().targetTexture;

    	//imgWidth  = renderTexture.width;
    	//imgHeight = renderTexture.height;

        
    }


    // Generate a card on a given transform
    // Return the char name
    private string GenerateCard(Transform parent)
    {

    	int idx = Random.Range(0, backgrounds.Length);
    	Instantiate(backgrounds[idx], parent.position, Quaternion.identity, parent);


    	idx               = Random.Range(0, props.Length);
    	Vector3 position = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), -1.0f);
   	  	Instantiate(props[idx], parent.position+position, Quaternion.identity, parent);

    	idx         = Random.Range(0, chars.Length);
    	position    = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), -2.0f);    	
   	  	Instantiate(chars[idx], parent.position+position, Quaternion.identity, parent);

    	return chars[idx].name;
    } 

    // Generate another turn
    IEnumerator GenerateTurn()
    {	
    	for(int turn=0; turn<1000; turn++) {

	        ///////////////////////////////////////
	        // Generate enemy cards
	        ///////////////////////////////////////

	    	// Destroy previous sprites (if any) and generate new cards
	    	foreach(Transform card in enemyCards.transform) {
	    		foreach(Transform sprite in card) {
	    			Destroy(sprite.gameObject);
	    		}

	    		currentChar = GenerateCard(card);
	    		Debug.Log(currentChar);
	    	}


	        ///////////////////////////////////////
	        // Generate player deck
	        ///////////////////////////////////////


	        ///////////////////////////////////////
	        // Tell the player to play
	        ///////////////////////////////////////
	        agent.SendMessage("Play");


	        ///////////////////////////////////////
	        // Show the player's cards and results
	        ///////////////////////////////////////


	        ///////////////////////////////////////
	        // Manage turns/games
	        ///////////////////////////////////////



	    	yield return new WaitForSeconds(2.5f);

    	}

    }

    // After the image is rendered, and if a new frame has been generated,
    // save it to disk
/*    void OnPostRender()
    {
    	string fileName   = "Output/frame-" + (fileId++) + "-" + currentChar + ".png";




		Texture2D capture = new Texture2D(imgWidth, imgHeight, TextureFormat.RGB24, false);
     	capture.ReadPixels( new Rect(0, 0, imgWidth, imgHeight), 0, 0);
     
     //RenderTexture.active = null; //can help avoid errors 
     //virtuCamera.camera.targetTexture = null;
     // consider ... Destroy(tempRT);
     
     	byte[] bytes;
     	bytes = capture.EncodeToPNG();
     
     	System.IO.File.WriteAllBytes(fileName, bytes );


     // virtualCam.SetActive(false); ... no great need for this.

    	 // string fileName = "holaquetal";
      
      //    File.WriteAllBytes(path, ImageConversion.EncodeToPNG(renderTexture));
      //    Debug.Log("Saved file to: " + path);    	
       
    }
*/
}
