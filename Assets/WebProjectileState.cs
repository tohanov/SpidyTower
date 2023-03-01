using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProjectileState : MonoBehaviour
{
    // Start is called before the first frame update
	static Vector3 spawnPoint = new Vector3(0.7f, 0.1f);
	[SerializeField] float projectileSpeed = 10;
	int direction = 1;

    void Awake() {
		transform.localPosition = spawnPoint;
		// transform.rotation = transform.parent.rotation;
		if (transform.parent.rotation != Quaternion.identity) {
			direction = -1;
		}
		transform.parent = null;
	}

	void Update() {
		transform.position += (direction * projectileSpeed * Time.deltaTime) * Vector3.right;
	}
	// onTrigger
}
