using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity;
using Random = UnityEngine.Random;

public class BuildingBlocksGenerator : MonoBehaviour
{
	GameObject[] heldBlocks;
	internal GenerateBuildings generateBuildingsScript;

	// internal GameObject[] regularMiddleBuildingBlockPrefabs;
	// internal GameObject[] openWindowMiddleBuildingBlockPrefabs;

	// internal GameObject[] regularRightBuildingBlockPrefabs;
	// internal GameObject[] openWindowRightBuildingBlockPrefabs;

	// // [SerializeField] GameObject[] collectablesPrefabs;
	// internal GameObject symbiotePrefab;
	// internal GameObject webCartridgePrefab;

	// internal float probabilityConstant;

	// internal Vector3[] blockPositions;
	// internal Vector2 blockSize;
	// internal Vector2 boundsHigh;
	// internal Vector2 boundsLow;

	void Awake() {
		heldBlocks = new GameObject[3];
	}

	internal void setupCollider() {
		// EdgeCollider2D triggerCollider = gameObject.AddComponent<EdgeCollider2D>();
		// triggerCollider.SetPoints(generateBuildingsScript.rowColliderPoints);
		// triggerCollider.isTrigger = true;

		CircleCollider2D c = gameObject.AddComponent<CircleCollider2D>();
		c.isTrigger = true;
		c.radius = generateBuildingsScript.blockSize.y/2;
	}


	bool trueWithProbability(float a) {
		return a > Random.Range(0f, 100)/100;
	}
	
	internal void generateRow() {
		// var fixedHeight = height + boundsLow.y + blockSize.y / 2;

		// GameObject row = new GameObject("Building Blocks Row");
		// row.tag = "Building Blocks Row";

		heldBlocks[0] = generateBlock(
			generateBuildingsScript.regularRightBuildingBlockPrefabs, 
			generateBuildingsScript.openWindowRightBuildingBlockPrefabs, 
			generateBuildingsScript.buildingPositions[0],
			Quaternion.Euler(0,180,0)
		);
		// var block = Instantiate(randomElement(regularRightBuildingBlockPrefabs)/*, buildingsWrapper.transform*/);
		// block.transform.position = 
		// block.transform.rotation = Quaternion.Euler(0,180,0);

		// Destroy(buildingBlockMiddle);
		heldBlocks[1] = generateBlock(
			generateBuildingsScript.regularMiddleBuildingBlockPrefabs, 
			generateBuildingsScript.openWindowMiddleBuildingBlockPrefabs, 
			generateBuildingsScript.buildingPositions[1],
			Quaternion.identity
		);
		// block = Instantiate(randomElement(regularMiddleBuildingBlockPrefabs)/*, buildingsWrapper.transform*/);
		// block.transform.position = new Vector2(buildingPositions[1], height + boundsLow.y + blockSize.y / 2);

		// Destroy(buildingBlockRight);
		heldBlocks[2] = generateBlock(
			generateBuildingsScript.regularRightBuildingBlockPrefabs, 
			generateBuildingsScript.openWindowRightBuildingBlockPrefabs, 
			generateBuildingsScript.buildingPositions[2],
			Quaternion.identity
		);
		// block = Instantiate(randomElement(regularRightBuildingBlockPrefabs)/*, buildingsWrapper.transform*/); // Instantiate(buildingBlock1Large);
		// block.transform.position = new Vector2(buildingPositions[2], height + boundsLow.y + blockSize.y / 2);
		// buildingBlockRight.transform.localScale = Vector3.Cross(buildingBlockRight.transform.localScale, Vector3.left);
		// buildingBlockRight.transform.rotation = Quaternion.Euler(180,0,0);
		// row.transform.position = new Vector3(0, fixedHeight);

		// lastGeneratedRow = row;
	}

	GameObject generateBlock(GameObject[] regularCollection, GameObject[] openWindowCollection, Vector3 blockPosition, Quaternion rotation) {
		var isOpenWindow = trueWithProbability(generateBuildingsScript.probabilityConstant);
		var containsWebCartridge = trueWithProbability(generateBuildingsScript.probabilityConstant);
		var containsSymbiote = trueWithProbability(generateBuildingsScript.probabilityConstant);

		GameObject block;

		if (isOpenWindow) {
			block = Instantiate(randomElement(openWindowCollection), transform/*, buildingsWrapper.transform*/);
		}
		else {
			block = Instantiate(randomElement(regularCollection), transform/*, buildingsWrapper.transform*/);
		}

		block.transform.localPosition = blockPosition;
		block.transform.rotation = rotation;

		if (containsSymbiote) {
			GameObject temp = Instantiate(generateBuildingsScript.symbiotePrefab, block.transform /*, buildingsWrapper.transform*/);
			temp.transform.localPosition = new Vector2(0, Random.Range(0, generateBuildingsScript.blockSize.y));
		}

		if (containsWebCartridge) {
			GameObject temp = Instantiate(generateBuildingsScript.webCartridgePrefab, block.transform /*, buildingsWrapper.transform*/);
			temp.transform.localPosition = new Vector2(0, Random.Range(0, generateBuildingsScript.blockSize.y));
		}

		return block;
	}
	

	T randomElement<T>(T[] a) {
		return a[Random.Range(0, a.Length*10) / 10];
	}


	void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log(collision.tag);

		if (collision.CompareTag("Screen Border/Bottom")) {
			Debug.Log("row collided");

			foreach (GameObject go in heldBlocks) {
				Destroy(go);
			}

			generateRow();

			transform.position = generateBuildingsScript.topmostRow.transform.position + new Vector3(0, generateBuildingsScript.blockSize.y);
			generateBuildingsScript.topmostRow = gameObject;
		}
	}
}
