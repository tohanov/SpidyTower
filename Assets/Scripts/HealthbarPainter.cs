using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarPainter : MonoBehaviour
{
	[SerializeField] GameObject redHeartPrefab;
	[SerializeField] GameObject blackHeartPrefab;
	// internal PlayerState playerState;
	SingleHeartState[] redHeartStates = new SingleHeartState[3];
	List<SingleHeartState> blackHeartStates = new List<SingleHeartState>();
	// List<SingleHeartState> heartStates = new List<SingleHeartState>();
	PlayerState playerStateScript;

	void Awake() {
		// instantiate 3 heart prefabs
		// set their states to full (should be automatic)
			// each heart should have a state that sets it's sprite accordingly
		for (int i = 0; i < 3; ++i) {
			var heart = Instantiate(redHeartPrefab, transform);
			redHeartStates[i] = heart.GetComponent<SingleHeartState>();
		}
	}

	void Start() {
		playerStateScript = GameObject.FindGameObjectWithTag("Spidy").GetComponent<PlayerState>();
	}

	internal void UpdateHearts(/* int redHalfs, int blackHalfs */) {
		int redHalfs = (int)playerStateScript.redHalfHearts.current;
		int blackHalfs = (int)playerStateScript.blackHalfHearts.current;

		Debug.Log("UPDATE HEARTS: redHalfs=" + redHalfs + " blackHalfs=" + blackHalfs);

		int fullRedHeartsNumber = redHalfs / 2;

		for (int i = 0; i < fullRedHeartsNumber; ++i) {
			redHeartStates[i].setState(HeartStates.Full);
		}

		if (fullRedHeartsNumber * 2 != redHalfs) redHeartStates[fullRedHeartsNumber].setState(HeartStates.Half);
		else if (fullRedHeartsNumber < 3) redHeartStates[fullRedHeartsNumber].setState(HeartStates.Empty);

		for (int i = fullRedHeartsNumber + 1; i < 3; ++i) {
			redHeartStates[i].setState(HeartStates.Empty);
		}


		// SingleHeartState lastVisited;
		int fullBlackHeartsNumber = blackHalfs / 2;
		clearBlackHearts();

		for (int i = 1; i <= fullBlackHeartsNumber; ++i) {
			blackHeartStates.Add(Instantiate(blackHeartPrefab, transform).GetComponent<SingleHeartState>());
		}
		
		if (fullBlackHeartsNumber * 2 != blackHalfs) {
			var shs = Instantiate(blackHeartPrefab, transform).GetComponent<SingleHeartState>();
			blackHeartStates.Add(shs);
			shs.setState(HeartStates.Half);
		}
	}

	// internal void DrawHearts() {
	// 	// ClearHearts();
	// }

	// public void CreateEmptyHeart() {
	// 	GameObject newHeart = Instantiate(heartPrefab);
	// 	newHeart.transform.SetParent(transform);

	// 	SignleHeartState heartState = newHeart.GetComponent<SignleHeartState>();
	// 	heartState.SetHeartType(HeartType.Empty);
	// 	heartStates.Add(heartState);
	// }

	public void clearBlackHearts() {
		foreach (SingleHeartState shs in blackHeartStates) {
			Destroy(shs.gameObject);
		}

		blackHeartStates.Clear();
	}
}
