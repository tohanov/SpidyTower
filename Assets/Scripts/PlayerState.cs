using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;

public class PlayerState : MonoBehaviour
{

	#region definitions
	enum Tower { Left, Middle, Right }
	enum JumpDirection { Left, Right };
	enum GameState { Ongoing, Over }
	enum OperationResult { Success, Failure }
	enum MeasureState { Empty, Full, InBetween }

	class Measure
	{
		protected int currentValue;
		protected int capacity;
		protected MeasureState measureState;

		private Func<dynamic> callbackOnFull;
		private Func<dynamic> callbackOnEmpty;

		public Measure(int startValue, int capacity, Func<dynamic> callbackOnFull, Func<dynamic> callbackOnEmpty)
		{
			this.currentValue = startValue;
			this.capacity = capacity;

			this.callbackOnFull = callbackOnFull;
			this.callbackOnEmpty = callbackOnEmpty;

			updateMeasureState();
		}

		private void updateMeasureState()
		{
			measureState =
				(currentValue == capacity) ? MeasureState.Full :
				(currentValue == 0) ? MeasureState.Empty :
				MeasureState.InBetween;

			useCallbacks();
		}

		private void useCallbacks()
		{
			if (measureState == MeasureState.Full && callbackOnFull != null)
			{
				callbackOnFull();
			}
			else if (measureState == MeasureState.Empty && callbackOnEmpty != null)
			{
				callbackOnEmpty();
			}
		}

		public void setToFull()
		{
			currentValue = capacity;
		}

		public void setToEmpty()
		{
			currentValue = 0;
		}

		public OperationResult increment()
		{
			if (measureState == MeasureState.Full)
			{
				return OperationResult.Failure;
			}

			currentValue += 1;
			updateMeasureState();

			return OperationResult.Success;
		}

		public OperationResult decrement()
		{
			if (measureState == MeasureState.Empty)
			{
				return OperationResult.Failure;
			}

			currentValue += 1;
			updateMeasureState();

			return OperationResult.Success;
		}

		public void updateCapacity(int newCapacity)
		{
			capacity = newCapacity;
		}
	}
	#endregion

	private UserInputActions userInputs;
	private Camera mainCamera;

	private static int LIVES = 3;
	Measure halflives = new Measure(LIVES * 2, LIVES * 2, null, null);
	Measure webUnits = new Measure(5, 5, null, null);
	Measure consecutivePeopleMisses = new Measure(0, 5, null, null);

	private Tower horizontalPosition;
	private double verticalPosition;
	private double currentSpeed;
	private bool isMidAir;

	Action doNothing = () => { Debug.Log("Not supposed to do anything :)"); };
	Action[] jumpActions;

	bool registeredSwipe;

	public void OnEnable()
	{
		userInputs.PlayerActions.Enable();
	}

	public void OnDisable()
	{
		userInputs.PlayerActions.Disable();
	}

	public void Awake()
	{

#if UNITY_EDITOR
		Debug.unityLogger.logEnabled = true;
#else
		Debug.unityLogger.logEnabled = false;
#endif

		Debug.Log("awake()");

		userInputs = new UserInputActions();
		mainCamera = Camera.main;

		horizontalPosition = Tower.Middle;

		jumpActions = new Action[] {
			doNothing,
			() => {
				// switch tower state to left
				Animate(Tower.Left);
				horizontalPosition = Tower.Left;
			},
			() => {
				// switch tower state to middle
				Animate(Tower.Middle);
				horizontalPosition = Tower.Middle;
			},
			() => {
				// switch tower state to right
				Animate(Tower.Right);
				Debug.Log("inside right function");
				horizontalPosition = Tower.Right;
				Debug.Log(horizontalPosition);
			},
			doNothing,
		};
	}

	private void Start()
	{
		userInputs.PlayerActions.PrimaryTouchContact.started += OnDetectedTouchContact;
		userInputs.PlayerActions.PrimaryTouchValue.performed += OnTouchValuesUpdated;
	}

	private void Animate(Tower destination)
	{
		isMidAir = true;

		transform.position += new Vector3(1, 0, 0) * (destination - horizontalPosition);

		isMidAir = false;
	}

	private void ShortJump(TouchState wholeTouch)
	{
		//isMidAir = true;

		// get delta along horizontal axis
		Vector3 delta = screenToWorldPoint(wholeTouch.position) - screenToWorldPoint(wholeTouch.startPosition);

		float deltaX = delta.x;

		// get jump direction from delta0
		int direction = Math.Sign(deltaX);

		Debug.Log(
			$"direction: {direction} ::: " + 
			$"new tower: {(Tower)((int)horizontalPosition + direction)} ::: " +
			$"function: {((int)horizontalPosition + direction)}"
		);

		// TODO : perform jump with animation and state change
		int functionIndex = ((int)horizontalPosition + direction) + 1;

		Action jumpAction = jumpActions[functionIndex];

		jumpAction.Invoke();

		Debug.Log($"new horizontal position: {horizontalPosition}");

		// unblock movement
		//isMidAir = false;
	}

	public void OnDetectedTouchContact(InputAction.CallbackContext context)
	{
		registeredSwipe = false;

		Debug.Log("started");
	}

	private void OnTouchValuesUpdated(InputAction.CallbackContext context)
	{
		TouchState touchInfo = context.ReadValue<TouchState>();

		if (registeredSwipe || touchInfo.isTap) return;

		double overallDeltaX = Mathf.Abs(touchInfo.position.x - touchInfo.startPosition.x);

		Debug.Log($"updated");
		//Debug.Log(touchInfo.position.x);
		//Debug.Log(touchInfo.startPosition.x);

		//Debug.Log($"updated: {overallDeltaX}");

		if (!registeredSwipe && overallDeltaX > 70)
		{
			Debug.Log($"registered swipe length: {overallDeltaX}");
			registeredSwipe = true;

			Debug.DrawLine(
				start: screenToWorldPoint(touchInfo.startPosition),
				end: screenToWorldPoint(touchInfo.position),
				color: Color.red,
				duration: 1
			);

			ShortJump(touchInfo);
		}
	}

	Vector3 screenToWorldPoint(Vector2 screenPoint)
	{
		Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
		worldPoint.z = mainCamera.nearClipPlane;

		return worldPoint;
	}
}