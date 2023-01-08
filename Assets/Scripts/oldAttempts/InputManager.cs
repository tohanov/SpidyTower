using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
	private TouchControls touchControls; //= new TouchControls();

	private void Awake() {
		Debug.Log("Awake()");
		touchControls = new TouchControls();
		touchControls.Touch.Enable();
	}

	// private void OnEnable() {
	// 	Debug.Log("Awake()");
	// 	touchControls.Enable();
	// }

	// private void OnDisable() {
	// 	touchControls.Disable();
	// }

	// Start is called before the first frame update
	void Start() {
		Debug.Log("Started");
		// touch
		touchControls.Touch.TouchPress.started += context => StartTouch(context);
		touchControls.Touch.TouchPress.canceled += (context => EndTouch(context));
	}

	// Update is called once per frame
	void Update() {
		
	}

	private void StartTouch(InputAction.CallbackContext context) {
		Debug.Log("StartTouch");
		Debug.Log("Touch started: " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
	}

	private void EndTouch(InputAction.CallbackContext context) {
		Debug.Log("Touch ended: " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
	}
}
