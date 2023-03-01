using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianState : MonoBehaviour
{

	internal Animator animator;

	internal enum State {
		Falling,
		Bound,
		Hanging,
	}

	internal State state = State.Falling;
	internal Rigidbody2D rigidBody;
    // Start is called before the first frame update

	internal static Vector2 velocity = Vector2.down * 3;

    void Start()
    {
		animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.velocity = velocity;
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
			rigidBody.velocity = Vector2.zero;
			transform.parent = collision.transform;
		}
		else if (state == State.Falling) {
			if (collision.CompareTag("Web Projectile")) {
				rigidBody.velocity = collision.gameObject.GetComponent<WebProjectileState>().velocity;
				animator.Play("Civilian_bound");
				state = State.Bound;
				Destroy(collision.gameObject);
			}
			else if (collision.CompareTag("Spidy")) {
				PlayerState playerState = collision.GetComponent<PlayerState>();

				if (playerState.heldCivilianStateScript != null) return;

				playerState.catchCivilian(this);

				setState(State.Hanging);
				rigidBody.velocity = Vector2.zero;
				transform.parent = collision.transform;
				transform.rotation = Quaternion.identity;
				transform.parent = collision.transform;
				transform.localPosition = Vector3.zero;
			}
		}
	}


}
