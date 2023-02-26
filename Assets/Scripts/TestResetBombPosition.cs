using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResetBombPosition : MonoBehaviour
{
	// Start is called before the first frame update
	GenerateBuildings generateBuildings;
	Animator animator;
	bool shouldFall = true;
TrailRenderer bombTrail;

	void Start()
	{
		bombTrail = GetComponent<TrailRenderer>();
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
		
		if (shouldFall)
		{
		// 	transform.Rotate(Vector3.back * Time.deltaTime * 30f);
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
		bombTrail.emitting = false;
		bombTrail.Clear();

		shouldFall = false;
		animator.Play("Bomb_explode");
		// animator.
		
		// gameObject.SetActive(false);
	}

	void OnFinishedExploding() {
		ReturnToPool();
		Destroy(gameObject);
		transform.position = Vector3.up * generateBuildings.boundsHigh.y;
		animator.Play("Bomb_idle");

		bombTrail.emitting = true;
		bombTrail.Clear();

		shouldFall = true;
	}

	void ReturnToPool() {
		// TODO
		Debug.Log("NEED TO RETURN TO POOL");
		// Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Debug.Log("OnTriggerEnter2D");
		if (collision.CompareTag("Spidy")) explode();
		if (collision.CompareTag("Screen Border Bottom")) Destroy(gameObject);//transform.position = Vector3.up * generateBuildings.boundsHigh.y;
	}
}
