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
	Dictionary<DamageSource, float> healthDamage;
	Dictionary<DamageSource, float> speedDamage;

	Animator animator;

	void Awake()
	{
		animator = gameObject.GetComponent<Animator>();

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

	internal void Move(InputAction.CallbackContext context)
	{
		Vector2 instruction = context.ReadValue<Vector2>();
		// if () {
		// 	animator.Play(("Spidy_shoot_left" : "Spidy_shoot_right"));
		// }
		Debug.Log(instruction);
	}
}
