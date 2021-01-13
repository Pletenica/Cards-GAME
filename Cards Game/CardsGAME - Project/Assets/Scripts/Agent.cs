using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class Agent : MonoBehaviour
{
	// Neural network: asset and worker
	public NNModel CNN_Model_Asset;
	private Model model;
	private IWorker worker;

	// Render textures (enemy's cards)
	public RenderTexture [] renderTextures;

	private int imgWidth, imgHeight;

    // Start is called before the first frame update
    void Start()
    {

    	imgWidth  = renderTextures[0].width;
    	imgHeight = renderTextures[0].height;



    	// Create the neural network

    	model  = ModelLoader.Load(CNN_Model_Asset, false);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharp, model, false);
        Debug.Log("started"); 


       
    }

    private int argmax(float [] values)
    {
    	float maxVal = values[0];
    	int   maxIdx = 0;

    	for(int i=1; i<values.Length; i++)
    		if(values[i] > maxVal)
    		{
    			maxVal = values[i];
    			maxIdx = i;
    		}

    	return maxIdx;
    }


    // Gets the images of the enemy's cards, uses the NN to
    // guess the class of each one
    /*private int [] ReadCards()
    {

    	
    }*/


    // Play one turn
    public void Play() 
    {
    	Debug.Log("playing");


        // Barracuda expects NHWC (batch, height, width, channels) as image format
        // but it also accepts a RenderTexture directly, so
        // Important: the pixel values must be in [0, 1]


        Tensor input = new Tensor(renderTextures[0], 3);
        if (worker != null)
        {
            worker.Execute(input);
            var res = worker.PeekOutput();

            // Just checking the actual output
            Debug.Log(res);
            Debug.Log(res[0]);
            Debug.Log(res[1]);
            Debug.Log(res[2]);


            // This gives the class of the card
            float[] output = res.AsFloats();


            Debug.Log(argmax(output));

            input.Dispose();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
