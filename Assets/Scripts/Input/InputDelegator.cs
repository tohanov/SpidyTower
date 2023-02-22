using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDelegator : MonoBehaviour
{

	InputDetector inputDetectorObject;

	InputDetector.InGameActionMapActions inGameActions;
	InputDetector.InstructionsScreenActionMapActions instructionsScreenActions;
	InputDetector.SpidyActionMapActions spidyActions;

	PlayerState playerStateScript;
	GameState gameState;

	void Awake() {
		inputDetectorObject = new InputDetector();

		inGameActions = inputDetectorObject.InGameActionMap;
		instructionsScreenActions = inputDetectorObject.InstructionsScreenActionMap;
		spidyActions = inputDetectorObject.SpidyActionMap;

		gameState = GetComponent<GameState>();
		playerStateScript = GameObject.FindGameObjectWithTag("Spidy").GetComponent<PlayerState>();
	}

	void OnEnable() {
		inGameActions.Enable();
		// instructionsScreenActions.Enable(); // FIXME make enabled when displaying the instructions scene
		spidyActions.Enable();

		inGameActions.GamePause.performed += HandleGamePause;

		spidyActions.Firing.performed += HandlePlayerFire;
		spidyActions.Movement.performed += HandlePlayerMovement;

		instructionsScreenActions.Skip.performed += HandleSkip;
		instructionsScreenActions.Pause.performed += HandleInstructionsPause;
	}

	void OnDisable() {
		inGameActions.Disable();
		instructionsScreenActions.Disable();
		spidyActions.Disable();
	} 

    void HandlePlayerFire(InputAction.CallbackContext context)
    {
        playerStateScript.Fire(context);
    }

	void HandlePlayerMovement(InputAction.CallbackContext context)
    {
		Debug.Log("movement delegation");
        playerStateScript.Move(context);
    }

	void HandleGamePause(InputAction.CallbackContext context)
    {
        gameState.PauseGame(context);
    }

	private void HandleInstructionsPause(InputAction.CallbackContext obj)
	{
		throw new NotImplementedException();
	}

	private void HandleSkip(InputAction.CallbackContext obj)
	{
		throw new NotImplementedException();
	}
}
