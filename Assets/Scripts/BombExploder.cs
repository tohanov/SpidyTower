using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExploder : MonoBehaviour
{
	// Start is called before the first frame update
	GenerateBuildings generateBuildings;
	Animator animator;
	bool shouldFall = true;
	TrailRenderer bombTrail;
	
	GameState gameState;

	void Start()
	{
		bombTrail = GetComponent<TrailRenderer>();
		animator = gameObject.GetComponent<Animator>();
		generateBuildings = GameObject.FindGameObjectWithTag("GameController").GetComponent<GenerateBuildings>();

		gameState = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameState>();
	}

	// Update is called once per frame
	void Update()
	{
		// if (Input.GetMouseButtonDown(0)){
		// 	// if (isActiveAndEnabled) {
		// 		Debug.Log("explode " + UnityEngine.Random.Range(1,2));
		// 		explode();
			// }
			// else {
			// 	recreate();
			// }
		// }
		
		// if (shouldFall)
		// {
		// // 	transform.Rotate(Vector3.back * Time.deltaTime * 30f);
		// 	transform.position += Vector3.down * Time.deltaTime * 7f;
		// }
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

		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		gameState.shootingMovementStopper = 0;

		shouldFall = false;
		animator.Play("Bomb_explode");
		Camera.main.GetComponent<ScreenShake>().performShake();
		// animator.
		
		// gameObject.SetActive(false);
	}

	void OnFinishedExploding() {

		gameState.shootingMovementStopper = 1;

		ReturnToPool();
		transform.position = Vector3.up * generateBuildings.boundsHigh.y;
		animator.Play("Bomb_idle");

		bombTrail.emitting = true;
		bombTrail.Clear();

		shouldFall = true;
	}

	void ReturnToPool() {
		// TODO
		Debug.Log("NEED TO RETURN TO POOL");
		Destroy(gameObject);
		// Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Debug.Log("OnTriggerEnter2D");
		if (collision.CompareTag("Spidy") && !collision.gameObject.GetComponent<PlayerState>().isInvincible) explode();
		// if (collision.CompareTag("Screen Border/Bottom")) Destroy(gameObject);//transform.position = Vector3.up * generateBuildings.boundsHigh.y;
	}
}
