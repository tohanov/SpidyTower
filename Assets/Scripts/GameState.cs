using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameState : MonoBehaviour
{
	// [SerializeField] AnimationCurve gameSpeedCurve;
	// float rawGameSpeed;
	[SerializeField] GameObject spidyPrefab;
	[SerializeField] GameObject obstacleSpawner;
	[SerializeField] GameObject buildingsGenerator;
	internal Stat gameSpeed;
	int stepsTravelled;

	// private int gamePaused = 0;
	[SerializeField] LayerMask regularCullingMask;
	[SerializeField] LayerMask gamePauseCullingMask;
	TextMeshProUGUI stepsHUDStat;
	TextMeshProUGUI websHUDStat;
	TextMeshProUGUI missedHUDStat;
	GameObject pauseOverlay;
	bool gamePaused;

	internal Vector2 boundsHigh;
	internal Vector2 boundsLow;

	[SerializeField] internal float movementSpeed;

	// internal Vector2 boundsHigh;
	// internal Vector2 boundsLow;
	void Awake() {

		// TODO : disable debugging if not in editor (release build)

		gameSpeed = new Stat(1, 1, 4, updateTimeScale, null, null);

		boundsHigh = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		boundsLow = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));

		pauseOverlay = GameObject.FindGameObjectWithTag("HUD/Pause Overlay");
		pauseOverlay.SetActive(false);
		gamePaused = false;

		// stepsHUDStat = GameObject.FindGameObjectWithTag("HUD/Stats/Steps");
		stepsHUDStat = GameObject.FindGameObjectWithTag("HUD/Stats/Steps").GetComponent<TextMeshProUGUI>();
		websHUDStat = GameObject.FindGameObjectWithTag("HUD/Stats/Web Cartridges").GetComponent<TextMeshProUGUI>();
		missedHUDStat = GameObject.FindGameObjectWithTag("HUD/Stats/Missed Civilians").GetComponent<TextMeshProUGUI>();

		setupScene();
	}

	private void updateTimeScale()
	{
		Time.timeScale = gameSpeed.current;
	}

	internal void incrementStepsTravelled() {
		++stepsTravelled;

		stepsHUDStat.text = stepsTravelled.ToString();
	}

	internal void updateAvailableWebCartridges(int current, int max) {
		websHUDStat.text = current + "/" + max;
	}
	
	internal void updateMissedCivilians(int current, int max) {
		missedHUDStat.text = current + "/" + max;
	}

	void Start() {
		Vector2 buildingBlockSize = GetComponent<GenerateBuildings>().blockSize;

		createBorderEdgeCollider(
			"Screen Border/Bottom",

			boundsLow.x - buildingBlockSize.x,
			boundsLow.y - buildingBlockSize.y, 

			boundsHigh.x + buildingBlockSize.x, 
			boundsLow.y - buildingBlockSize.y
		);

		createBorderEdgeCollider(
			"Screen Border/Right",

			boundsHigh.x + buildingBlockSize.x, 
			boundsLow.y - buildingBlockSize.y,
			
			boundsHigh.x + buildingBlockSize.x,
			boundsHigh.y
		);

		createBorderEdgeCollider(		
			"Screen Border/Left",

			boundsLow.x - buildingBlockSize.x,
			boundsLow.y - buildingBlockSize.y,  
		
			boundsLow.x - buildingBlockSize.x,
			boundsHigh.y
		);
	}

	private void createBorderEdgeCollider(string tag, float x1, float y1, float x2, float y2)
	{
		GameObject screenBorder = new GameObject(tag);
		screenBorder.transform.SetParent(gameObject.transform);
		screenBorder.gameObject.tag = tag;

		screenBorder.AddComponent<OutOfScreenObjectsDestroyer>();

		EdgeCollider2D triggerCollider = screenBorder.AddComponent<EdgeCollider2D>();
		triggerCollider.isTrigger = true;
		triggerCollider.SetPoints(new List<Vector2>{
			new Vector2(x1, y1),
			new Vector2(x2, y2),
		});

		// screenBorder.gameObject.AddComponent<Rigidbody2D>();

		Rigidbody2D rb = screenBorder.AddComponent<Rigidbody2D>();
		rb.isKinematic = true;
		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

		rb.gravityScale = 0f;
		rb.mass = 1.0f;
		rb.drag = 0.0f;
		rb.angularDrag = 0.05f;

		// 	// Add a Rigidbody2D component to the game object this script is attached to
		// Rigidbody2D rb = wrapper.AddComponent<Rigidbody2D>();

		// 	// Set the Rigidbody2D properties as desired
			
	}

	void setupScene() {
		Instantiate(spidyPrefab, transform);
		Instantiate(obstacleSpawner, transform);
		// Instantiate(buildingsGenerator); // TODO
	}

	internal void TogglePauseGame(InputAction.CallbackContext context)
	{
		pauseOverlay.SetActive(gamePaused = !gamePaused);

		// gamePaused = 1 - gamePaused; // FIXME : remove reference to this from everywhere
		if (gamePaused) Time.timeScale = 0;
		else Time.timeScale = 1;

		// TODO : hide obstacles, civilians and collectables
		Camera.main.cullingMask = Time.timeScale != 0 ? regularCullingMask : gamePauseCullingMask;
	}

	internal float getGameSpeed() {
		return gameSpeed.current;// * (1 - gamePaused);
	}

	// public float colThickness = 4f;
    // public float zPosition = 0f;
    // private Vector2 screenSize;
 
    // void Start ()
    // {//Create a Dictionary to contain all our Objects/Transforms
    //     System.Collections.Generic.Dictionary<string,Transform> colliders = new System.Collections.Generic.Dictionary<string,Transform>();
    // //Create our GameObjects and add their Transform components to the Dictionary we created above
    //     colliders.Add("Top",new GameObject().transform);
    //     colliders.Add("Bottom",new GameObject().transform);
    //     colliders.Add("Right",new GameObject().transform);
    //     colliders.Add("Left",new GameObject().transform);
    // //Generate world space point information for position and scale calculations
    //     Vector3 cameraPos = Camera.main.transform.position;
    //     screenSize.x = Vector2.Distance (Camera.main.ScreenToWorldPoint(new Vector2(0,0)),Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f; //Grab the world-space position values of the start and end positions of the screen, then calculate the distance between them and store it as half, since we only need half that value for distance away from the camera to the edge
    //     screenSize.y = Vector2.Distance (Camera.main.ScreenToWorldPoint(new Vector2(0,0)),Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;
    // //For each Transform/Object in our Dictionary
    //     foreach(KeyValuePair<string,Transform> valPair in colliders)
    //     {
    //         valPair.Value.gameObject.AddComponent<BoxCollider2D>(); //Add our colliders. Remove the "2D", if you would like 3D colliders.
    //         valPair.Value.name = valPair.Key + "Collider"; //Set the object's name to it's "Key" name, and take on "Collider".  i.e: TopCollider
    //         valPair.Value.parent = transform; //Make the object a child of whatever object this script is on (preferably the camera)
 
    //         if(valPair.Key == "Left" || valPair.Key == "Right") //Scale the object to the width and height of the screen, using the world-space values calculated earlier
    //             valPair.Value.localScale = new Vector3(colThickness, screenSize.y * 2, colThickness);
    //         else
    //             valPair.Value.localScale = new Vector3(screenSize.x * 2, colThickness, colThickness);
    //     }  
    // //Change positions to align perfectly with outter-edge of screen, adding the world-space values of the screen we generated earlier, and adding/subtracting them with the current camera position, as well as add/subtracting half out objects size so it's not just half way off-screen
    //     colliders["Right"].position = new Vector3(cameraPos.x + screenSize.x + (colliders["Right"].localScale.x * 0.5f), cameraPos.y, zPosition);
    //     colliders["Left"].position = new Vector3(cameraPos.x - screenSize.x - (colliders["Left"].localScale.x * 0.5f), cameraPos.y, zPosition);
    //     colliders["Top"].position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (colliders["Top"].localScale.y * 0.5f), zPosition);
    //     colliders["Bottom"].position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (colliders["Bottom"].localScale.y * 0.5f), zPosition);
    // }
}
