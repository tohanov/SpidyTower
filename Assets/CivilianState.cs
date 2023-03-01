using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianState : MonoBehaviour
{

	Animator animator;

	internal enum State {
		Falling,
		Bound,
		Hanging,
	}

	internal State state = State.Falling;
	Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
		animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
		rigidbody.velocity = Vector2.down * 3;
    }


	internal void setState(State s) {
		state = s;

		switch (state) {
			case State.Falling:
			animator.Play("Civilian_falling");
			break;
			case State.Hanging:
			animator.Play("Civilian_hanging");
			break;
			case State.Bound:
			animator.Play("Civilian_bound");
			break;
		}
	}


	void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log("Civilian collision: " + collision.tag);

		if (state == State.Bound && (collision.CompareTag("Building Block/Closed") || collision.CompareTag("Building Block/Open"))) {
			rigidbody.velocity = Vector2.zero;
			transform.parent = collision.transform;
		}
		else if (state == State.Falling) {
			if (collision.CompareTag("Web Projectile")) {
				rigidbody.velocity = collision.gameObject.GetComponent<WebProjectileState>().velocity;
				animator.Play("Civilian_bound");
				state = State.Bound;
				Destroy(collision.gameObject);
			}
			else if (collision.CompareTag("Spidy")) {
				PlayerState playerState = collision.GetComponent<PlayerState>();
				if (playerState.heldCivilianStateScript != null) return;

				playerState.heldCivilianStateScript = this;

				animator.Play("Civilian_hanging");
				rigidbody.velocity = Vector2.zero;
				transform.parent = collision.transform;
				transform.rotation = Quaternion.identity;
				transform.parent = collision.transform;
				transform.localPosition = Vector3.zero;
			}
		}
	}


}
