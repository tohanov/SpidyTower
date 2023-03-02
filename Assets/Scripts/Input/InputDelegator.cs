using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputDelegator : MonoBehaviour
{
	InputDetector inputDetectorObject;

	internal InputDetector.InGameActionMapActions inGameActions;
	internal InputDetector.InstructionsScreenActionMapActions instructionsScreenActions;
	internal InputDetector.SpidyActionMapActions spidyActions;
	internal InputDetector.DeathScreenActionMapActions deathScreenActions;

	PlayerState playerStateScript;
	GameState gameState;

	void Awake() {
		inputDetectorObject = new InputDetector();

		inGameActions = inputDetectorObject.InGameActionMap;
		instructionsScreenActions = inputDetectorObject.InstructionsScreenActionMap;
		spidyActions = inputDetectorObject.SpidyActionMap;
		deathScreenActions = inputDetectorObject.DeathScreenActionMap;

		gameState = GetComponent<GameState>();
		playerStateScript = GameObject.FindGameObjectWithTag("Spidy").GetComponent<PlayerState>();
	}

	void OnEnable() {
		inGameActions.Enable();
		instructionsScreenActions.Disable(); // FIXME make enabled when displaying the instructions scene
		spidyActions.Enable();
		deathScreenActions.Disable();

		inGameActions.GamePause.performed += HandleGamePause;

		spidyActions.Firing.performed += HandlePlayerFire;
		spidyActions.Movement.performed += HandlePlayerMovement;
		spidyActions.DropCivilian.performed += HandleDropCivilian;


		instructionsScreenActions.Skip.performed += HandleSkip;
		instructionsScreenActions.Pause.performed += HandleInstructionsPause;

		deathScreenActions.PlayAgain.performed += HandlePlayAgain;
		deathScreenActions.Exit.performed += HandleExit;
	}

	private void HandlePlayAgain(InputAction.CallbackContext obj)
	{
		// SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Time.timeScale = 1;
	}

	private void HandleExit(InputAction.CallbackContext obj)
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}


	void OnDisable() {
		inGameActions.Disable();
		// instructionsScreenActions.Disable();
		spidyActions.Disable();
		// deathScreenActions.Disable();
	} 

    void HandlePlayerFire(InputAction.CallbackContext context)
    {
        playerStateScript.Fire((int)context.ReadValue<float>());
    }

	void HandlePlayerMovement(InputAction.CallbackContext context)
    {
		Vector2 movement = context.ReadValue<Vector2>();
		// Debug.Log("movement delegation" + movement);

		if (movement.y != 0) {
			playerStateScript.MoveVertically((int)movement.y);
		}
		else if (movement.x != 0) {
        	playerStateScript.ShortJump((int)movement.x);
		}
    }

	void HandleDropCivilian(InputAction.CallbackContext context) {
		playerStateScript.dropCivilian();
	}

	void HandleGamePause(InputAction.CallbackContext context)
    {
		if (spidyActions.enabled) spidyActions.Disable();
		else spidyActions.Enable();

        gameState.TogglePauseGame(context);
    }

	private void HandleInstructionsPause(InputAction.CallbackContext obj)
	{
		throw new NotImplementedException();
	}

	private void HandleSkip(InputAction.CallbackContext obj)
	{
		throw new NotImplementedException();
	}

	internal void disableSpidyInput()
	{
		spidyActions.Disable();
	}
}
