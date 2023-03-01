using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockState : MonoBehaviour
{
	[SerializeField] Sprite withCivilian;
	SpriteRenderer spriteRenderer;
	bool holdingCivilian = false;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	internal bool tryPutCivilian(CivilianState civilianStateScript) {
		if (holdingCivilian) return false;
		
		holdingCivilian = true;
		Destroy(civilianStateScript.gameObject);
		spriteRenderer.sprite = withCivilian;

		return true;
	}
}
