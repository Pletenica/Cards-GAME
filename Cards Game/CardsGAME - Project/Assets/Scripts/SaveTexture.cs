using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveTexture : MonoBehaviour
{

	// Generation of the output images
	private int fileId = 0;
	private int imgWidth, imgHeight;
	private RenderTexture renderTexture;

	// Management of sprites
	private GameObject spriteContainer;
	private Object[] backgrounds;
	private Object[] props;
	private Object[] chars;

	private string currentChar;

    // Initialize sprite management and image generation
    void Start()
    {
        Debug.Log("hello");

        ///////////////////////////////////////
        // Sprite management
        ///////////////////////////////////////

        spriteContainer = GameObject.Find("SpriteContainer");

        // Load all prefabs
        backgrounds = Resources.LoadAll("Backgrounds/");
        props       = Resources.LoadAll("Props/");
        chars       = Resources.LoadAll("Chars/");


        ///////////////////////////////////////
        // Image generation
        ///////////////////////////////////////
    	renderTexture = gameObject.GetComponent<Camera>().targetTexture;

    	imgWidth  = renderTexture.width;
    	imgHeight = renderTexture.height;





//        var res = Resources.LoadAll<GameObject>("ai/");

//    	foreach (GameObject obj in res)
//    	{
//        	ai.Add(obj);
//    	}
    }

    // Use different z's for the rendering order
    private Vector3 SpawnPoint(float z=0.0f)
    {
    	return new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), z);	
    } 

    // Create a background, several props and one character sprite
    void Update()
    {	
    	// Destroy previous sprites (if any)
    	foreach(Transform child in spriteContainer.transform) {
    		Destroy(child.gameObject);
    	}


	
    	int idx = Random.Range(0, backgrounds.Length);
    	Instantiate(backgrounds[idx], Vector3.zero, Quaternion.identity, spriteContainer.transform);


    	idx = Random.Range(0, props.Length);
   	  	Instantiate(props[idx], SpawnPoint(-1), Quaternion.identity, spriteContainer.transform);

    	idx         = Random.Range(0, chars.Length);
    	currentChar = chars[idx].name;
   	  	Instantiate(chars[idx], SpawnPoint(-2), Quaternion.identity, spriteContainer.transform);

    }

    // After the image is rendered, and if a new frame has been generated,
    // save it to disk
    void OnPostRender()
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
}
