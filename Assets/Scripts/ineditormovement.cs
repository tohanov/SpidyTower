using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ineditormovement : MonoBehaviour
{
	Vector3 startPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        // startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up/10;
		if (transform.position.y > 5)
			transform.position = startPosition;

        transform.position += Vector3.right/10;
		if (transform.position.x > 5)
			transform.position = startPosition;

		Renderer renderer = GetComponentInChildren<Renderer>();
		Material mat = renderer.material;
		
		float emission = Mathf.PingPong(Time.time, 1.0f);
		Color baseColor = Color.yellow; //Replace this with whatever you want for your base color at emission level '1'
		
		Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
		
		mat.SetColor("_EmissionColor", finalColor);
    }
}
