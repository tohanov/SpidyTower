using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using System.Collections.Generic;

public class SingleHeartState : MonoBehaviour
{
	[SerializeField] private Sprite emptyHeartSprite, halfHeartSprite, fullHeartSprite;
	// private Sprite[] heartStates;
	Image heartImage;
	// Dictionary<HeartStatus, Sprite> spriteMap;

	private void Awake()
	{
		heartImage = GetComponent<Image>();
		setState(HeartStates.Full);
		// heartStates = new Sprite[] {
		// 	emptyHeart, halfHeart, fullHeart,};
		// spriteMap = new Dictionary<HeartStatus, Sprite>();
	}

	// public void SetHeartType(HeartType status)
	// {
	// 	// heartImage.sprite = 
	// 	heartImage.sprite = heartStates[(int)status];
	// }

	internal void setState(HeartStates heartState)
	{
		switch(heartState) {
			case HeartStates.Empty:
				setSprite(emptyHeartSprite);
			break;
			case HeartStates.Half:
				setSprite(halfHeartSprite);
			break;
			case HeartStates.Full:
				setSprite(fullHeartSprite);
			break;
		}
	}

	void setSprite(Sprite sprite) {
		heartImage.sprite = sprite;
	}
}

public enum HeartStates
{
	Empty = 0,
	Half,
	Full,
}