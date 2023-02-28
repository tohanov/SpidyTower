using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
	[SerializeField] GameObject rubbleSpawner;
	[SerializeField] GameObject civilianSpawner;
	[SerializeField] GameObject bombSpawner;

	[SerializeField] GameObject obstaclePrefabs;

	GameState gameStateScript;
	GenerateBuildings generateBuildingsScript;

	// TODO
	Vector3[] rubbleSpawnPositions;
	Vector2 boundsHigh;
	Vector2 boundsLow;
	PlayerState playerStateScript;

	// Vector3 bombSpawnerBounds;
	// Vector3[,] civilianSpawnerBounds;
	[SerializeField] GameObject bombPrefab;
	[SerializeField] GameObject[] rubblePrefabs;
	// [SerializeField] GameObject civilianPrefab;
	WaitForSeconds spawnDelay;

	void Awake() {
		var gameController = GameObject.FindGameObjectWithTag("GameController");
		gameStateScript = gameController.GetComponent<GameState>();
		generateBuildingsScript = gameController.GetComponent<GenerateBuildings>();
		
		playerStateScript = GameObject.FindGameObjectWithTag("Spidy").GetComponent<PlayerState>();

		// initializeSpawnerPositions();


		spawnDelay = new WaitForSeconds(3);
	}

	void Start() {
		boundsHigh = gameStateScript.boundsHigh;
		boundsLow = gameStateScript.boundsLow;

		transform.position = new Vector3(0, boundsHigh.y * 1.05f);

		// get suitable positions to spawn obstacles
		rubbleSpawnPositions = generateBuildingsScript.buildingPositions;

		// start a coroutine for generating obstacles
			// wait for time until next generation of an obstacle
			// randomly choose which obstacle to generate
			// run appropriate generation function using a pool
				// configure random appropriate start position
				// configure initial trajectory of movement

		StartCoroutine(ObstacleSpawningCoroutine());
	}

	// void initializeSpawnerPositions() {
	// 	Instantiate(rubbleSpawner, transform);
	// 	Instantiate(civilianSpawner, transform);
	// 	Instantiate(bombSpawner, transform);
	// }

	IEnumerator ObstacleSpawningCoroutine() {
		
		Debug.Log("In spawner coroutine");

		while (true) {
			yield return spawnDelay;

			Debug.Log("In spawner loop");
			// random type of obstacle
			int type = Random.Range(0, 3);

			if (type <= 1) { // chose rubble

				// random building position
				int positionIndex = Random.Range(0, 3);
				// float spawnPosition = rubbleSpawnPositions[positionIndex];


				var rubbleObject = Instantiate(rubblePrefabs[type], transform);
				rubbleObject.transform.localPosition = rubbleSpawnPositions[positionIndex];
				rubbleObject.GetComponent<Rigidbody2D>().velocity = Vector3.down * 2;
			}
			else {
				bool aimedAtSpidy = Util.trueWithProbability(0.5f); // TODO
				Vector3 bombTargetPosition;

				float sourceRandomX = Random.Range(boundsLow.x, boundsHigh.x);

				if (aimedAtSpidy) {
					Debug.Log("aimed at spidy" + Random.Range(0f,1));
					// bombTargetPosition = playerStateScript.transform.position;
					bombTargetPosition = playerStateScript.getPositionAsVector3();
				}
				else {
					Debug.Log("NOT aimed at spidy" + Random.Range(0f,1));
					float targetRandomX = Random.Range(rubbleSpawnPositions[0].x, rubbleSpawnPositions[2].x);
					float targetRandomY = Random.Range(
						boundsLow.y,
						(boundsHigh.y - boundsLow.y) * 0.75f + boundsLow.y
					);

					bombTargetPosition = new Vector3(targetRandomX, targetRandomY);
				}

				var bombObject = Instantiate(bombPrefab, transform);
				bombObject.transform.localPosition = Vector3.right * sourceRandomX;
				// bombObject.transform.position = new Vector3(sourceRandomX, boundsHigh.y);

				var heading = bombTargetPosition - bombObject.transform.position;
				var direction = heading / heading.magnitude; // This is now the normalized direction.

				Debug.DrawLine(bombObject.transform.position, bombTargetPosition, Color.green, 1f);
				bombObject.GetComponent<Rigidbody2D>().velocity = direction * 4;
			}
		}
	}

}
