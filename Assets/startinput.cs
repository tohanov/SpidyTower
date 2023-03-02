using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class startinput : MonoBehaviour
{
    // Start is called before the first frame update
	InputDetector inputDetectorObject;

	internal InputDetector.InGameActionMapActions inGameActions;
	internal InputDetector.InstructionsScreenActionMapActions instructionsScreenActions;
	internal InputDetector.SpidyActionMapActions spidyActions;
	internal InputDetector.DeathScreenActionMapActions deathScreenActions;

	void OnEnable() {
		inputDetectorObject.Enable();
		inputDetectorObject.InstructionsScreenActionMap.Enable();
		inputDetectorObject.SpidyActionMap.Disable();
		inputDetectorObject.InGameActionMap.Disable();
		inputDetectorObject.DeathScreenActionMap.Disable();
		// inputDetectorObject.action.Disable();
	}
    void Awake()
    {
        inputDetectorObject = new InputDetector();
		inputDetectorObject.InstructionsScreenActionMap.Enable();
		inputDetectorObject.SpidyActionMap.Disable();
		inputDetectorObject.InGameActionMap.Disable();
		inputDetectorObject.DeathScreenActionMap.Disable();
		// inputDetectorObject.Disable();

		// inputDetectorObject.InstructionsScreenActionMap.Enable();

		inputDetectorObject.InstructionsScreenActionMap.Play.performed += HandlePlay;
		inputDetectorObject.InstructionsScreenActionMap.Quit.performed += HandleQuit;
    }

	void OnDisable() {
		inputDetectorObject.Disable();
	}

	private void HandlePlay(InputAction.CallbackContext obj)
	{
		SceneManager.LoadScene("inGameScene");
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		// Time.timeScale = 1;
	}

	private void HandleQuit(InputAction.CallbackContext obj)
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
