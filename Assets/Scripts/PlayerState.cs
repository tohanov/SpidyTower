using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
	Stat redHalfHearts;
	Stat blackHalfHearts;

	Stat missedPeople;
	Stat webs;

	GameState gameState;
	Dictionary<DamageSource, float> healthDamage;
	Dictionary<DamageSource, float> speedDamage;

	void Awake()
	{

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
			redrawRedHearts(),
			deathAndGameOver(),
			null
		);

		blackHalfHearts = new Stat(
			0,
			0,
			float.PositiveInfinity,
			redrawBlackHearts() and updatePlayerStats(),
			updateGameSpeed(),
			null
		);

		webs = new Stat(
			5,
			0,
			5,
			redrawWebsCounter() and enableUnloadingOfCivilianIfHasWebs(),
			disableUnloadingOfCivilian(),
			null
		);

		missedPeople = new Stat(
			0,
			0,
			5,
			redrawMissedCiviliansCounter(),
			null,
			failAndEndGame()
		);

		// GetComponent<GameState>();
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
}
