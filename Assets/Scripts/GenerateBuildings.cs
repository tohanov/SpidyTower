using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class GenerateBuildings : MonoBehaviour
{
	[SerializeField] GameObject buildingBlock1Large;
	[SerializeField] GameObject buildingBlock2Large;
	[SerializeField] GameObject buildingBlock1Small;

	GameObject buildingBlockRight;
	GameObject buildingBlockLeft;
	GameObject buildingBlockMiddle;

	public Vector2 boundsHigh;
	public Vector2 boundsLow;
	public Vector3 blockSize;
	int numberOfBlocksToFillScreen;
	GameObject[] buildingWrappers;

	public Vector2 wrapperResetPosition;
	float gameSpeed;
	GameState gameState;

	internal float[] buildingPositions;

	void Awake()
	{	
		// TODO put the 
		gameState = GetComponent<GameState>();
		boundsHigh = gameState.boundsHigh;
		boundsLow = gameState.boundsLow;

		blockSize = buildingBlock1Large.GetComponent<SpriteRenderer>().bounds.size;

		buildingPositions = new float[] {
			// left
			boundsLow.x + blockSize.x/2,
			// middle
			(boundsHigh.x - boundsLow.x)/2 + boundsLow.x,
			// right
			boundsHigh.x - blockSize.x/2
		};

		numberOfBlocksToFillScreen = Mathf.CeilToInt((boundsHigh.y - boundsLow.y)/blockSize.y);

		GameObject[] blocksForLeft = new GameObject[3 * numberOfBlocksToFillScreen];

		buildingWrappers = new GameObject[] {
			new GameObject("buildingsWrapper1"),
			new GameObject("buildingsWrapper2"),
		};

		// TODO hide building wrapper 2

		buildingWrappers[0].transform.position = new Vector2(boundsLow.x, boundsHigh.y);
		buildingWrappers[1].transform.position = new Vector2(boundsLow.x, boundsHigh.y);
		generateBlocks(buildingWrappers[0]);
		generateBlocks(buildingWrappers[1]);

		wrapperResetPosition = buildingWrappers[1].transform.position = buildingWrappers[0].transform.position + Vector3.up * (boundsHigh.y - boundsLow.y);

		foreach (GameObject wrapper in buildingWrappers)
		{
			// Rigidbody2D temp2 = wrapper.AddComponent<Rigidbody2D>();
			// temp2.isKinematic = true;

			// Add a Rigidbody2D component to the game object this script is attached to
			Rigidbody2D rb = wrapper.AddComponent<Rigidbody2D>();

			// Set the Rigidbody2D properties as desired
			rb.gravityScale = 0f;
			rb.mass = 1.0f;
			rb.drag = 0.0f;
			rb.angularDrag = 0.05f;
			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

			BoxCollider2D temp = wrapper.AddComponent<BoxCollider2D>();
			temp.isTrigger = true;
			wrapper.AddComponent<BuildingsWrapperScript>();
		}

		EdgeCollider2D wrapperSwitchTrigger = new GameObject("Screen Bottom Trigger").AddComponent<EdgeCollider2D>();
		wrapperSwitchTrigger.isTrigger = true;
		wrapperSwitchTrigger.SetPoints(new List<Vector2>{
			new Vector2(boundsHigh.x, boundsLow.y),
			new Vector2(boundsLow.x, boundsLow.y),
		});

		// colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
		// EdgeCollider2D leftEdge = new GameObject("leftEdge").AddComponent<EdgeCollider2D>();
        // colliderpoints = leftEdge.points;
        // colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
        // colliderpoints[1] = new Vector2(lDCorner.x, rUCorner.y);
	}

	void FixedUpdate()
	{
		// if (Input.GetMouseButtonDown(0))
		// {
			
		// }

		// transform.position += Vector3.down * Time.fixedDeltaTime;

		foreach (GameObject wrapper in buildingWrappers)
		{
			// Debug.Log("gamespeed: " + gameState.getGameSpeed());
			wrapper.transform.position += Vector3.down * Time.fixedDeltaTime * gameState.getGameSpeed();
		}
	}


	void generateBlocks(GameObject buildingsWrapper)
	{
		float previousHeight = 0;

		for (int i = 0; i < numberOfBlocksToFillScreen; ++i) {
			// Destroy(buildingBlockLeft);
			buildingBlockRight = Instantiate(buildingBlock1Large, buildingsWrapper.transform);
			buildingBlockRight.transform.position = new Vector2(buildingPositions[0], previousHeight + boundsLow.y + blockSize.y / 2);
			buildingBlockRight.transform.rotation = Quaternion.Euler(0,180,0);

			// Destroy(buildingBlockMiddle);
			buildingBlockRight = Instantiate(buildingBlock1Large, buildingsWrapper.transform);
			buildingBlockRight.transform.position = new Vector2(buildingPositions[1], previousHeight + boundsLow.y + blockSize.y / 2);

			// Destroy(buildingBlockRight);
			buildingBlockRight = Instantiate(buildingBlock1Large, buildingsWrapper.transform); // Instantiate(buildingBlock1Large);
			buildingBlockRight.transform.position = new Vector2(buildingPositions[2], previousHeight + boundsLow.y + blockSize.y / 2);
			// buildingBlockRight.transform.localScale = Vector3.Cross(buildingBlockRight.transform.localScale, Vector3.left);
			// buildingBlockRight.transform.rotation = Quaternion.Euler(180,0,0);

			previousHeight += blockSize.y;
		}
	}
}
