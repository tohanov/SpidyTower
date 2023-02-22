using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsWrapperScript : MonoBehaviour
{

	GenerateBuildings generateBuildings;

	void Start() {
		// Debug.Log("Start");
		generateBuildings = GameObject.FindGameObjectWithTag("GameController").GetComponent<GenerateBuildings>();

	}

	private void OnTriggerEnter2D(Collider2D collision) {
		// Debug.Log("OnTriggerEnter2D");
		
		transform.position = generateBuildings.wrapperResetPosition;
	}
}
