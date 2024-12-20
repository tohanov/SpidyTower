using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDownwards : MonoBehaviour
{
	/* [SerializeField]  */float fallingSpeed;
	GameState gameStateScript;
	void Start() {
		var gameController = GameObject.FindGameObjectWithTag("GameController");
		gameStateScript = gameController.GetComponent<GameState>();

		fallingSpeed = gameStateScript.movementSpeed;
	}

	void FixedUpdate() {
		transform.position += gameStateScript.getOverallSpeed() * Vector3.down * Time.fixedDeltaTime;
	}
}
