using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfScreenObjectsDestroyer : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Building Blocks Row") != true /* && collision.CompareTag("Spidy") != true */) {
			Debug.Log("Destroying " + collision.tag);
			Destroy(collision.gameObject);
		}
	}
}
