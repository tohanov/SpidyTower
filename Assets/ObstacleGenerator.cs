using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
	[SerializeField] GameObject rubbleSpawner;
	[SerializeField] GameObject civilianSpawner;
	[SerializeField] GameObject bombSpawner;

	GameState gameStateScript;
	GenerateBuildings generateBuildingsScript;

	// TODO
	// Vector3[] rubbleSpawnerPositions;
	// Vector3 bombSpawnerBounds;
	// Vector3[,] civilianSpawnerBounds;
	// WaitForSeconds spawnDelay;
	// [SerializeField] GameObject bombPrefab;
	// [SerializeField] GameObject[] rubbleLargeBricksPrefab;
	// [SerializeField] GameObject civilianPrefab;
	// spawnDelay = new WaitForSeconds(2);

	void Awake() {
		var gameController = GameObject.FindGameObjectWithTag("GameController");
		gameStateScript = gameController.GetComponent<GameState>();
		generateBuildingsScript = gameController.GetComponent<GenerateBuildings>();


		initializeSpawnerPositions();
		transform.position = new Vector3(0, gameStateScript.boundsHigh.y * 1.05f);
	}

	void initializeSpawnerPositions() {
		Instantiate(rubbleSpawner, transform);
		Instantiate(civilianSpawner, transform);
		Instantiate(bombSpawner, transform);
	}
}
