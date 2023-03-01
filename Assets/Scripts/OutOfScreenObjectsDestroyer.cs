using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfScreenObjectsDestroyer : MonoBehaviour
{
	PlayerState playerState;

	void Start() {
		playerState = GameObject.FindGameObjectWithTag("Spidy").GetComponent<PlayerState>();
	}

	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Building Blocks Row") == true) return;
		
		if (collision.CompareTag("Collectables/Civilian") && collision.gameObject.GetComponent<CivilianState>().state == CivilianState.State.Falling) {
			playerState.incrementMissedCivilians();
		}

		Debug.Log("Destroying " + collision.tag);
		Destroy(collision.gameObject);
	}
}
