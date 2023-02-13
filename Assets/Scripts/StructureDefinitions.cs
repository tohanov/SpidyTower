using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum DamageSource
{
	Grenade,
	Rubble,
}

public enum ItemType
{
	Web,
	Symbiote,
}

public class Stat
{
	Action onUpdatedAction;
	Action onFullAction;
	Action onEmptyAction;
	public float current { get; private set; }
	readonly float max;
	readonly float min;

	Stat(float start, float max, float min, Action onUpdatedAction, Action onFullAction, Action onEmptyAction)
	{
		this.onUpdatedAction = onUpdatedAction;
		this.onFullAction = onFullAction;
		this.onEmptyAction = onEmptyAction;

		this.max = max;
		this.min = min;
		current = Clamped(start);
	}

	public void updateCurrent(float updatedValue)
	{
		float clampedUpdatedValue = Clamped(updatedValue);

		if (current == clampedUpdatedValue)
		{
			return;
		}

		current = clampedUpdatedValue;
		onUpdatedAction();
		
		if (isEmpty())
		{
			onEmptyAction();
		}
		else if (isFull())
		{
			onFullAction();
		}
	}

	float Clamped(float updatedValue)
	{
		return Mathf.Clamp(updatedValue, min, max);
	}

	public bool isEmpty()
	{
		return current == min;
	}

	public bool isFull()
	{
		return current == max;
	}
}