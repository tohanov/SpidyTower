using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerState : MonoBehaviour
{
	Stat redHalfHearts;
	Stat blackHalfHearts;

	Stat missedPeople;
	Stat webs;

	GameState gameState;
	
	GenerateBuildings generateBuildingsScript;
	Dictionary<DamageSource, float> healthDamage;
	Dictionary<DamageSource, float> speedDamage;
	// float[] spidyVerticalPositions;
	// float[] spidyHorizontalPositions;
	Animator animator;
	int[] playerCurrentPosition;
	Vector3[,] spidyPositions;

	void Start()
	{
		// TODO make the game manager spawn the player (at the correct position) and move all the Start() to an Awake() function
		generateBuildingsScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GenerateBuildings>();
		animator = gameObject.GetComponent<Animator>();
		
		playerCurrentPosition = new int[] {1, 0};
		populatePositions();
		transform.position = spidyPositions[1, 0];

		healthDamage = new Dictionary<DamageSource, float>() {
			{DamageSource.Grenade, 1f},
			{DamageSource.Rubble, 0.5f}
		};

		speedDamage = new Dictionary<DamageSource, float>() {
			{DamageSource.Grenade, 1f},
			{DamageSource.Rubble, 0}
		};

		redHalfHearts = new Stat(
			6,
			0,
			6,
			redrawRedHearts,
			deathAndGameOver,
			null
		);

		blackHalfHearts = new Stat(
			0,
			0,
			float.PositiveInfinity,
			() => {redrawBlackHearts(); updatePlayerStats();},
			updateGameSpeed,
			null
		);

		webs = new Stat(
			5,
			0,
			5,
			() => {redrawWebsCounter(); enableUnloadingOfCivilianIfHasWebs();},
			disableUnloadingOfCivilian,
			null
		);

		missedPeople = new Stat(
			0,
			0,
			5,
			redrawMissedCiviliansCounter,
			null,
			failAndEndGame
		);

		// GetComponent<GameState>();
	}

	// void Start() {
	// 	playerCurrentPosition = new int[] {1, 0};
	// 	transform.position = spidyPositions[1, 0];
	// }

	private void populatePositions()
	{
		spidyPositions = new Vector3[3,3];

		Vector2 boundsHigh = generateBuildingsScript.boundsHigh;
		Vector2 boundsLow = generateBuildingsScript.boundsLow;
		Vector2 blockSize = generateBuildingsScript.blockSize;

		float intervalX = (boundsHigh.x - boundsLow.x)/2f - blockSize.x * 0.5f;
		
		// spidyHorizontalPositions = new float[3]; // according to block sizes and screen corners
		// spidyHorizontalPositions[0] = boundsLow.x + blockSize.x * 0.5f;
		// spidyHorizontalPositions[1] = (boundsHigh.x - boundsLow.x)/2;
		// spidyHorizontalPositions[2] = boundsHigh.x - blockSize.x * 0.5f;


		Vector2 playerSize = gameObject.GetComponent<SpriteRenderer>().size;
		// spidyVerticalPositions = new float[3]; // divide screen to sevenths and use spaces 1-2, 3-4, 5-6
		float intervalY = (boundsHigh.y - boundsLow.y)/7;
		
		// spidyHorizontalPositions[0] = boundsLow.y + intervalY + playerSize.y*0.5f;
		// spidyHorizontalPositions[1] = spidyVerticalPositions[0] + intervalY*2;
		// spidyHorizontalPositions[2] = spidyVerticalPositions[1] + intervalY*2;

		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				spidyPositions[i,j] = new Vector3(
					boundsLow.x + blockSize.x * 0.5f + intervalX * i,
					boundsLow.y + intervalY + playerSize.y*0.5f + intervalY*2*j
				);
				// spidyPositions = new Vector3(
				// 	,
				// 	boundsLow.y + intervalY + playerSize.y * 0.5f,
				// 	0
				// 	);
				Debug.Log(spidyPositions[i,j]);
			}
		}
	}

	private void failAndEndGame()
	{
		throw new NotImplementedException();
	}

	private void redrawMissedCiviliansCounter()
	{
		throw new NotImplementedException();
	}

	private void redrawRedHearts()
	{
		throw new NotImplementedException();
	}

	private void deathAndGameOver()
	{
		throw new NotImplementedException();
	}

	private void updateGameSpeed()
	{
		throw new NotImplementedException();
	}

	private void updatePlayerStats()
	{
		throw new NotImplementedException();
	}

	private void redrawBlackHearts()
	{
		throw new NotImplementedException();
	}

	private void enableUnloadingOfCivilianIfHasWebs()
	{
		throw new NotImplementedException();
	}

	private void redrawWebsCounter()
	{
		throw new NotImplementedException();
	}

	private void disableUnloadingOfCivilian()
	{
		throw new NotImplementedException();
	}

	public void takeDamage(DamageSource damageSource)
	{
		Stat relevantHealthbar = redHalfHearts;

		if (!blackHalfHearts.isEmpty())
		{
			relevantHealthbar = blackHalfHearts;
		}

		relevantHealthbar.updateCurrent(relevantHealthbar.current - healthDamage[damageSource]);
		gameState.gameSpeed.updateCurrent(gameState.gameSpeed.current - speedDamage[damageSource]); // TODO : might want to make immune to speed loss when have black hearts
	}

	public void collectItem(ItemType itemType)
	{
		throw new NotImplementedException();
	}
	
	public void Fire(InputAction.CallbackContext context) {
		// Debug.Log("Fired " + (context.ReadValue<float>() < 0 ? "left" : "right"));
		animator.Play((context.ReadValue<float>() < 0 ? "Spidy_shoot_left" : "Spidy_shoot_right"));
	}

	// internal void Move(InputAction.CallbackContext context)
	// {
	// 	Vector2 instruction = context.ReadValue<Vector2>();
	// 	// if () {
	// 	// 	animator.Play(("Spidy_shoot_left" : "Spidy_shoot_right"));
	// 	// }
	// 	Debug.Log(instruction);
	// }

	internal void ShortJump(int x)
	{
		int newPositionX = Mathf.Clamp(playerCurrentPosition[0] + x, 0, 2);

		if (newPositionX != playerCurrentPosition[0]) {
			animator.Play(x < 0 ? "Spidy_jump_left" : "Spidy_jump_right");
			
			transform.position = spidyPositions[newPositionX, playerCurrentPosition[1]];
			// transform.localPosition = Vector3.zero;//spidyPositions[newPosition, playerCurrentPosition[1]];
			// transform.TransformPoint(Vector2.one);
			playerCurrentPosition[0] = newPositionX;
		}

		Debug.Log("updated position: " + string.Join(", ", playerCurrentPosition));
	}

	internal void MoveVertically(int y)
	{
		int newPositionY = Mathf.Clamp(playerCurrentPosition[1] + y, 0, 2);
		
		if (newPositionY != playerCurrentPosition[1]) {
			// animator.Play(y < 0 ? "Spidy_jump_left" : "Spidy_jump_right");
			
			transform.position = spidyPositions[playerCurrentPosition[0], newPositionY];
			// transform.localPosition = Vector3.zero;//spidyPositions[newPosition, playerCurrentPosition[1]];
			// transform.TransformPoint(Vector2.one);
			playerCurrentPosition[1] = newPositionY;
		}

		Debug.Log("updated position: " + string.Join(", ", playerCurrentPosition));
	}
}
