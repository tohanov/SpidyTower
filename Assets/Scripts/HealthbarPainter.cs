using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarPainter : MonoBehaviour
{
	private GameObject heartPrefab;
	private PlayerState playerState;

	List<SignleHeartState> heartStates = new List<SignleHeartState>();

	public void DrawHearts() {
		ClearHearts();
	}

	public void CreateEmptyHeart() {
		GameObject newHeart = Instantiate(heartPrefab);
		newHeart.transform.SetParent(transform);

		SignleHeartState heartState = newHeart.GetComponent<SignleHeartState>();
		heartState.SetHeartType(HeartType.Empty);
		heartStates.Add(heartState);
	}

	public void ClearHearts() {
		foreach (Transform childHeart in transform) {
			Destroy(childHeart.gameObject);
		}

		heartStates.Clear();
	}
}
