using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum DamageSource
{
	Bomb,
	Rubble,
}

public enum ItemType
{
	WebCartridge,
	Symbiote,
}

public class Stat
{
	internal Action onUpdatedAction;
	Action onFullAction;
	Action onEmptyAction;
	public int current { get; private set; }
	internal int max;
	internal int min;

	public Stat(int start, int min, int max, Action onUpdatedAction, Action onFullAction, Action onEmptyAction)
	{
		this.onUpdatedAction = onUpdatedAction;
		this.onFullAction = onFullAction;
		this.onEmptyAction = onEmptyAction;

		this.max = max;
		this.min = min;
		current = Clamped(start);
	}

	public void updateCurrent(int updatedValue)
	{
		int clampedUpdatedValue = Clamped(updatedValue);

		// if (current == clampedUpdatedValue)
		// {
		// 	return;
		// }

		current = clampedUpdatedValue;

		if (onUpdatedAction != null) onUpdatedAction();
		
		if (isEmpty() && onEmptyAction != null)
		{
			onEmptyAction();
		}
		else if (isFull() && onFullAction != null)
		{
			onFullAction();
		}
	}

	internal int Clamped(int updatedValue)
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