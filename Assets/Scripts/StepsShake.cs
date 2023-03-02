using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepsShake : MonoBehaviour
{
	[SerializeField] float duration = 1f;
	Vector3 startPosition;
	[SerializeField] AnimationCurve curve;
	internal TextMeshProUGUI textMeshProuGui;
	float biggerFont;
	float smallerFont;

	void Start() {

		// performShake();
		startPosition = transform.position;

		biggerFont = textMeshProuGui.fontSize + 10;
		smallerFont = textMeshProuGui.fontSize;
	}
	public void performShake() {
		Debug.Log("performshake");
		StopCoroutine("coroutineShake");
		StartCoroutine("coroutineShake");
	}

	IEnumerator coroutineShake() {
		Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

		textMeshProuGui.fontSize = biggerFont;

		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			float strength = curve.Evaluate(elapsedTime / duration);
			transform.position = startPosition + Random.insideUnitSphere/*Random.insideUnitSphere*/ * strength;
			yield return null;
		}
		
		textMeshProuGui.fontSize = smallerFont;

		transform.position = startPosition;
	}
}
