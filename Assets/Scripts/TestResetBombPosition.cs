using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResetBombPosition : MonoBehaviour
{
	// Start is called before the first frame update
	GenerateBuildings generateBuildings;
	Animator animator;

	void Start()
	{
		animator = gameObject.GetComponent<Animator>();
		generateBuildings = GameObject.FindGameObjectWithTag("GameController").GetComponent<GenerateBuildings>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0)){
			// if (isActiveAndEnabled) {
				Debug.Log("explode " + UnityEngine.Random.Range(1,2));
				explode();
			// }
			// else {
			// 	recreate();
			// }
		}
		
		if (isActiveAndEnabled)
		{
			transform.Rotate(Vector3.back * Time.deltaTime * 30f);
			transform.position += Vector3.down * Time.deltaTime * 7f;
		}
	}

	// private void recreate()
	// {
	// 	animator.Play("Bomb_idle");
	// 	transform.position = Vector3.up * generateBuildings.boundsHigh.y;
	// 	gameObject.SetActiveRecursively(true);
	// }

	void explode()
	{
		animator.Play("Bomb_explode");
		// animator.
		
		// gameObject.SetActive(false);
	}

	void OnFinishedExploding() {
		ReturnToPool();
	}

	void ReturnToPool() {
		// TODO
		Debug.Log("ReturnToPool");
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Debug.Log("OnTriggerEnter2D");

		transform.position = Vector3.up * generateBuildings.boundsHigh.y;
	}
}
