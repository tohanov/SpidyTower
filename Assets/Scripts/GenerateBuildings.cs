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

	void Awake()
	{	
		boundsHigh = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		boundsLow = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
		blockSize = buildingBlock1Large.GetComponent<SpriteRenderer>().bounds.size;

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

		EdgeCollider2D wrapperSwitchTrigger = new GameObject().AddComponent<EdgeCollider2D>();
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
			wrapper.transform.position += Vector3.down * Time.fixedDeltaTime;
		}
	}

	void generateBlocks(GameObject buildingsWrapper)
	{
		float previousHeight = 0;

		for (int i = 0; i < numberOfBlocksToFillScreen; ++i) {
			// Destroy(buildingBlockRight);
			buildingBlockRight = Instantiate(buildingBlock1Large, buildingsWrapper.transform); // Instantiate(buildingBlock1Large);
			buildingBlockRight.transform.position = new Vector2(boundsHigh.x - blockSize.x / 2, previousHeight + boundsLow.y + blockSize.y / 2);

			// Destroy(buildingBlockLeft);
			buildingBlockRight = Instantiate(buildingBlock1Large, buildingsWrapper.transform);
			buildingBlockRight.transform.position = new Vector2(boundsLow.x + blockSize.x / 2, previousHeight + boundsLow.y + blockSize.y / 2);

			// Destroy(buildingBlockMiddle);
			buildingBlockRight = Instantiate(buildingBlock1Large, buildingsWrapper.transform);
			buildingBlockRight.transform.position = new Vector2((boundsHigh.x - boundsLow.x)/2 + boundsLow.x, previousHeight + boundsLow.y + blockSize.y / 2);

			previousHeight += blockSize.y;
		}
	}
}
