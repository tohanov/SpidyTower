using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
	[SerializeField] float duration = 1f;
	Vector3 startPosition;
	[SerializeField] AnimationCurve curve;
	void Start() {
		// performShake();
		startPosition = transform.position;
	}
	public void performShake() {
		Debug.Log("performshake");
		StopCoroutine("coroutineShake");
		StartCoroutine("coroutineShake");
	}

	IEnumerator coroutineShake() {
		Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			float strength = curve.Evaluate(elapsedTime / duration);
			transform.position = startPosition + Random.insideUnitSphere/*Random.insideUnitSphere*/ * strength;
			yield return null;
		}

		transform.position = startPosition;
	}
}
