using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfScreenObjectsDestroyer : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Building Blocks Row") != true) {
			Destroy(collision.gameObject);
		}
	}
}
