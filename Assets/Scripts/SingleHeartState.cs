using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using System.Collections.Generic;

public class SignleHeartState : MonoBehaviour
{
	[SerializeField] private Sprite emptyHeart, halfHeart, fullHeart;
	private Sprite[] heartStates;
	Image heartImage;
	// Dictionary<HeartStatus, Sprite> spriteMap;

	private void Awake()
	{
		heartImage = GetComponent<Image>();
		heartStates = new Sprite[] {
			emptyHeart, halfHeart, fullHeart,};
		// spriteMap = new Dictionary<HeartStatus, Sprite>();
	}

	public void SetHeartType(HeartType status)
	{
		// heartImage.sprite = 
		heartImage.sprite = heartStates[(int)status];
	}

}

public enum HeartType
{
	Empty = 0,
	Half,
	Full,
}