using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianState : MonoBehaviour
{

	Animator animator;

	enum State {
		Falling,
		Bound,
		Hanging,
	}

	State state = State.Falling;
	Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
		rigidbody.velocity = Vector2.down * 3;
    }


	void setState(State s) {
		
	}


	void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log("Civilian collision: " + collision.tag);

		if (collision.CompareTag("Building Block") && state == State.Bound) {
			rigidbody.velocity = Vector2.zero;
			transform.parent = collision.transform;
		}
		else if (collision.CompareTag("Web Projectile")) {
			rigidbody.velocity = collision.gameObject.GetComponent<WebProjectileState>().velocity;
			animator.Play("Civilian_bound");
			state = State.Bound;
			Destroy(collision.gameObject);
		}
	}


}
