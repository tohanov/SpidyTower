using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util {
	public static T randomElement<T>(T[] a) {
		return a[Random.Range(0, a.Length*10) / 10];
	}


	public static bool trueWithProbability(float a) {
		return a > Random.Range(0f, 1000f)/1000f;
	}
}