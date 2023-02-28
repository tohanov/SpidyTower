using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class GenerateBuildings : MonoBehaviour
{
	// [SerializeField] GameObject buildingBlock1Large;
	// [SerializeField] GameObject buildingBlock2Large;
	// [SerializeField] GameObject buildingBlock1Small;

	[SerializeField] internal GameObject buildingBlocksRowPrefab;

	[SerializeField] internal GameObject[] regularMiddleBuildingBlockPrefabs;
	[SerializeField] internal GameObject[] openWindowMiddleBuildingBlockPrefabs;

	[SerializeField] internal GameObject[] regularRightBuildingBlockPrefabs;
	[SerializeField] internal GameObject[] openWindowRightBuildingBlockPrefabs;

	// [SerializeField] GameObject[] collectablesPrefabs;
	[SerializeField] internal GameObject symbiotePrefab;
	[SerializeField] internal GameObject webCartridgePrefab;

	internal Vector2 boundsHigh;
	internal Vector2 boundsLow;
	internal Vector3 blockSize;
	int numberOfBlocksToFillScreen;
	// GameObject[] buildingWrappers;

	// public Vector2 wrapperResetPosition;
	float gameSpeed;
	GameState gameState;

	internal Vector3[] buildingPositions;
	float buildingRegenerationHeight;

	internal GameObject topmostRow;
	internal float probabilityConstant;

	internal List<Vector2> rowColliderPoints;

	void Awake()
	{	
		// TODO put the 
		gameState = GetComponent<GameState>();
		boundsHigh = gameState.boundsHigh;
		boundsLow = gameState.boundsLow;

		blockSize = regularRightBuildingBlockPrefabs[0].GetComponent<SpriteRenderer>().bounds.size;

		buildingPositions = new Vector3[] {
			// left
			new Vector3(boundsLow.x + blockSize.x/2, 0),
			// middle
			new Vector3((boundsHigh.x - boundsLow.x)/2 + boundsLow.x, 0),
			// right
			new Vector3(boundsHigh.x - blockSize.x/2, 0),
		};

		numberOfBlocksToFillScreen = Mathf.CeilToInt((boundsHigh.y - boundsLow.y)/blockSize.y);

		// GameObject[] blocksForLeft = new GameObject[3 * numberOfBlocksToFillScreen];

		// buildingWrappers = new GameObject[] {
		// 	new GameObject("buildingsWrapper1"),
		// 	new GameObject("buildingsWrapper2"),
		// };

		// TODO hide building wrapper 2

		// buildingWrappers[0].transform.position = new Vector2(boundsLow.x, boundsHigh.y);
		// buildingWrappers[1].transform.position = new Vector2(boundsLow.x, boundsHigh.y);

		rowColliderPoints = new List<Vector2> {
			new Vector2(boundsLow.x, -blockSize.y/2),
			new Vector2(boundsHigh.x, -blockSize.y/2),
		};
		probabilityConstant = 1f/(3*numberOfBlocksToFillScreen);
		generateInitialBlocks(/*buildingWrappers[0]*/);
		// generateInitialBlocks(/*buildingWrappers[1]*/);

		// wrapperResetPosition = buildingWrappers[1].transform.position = buildingWrappers[0].transform.position + Vector3.up * (boundsHigh.y - boundsLow.y);

		// transform.position = new Vector3(boundsHigh.x, boundsLow.y);

		// foreach (GameObject wrapper in buildingWrappers)
		// {
		// 	// Rigidbody2D temp2 = wrapper.AddComponent<Rigidbody2D>();
		// 	// temp2.isKinematic = true;

		// 	// Add a Rigidbody2D component to the game object this script is attached to
		// 	Rigidbody2D rb = wrapper.AddComponent<Rigidbody2D>();

		// 	// Set the Rigidbody2D properties as desired
		// 	rb.gravityScale = 0f;
		// 	rb.mass = 1.0f;
		// 	rb.drag = 0.0f;
		// 	rb.angularDrag = 0.05f;
		// 	rb.bodyType = RigidbodyType2D.Kinematic;
		// 	rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

		// 	BoxCollider2D temp = wrapper.AddComponent<BoxCollider2D>();
		// 	temp.isTrigger = true;
		// 	wrapper.AddComponent<BuildingsWrapperScript>();
		// }

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

		// foreach (GameObject wrapper in buildingWrappers)
		// {
		// 	// Debug.Log("gamespeed: " + gameState.getGameSpeed());
		// 	wrapper.transform.position += Vector3.down * Time.fixedDeltaTime * gameState.getGameSpeed();
		// }
	}


	void generateBlock(GameObject[] regularCollection, GameObject[] openWindowCollection, float horizontalPosition, Quaternion rotation, GameObject row) {
		float probabilityConstant = 1f/(3*numberOfBlocksToFillScreen);
		
		var isOpenWindow = trueWithProbability(probabilityConstant);
		var containsWebCartridge = trueWithProbability(probabilityConstant);
		var containsSymbiote = trueWithProbability(probabilityConstant);

		// containsSymbiote = false;
		// containsWebCartridge = false;


		GameObject block;

		if (isOpenWindow) {
			block = Instantiate(randomElement(openWindowCollection), row.transform/*, buildingsWrapper.transform*/);
			block.transform.localPosition = new Vector3(horizontalPosition, 0);
		}
		else {
			block = Instantiate(randomElement(regularCollection), row.transform/*, buildingsWrapper.transform*/);
			block.transform.localPosition = new Vector3(horizontalPosition, 0);
		}

		block.transform.rotation = rotation;

		if (containsSymbiote) {
			GameObject temp = Instantiate(symbiotePrefab, block.transform /*, buildingsWrapper.transform*/);
			temp.transform.localPosition = new Vector2(0, Random.Range(0, blockSize.y));
		}

		if (containsWebCartridge) {
			GameObject temp = Instantiate(webCartridgePrefab, block.transform /*, buildingsWrapper.transform*/);
			temp.transform.localPosition = new Vector2(0, Random.Range(0, blockSize.y));
		}

		// block.transform.position = position;
		// block.transform.rotation = rotation;
	}


	void generateRowAtHeight(float height) {
		var fixedHeight = height + boundsLow.y + blockSize.y / 2;

		GameObject row = Instantiate(buildingBlocksRowPrefab);
		RowBlocksGenerator bbGen = row.GetComponent<RowBlocksGenerator>();
		bbGen.generateBuildingsScript = this;
		bbGen.setupCollider();
		bbGen.generateRow();

		// bbGen.probabilityConstant = probabilityConstant;
		// bbGen.blockPositions = buildingPositions;
		// bbGen.blockSize = blockSize;
		// bbGen.openWindowMiddleBuildingBlockPrefabs = openWindowMiddleBuildingBlockPrefabs;
		// bbGen.openWindowRightBuildingBlockPrefabs = openWindowRightBuildingBlockPrefabs;
		// bbGen.regularMiddleBuildingBlockPrefabs = regularMiddleBuildingBlockPrefabs;
		// bbGen.regularRightBuildingBlockPrefabs = regularRightBuildingBlockPrefabs;
		// bbGen.symbiotePrefab = symbiotePrefab;
		// bbGen.webCartridgePrefab = webCartridgePrefab;

		// generateBlock(regularRightBuildingBlockPrefabs, openWindowRightBuildingBlockPrefabs, buildingPositions[0], Quaternion.Euler(0,180,0), row);
		// // var block = Instantiate(randomElement(regularRightBuildingBlockPrefabs)/*, buildingsWrapper.transform*/);
		// // block.transform.position = 
		// // block.transform.rotation = Quaternion.Euler(0,180,0);

		// // Destroy(buildingBlockMiddle);
		// generateBlock(regularRightBuildingBlockPrefabs, openWindowRightBuildingBlockPrefabs, buildingPositions[1], Quaternion.identity, row);
		// // block = Instantiate(randomElement(regularMiddleBuildingBlockPrefabs)/*, buildingsWrapper.transform*/);
		// // block.transform.position = new Vector2(buildingPositions[1], height + boundsLow.y + blockSize.y / 2);

		// // Destroy(buildingBlockRight);
		// generateBlock(regularRightBuildingBlockPrefabs, openWindowRightBuildingBlockPrefabs, buildingPositions[2], Quaternion.identity, row);
		// // block = Instantiate(randomElement(regularRightBuildingBlockPrefabs)/*, buildingsWrapper.transform*/); // Instantiate(buildingBlock1Large);
		// // block.transform.position = new Vector2(buildingPositions[2], height + boundsLow.y + blockSize.y / 2);
		// // buildingBlockRight.transform.localScale = Vector3.Cross(buildingBlockRight.transform.localScale, Vector3.left);
		// // buildingBlockRight.transform.rotation = Quaternion.Euler(180,0,0);
		
		row.transform.position = new Vector3(0, fixedHeight);
		topmostRow = row;
	}


	void generateInitialBlocks(/*GameObject buildingsWrapper*/)
	{
		float previousHeight = 0;

		// creating 2 more rows over what's necessary to fill the screen, to make regeneration
		for (int i = 1; i <= numberOfBlocksToFillScreen + 2; ++i) {
			// Destroy(buildingBlockLeft);

			generateRowAtHeight(previousHeight);

			previousHeight += blockSize.y;
		}

		buildingRegenerationHeight = previousHeight - blockSize.y;
	}

	
	T randomElement<T>(T[] a) {
		return a[Random.Range(0, a.Length*10) / 10];
	}


	bool trueWithProbability(float a) {
		return a > Random.Range(0f, 100)/100;
	}


	void regenerateBlocks() {
		float height = topmostRow.transform.position.y + blockSize.y/2;

		generateRowAtHeight(height);
	}

	// void OnTriggerEnter2D() {
	// 	Debug.Log("GenerateBuilding's OnTriggerEnter2D");
	// }
}
