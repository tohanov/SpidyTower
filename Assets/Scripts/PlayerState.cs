using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;




	
		// spidyActions.Firing.
		// spidyActions.Movement.
		// spidyActions.DropCivilian.



public class PlayerState : MonoBehaviour
{
	[SerializeField] Gradient redSpidyGradient;
	[SerializeField] Gradient blackSpidyGradient;
	static Vector3 projectileSpawnPoint = new Vector3(0.7f, 0.1f);
	internal Stat redHalfHearts;
	internal Stat blackHalfHearts;

	Stat missedCivilians;
	Stat webCartridges;

	GameState gameState;
	
	GenerateBuildings generateBuildingsScript;
	HealthbarPainter healthbarPainterScript;

	Dictionary<DamageSource, int> healthDamage;
	Dictionary<DamageSource, float> speedDamage;
	// float[] spidyVerticalPositions;
	// float[] spidyHorizontalPositions;
	Animator animator;

	[SerializeField] AnimationCurve yJumpCurve;
	[SerializeField] AnimationCurve jumpTimeCurve;
	[SerializeField] AnimationCurve verticalJumpTimeCurve;
	[SerializeField] GameObject webProjectilePrefab;

	[SerializeField] float jumpDuration = 1f;
	[SerializeField] float yJumpMax;
	internal Vector3[,] spidyPositions;
	internal int[] spidyCurrentPosition;
	bool midJump = false;
	TrailRenderer spidyTrail;
	string animationNamePrefix = "";

	string playedLast = "Spidy_climb";

	SpriteRenderer spriteRenderer;
	// internal int invincibilityCoefficient = 1;
	internal bool isInvincible = false;
	Collider2D collider;
	InputDelegator inputDelegator;

	CanvasRenderer healthbarRenderer;
	CanvasRenderer civiliansBarRenderer;
	private int originalWebCartridgesMax = 5;
	private int originalMissedCiviliansMax = 5;

	void Awake()
	{	
		// Time.timeScale = 2;
		collider = GetComponent<Collider2D>();
		animator = GetComponent<Animator>();
		spidyTrail = GetComponent<TrailRenderer>();
		gameState = GetComponentInParent<GameState>();
		// spidyTrail.emitting = false;
		// spidyTrail.enabled = false;
		// spidyTrail.Clear();

		spriteRenderer = GetComponent<SpriteRenderer>();

		healthDamage = new Dictionary<DamageSource, int>() {
			{DamageSource.Bomb, 2},
			{DamageSource.Rubble, 1}
		};
		speedDamage = new Dictionary<DamageSource, float>() {
			{DamageSource.Bomb, 1},
			{DamageSource.Rubble, 0}
		};
		redHalfHearts = new Stat(
			6,
			0,
			6,
			redrawHearts,
			null,
			() => {if (blackHalfHearts.current == 0) {StartCoroutine(damageCauseBlinking(healthbarRenderer)); deathAndGameOver();}}
		);
		blackHalfHearts = new Stat(
			0,
			0,
			int.MaxValue,
			() => {redrawHearts(); updatePlayerStats();},
			null, // updateGameSpeed,
			() => {if (redHalfHearts.current == 0) {StartCoroutine(damageCauseBlinking(healthbarRenderer)); deathAndGameOver();}}
		);
		webCartridges = new Stat(
			originalWebCartridgesMax, //3,
			0,
			originalWebCartridgesMax,
			() => {redrawWebsCounter(); /* enableUnloadingOfCivilianIfHasWebs(); */},
			null/* disableUnloadingOfCivilian */,
			null
		);
		missedCivilians = new Stat(
			0,
			0,
			originalMissedCiviliansMax,
			redrawMissedCiviliansCounter,
			() => { StartCoroutine(damageCauseBlinking(civiliansBarRenderer)); deathAndGameOver();},
			null
		);
		// GetComponent<GameState>();

		updateWholeHUD();
	}

	private void updateWholeHUD()
	{
		// redrawHearts(); // not needed
		redrawWebsCounter();
		redrawMissedCiviliansCounter();
	}

	void Start() {
		var gameController = GameObject.FindGameObjectWithTag("GameController");
		generateBuildingsScript = gameController.GetComponent<GenerateBuildings>();
		inputDelegator = gameController.GetComponent<InputDelegator>();

		healthbarPainterScript = GameObject.FindGameObjectWithTag("HUD/Stats/Healthbar").GetComponent<HealthbarPainter>();

		healthbarRenderer = healthbarPainterScript.gameObject.GetComponent<CanvasRenderer>();
		civiliansBarRenderer = GameObject.FindGameObjectWithTag("Civilians Square").GetComponent<CanvasRenderer>();


		spidyCurrentPosition = new int[] {1, 0};
		populatePositions();
		transform.position = getPositionAsVector3();

		spidyTrail.enabled = true;
		spidyTrail.Clear();
	}

	private void populatePositions()
	{
		spidyPositions = new Vector3[3,3];

		Vector2 boundsHigh = generateBuildingsScript.boundsHigh;
		Vector2 boundsLow = generateBuildingsScript.boundsLow;
		Vector2 blockSize = generateBuildingsScript.blockSize;

		float intervalX = (boundsHigh.x - boundsLow.x)/2f - blockSize.x * 0.5f;
		
		// spidyHorizontalPositions = new float[3]; // according to block sizes and screen corners
		// spidyHorizontalPositions[0] = boundsLow.x + blockSize.x * 0.5f;
		// spidyHorizontalPositions[1] = (boundsHigh.x - boundsLow.x)/2;
		// spidyHorizontalPositions[2] = boundsHigh.x - blockSize.x * 0.5f;


		Vector2 playerSize = gameObject.GetComponent<SpriteRenderer>().size;
		// spidyVerticalPositions = new float[3]; // divide screen to sevenths and use spaces 1-2, 3-4, 5-6
		float intervalY = (boundsHigh.y - boundsLow.y)/7;
		
		// spidyHorizontalPositions[0] = boundsLow.y + intervalY + playerSize.y*0.5f;
		// spidyHorizontalPositions[1] = spidyVerticalPositions[0] + intervalY*2;
		// spidyHorizontalPositions[2] = spidyVerticalPositions[1] + intervalY*2;

		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				spidyPositions[i,j] = new Vector3(
					boundsLow.x + blockSize.x * 0.5f + intervalX * i,
					boundsLow.y + intervalY + playerSize.y*0.5f + intervalY*2*j
				);
				// spidyPositions = new Vector3(
				// 	,
				// 	boundsLow.y + intervalY + playerSize.y * 0.5f,
				// 	0
				// 	);
				// Debug.Log(spidyPositions[i,j]);
			}
		}
	}

	public void takeDamage(DamageSource damageSource)
	{
		if (gameState.gameOverStopper == 0) return;

		// StopCoroutine("damageCauseBlinking");
		StartCoroutine("damageCauseBlinking", healthbarRenderer);

		int damageLeftovers = Mathf.Clamp(healthDamage[damageSource] - blackHalfHearts.current, 0, int.MaxValue);

		blackHalfHearts.updateCurrent(blackHalfHearts.current - healthDamage[damageSource]);

		if (damageLeftovers > 0)
		{
			redHalfHearts.updateCurrent(redHalfHearts.current - damageLeftovers);
		}

		// healthDamage[damageSource]

		
		// gameState.gameSpeed.updateCurrent(gameState.gameSpeed.current - speedDamage[damageSource]); // TODO : might want to make immune to speed loss when have black hearts
	}

	public void collectItem(ItemType itemType)
	{
		switch(itemType) {
			case ItemType.WebCartridge:
			webCartridges.updateCurrent(webCartridges.current + 1);
			break;

			case ItemType.Symbiote:
			blackHalfHearts.updateCurrent(blackHalfHearts.current + 2);
			break;
		}
	}

	
	public void Fire(int x) {
		// Debug.Log("Fired " + (context.ReadValue<float>() < 0 ? "left" : "right"));

		if (spidyCurrentPosition[0] == 0 && x < 0 || spidyCurrentPosition[0] == 2 && x > 0) return;
		if (midJump) return;

		rotatePlayerInDirection(x);

		setAnimation("Spidy_shoot");

		if (webCartridges.isEmpty()) return;
		
		// midJump = true; // hack to block jumping while shooting cause it causes a bug
		inputDelegator.spidyActions.Disable();

		webCartridges.updateCurrent(webCartridges.current - 1);
		gameState.shootingMovementStopper = 0;

		Instantiate(webProjectilePrefab, transform);
	}

	private void setAnimation(string animationName)
	{
		playedLast = animationName;
		animator.Play(animationNamePrefix + animationName);
	}

	public void EndFire(int x) {
		// Debug.Log("Fired " + (context.ReadValue<float>() < 0 ? "left" : "right"));

		
		setAnimation("Spidy_climb");

		gameState.shootingMovementStopper = 1;

		inputDelegator.spidyActions.Enable();
		// midJump = false;
	}

	// internal void Move(InputAction.CallbackContext context)
	// {
	// 	Vector2 instruction = context.ReadValue<Vector2>();
	// 	// if () {
	// 	// 	animator.Play(("Spidy_shoot_left" : "Spidy_shoot_right"));
	// 	// }
	// 	Debug.Log(instruction);
	// }

	void rotatePlayerInDirection(int x) {
		transform.rotation = (x < 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0,0,0);
	}

	internal void ShortJump(int x)
	{
		if (midJump) return;
		
		// midJump = true;
		inputDelegator.spidyActions.Disable();

		int newPositionX = Mathf.Clamp(spidyCurrentPosition[0] + x, 0, 2);

		if (newPositionX == spidyCurrentPosition[0]) return;

		rotatePlayerInDirection(x);

		spidyCurrentPosition[0] = newPositionX;
		StartCoroutine(ShortJumpCoroutine(
			transform.position, 
			getPositionAsVector3()
		));

		// transform.position = spidyPositions[newPositionX, playerCurrentPosition[1]];
		// transform.localPosition = Vector3.zero;//spidyPositions[newPosition, playerCurrentPosition[1]];
		// transform.TransformPoint(Vector2.one);

		// Debug.Log("updated position: " + string.Join(", ", spidyCurrentPosition));
	}

	private IEnumerator ShortJumpCoroutine(Vector3 start, Vector3 end) 
	{
		// midJump = true;
		setAnimation("Spidy_jump_up");
		
		float elapsedTime = 0;

		while (elapsedTime < jumpDuration) {
			elapsedTime += gameState.getOverallSpeed()/gameState.symbioteBoost * Time.fixedDeltaTime;
			float normalizedElapsedTime = elapsedTime / jumpDuration;

			if (elapsedTime / jumpDuration >= 0.7f) setAnimation("Spidy_jump_down");

			float modifiedTime = jumpTimeCurve.Evaluate(normalizedElapsedTime);

			transform.position = new Vector3(
				Mathf.Lerp(start.x, end.x, modifiedTime),
				start.y + yJumpMax * yJumpCurve.Evaluate(modifiedTime)
			);

			// yield return null;
			yield return wffu;
		}

		setAnimation("Spidy_climb");

		inputDelegator.spidyActions.Enable();
		// midJump = false;
	}

	internal void MoveVertically(int y)
	{
		if (midJump) return;
		// midJump = true;
		inputDelegator.spidyActions.Disable();
		
		int newPositionY = Mathf.Clamp(spidyCurrentPosition[1] + y, 0, 2);
		
		if (newPositionY == spidyCurrentPosition[1]) return;
		// animator.Play(y < 0 ? "Spidy_jump_left" : "Spidy_jump_right");
		
		spidyCurrentPosition[1] = newPositionY;

		StartCoroutine(MoveVerticallyCoroutine(
			transform.position, 
			getPositionAsVector3()
		));
		// transform.position = getPositionAsVector3();
		// transform.localPosition = Vector3.zero;//spidyPositions[newPosition, playerCurrentPosition[1]];
		// transform.TransformPoint(Vector2.one);

		// Debug.Log("updated position: " + string.Join(", ", spidyCurrentPosition));
	}

	WaitForFixedUpdate wffu = new WaitForFixedUpdate();

	private IEnumerator MoveVerticallyCoroutine(Vector3 start, Vector3 end) 
	{
		// midJump = true;
		if (end.y < start.y) setAnimation("Spidy_jump_down");
		
		float elapsedTime = 0;

		while (elapsedTime < jumpDuration) {
			elapsedTime += gameState.getOverallSpeed()/gameState.symbioteBoost * Time.fixedDeltaTime;
			float normalizedElapsedTime = elapsedTime / jumpDuration;
			// if (normalizedElapsedTime / jumpDuration >= 0.7f) animator.Play("Spidy_jump_down");

			float modifiedTime = verticalJumpTimeCurve.Evaluate(normalizedElapsedTime);

			transform.position = new Vector3(
				// Mathf.Lerp(start.x, end.x, modifiedTime),
				// start.y + yJumpMax * yJumpCurve.Evaluate(modifiedTime)
				start.x,
				Mathf.Lerp(start.y, end.y, modifiedTime)
			);

			yield return wffu;
		}

		setAnimation("Spidy_climb");

		inputDelegator.spidyActions.Enable();
		// midJump = false;
	}

	private void OnTriggerEnter2D(Collider2D collision) {

		// TODO react accordingly
		// collision.CompareTag("Web Cartridge");
		// collision.CompareTag("Civilian");
		// collision.CompareTag("Symbiote");
		Debug.Log("Spidy Trigger: " + collision.tag);
		if (!isInvincible && collision.CompareTag("Obstacles/Bomb")) {
			// TODO take damage
			invincibilityBlink();
			takeDamage(DamageSource.Bomb);
		}
		else if (heldCivilianStateScript != null && collision.CompareTag("Building Block/Open")) {
			bool succeeded = collision.GetComponent<BuildingBlockState>().tryPutCivilian(heldCivilianStateScript);
			if (succeeded) {
				heldCivilianStateScript = null;
				restoreCivilianHoldingSpeed();
			}
		}
		else if (collision.CompareTag("Collectables/Web Cartridge")) {
			collectItem(ItemType.WebCartridge);
			Destroy(collision.gameObject);
		}
		else if (collision.CompareTag("Collectables/Symbiote")) {
			collectItem(ItemType.Symbiote);
			Destroy(collision.gameObject);

			// TODO collect
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		// private void OnCollisionEnter2D(Collider2D collision) {

		// TODO react accordingly
		// collision.CompareTag("Web Cartridge");
		// collision.CompareTag("Civilian");
		// collision.CompareTag("Symbiote");
		Debug.Log("Spidy Collider: " + collision.gameObject.tag);
		if (!isInvincible && collision.gameObject.CompareTag("Obstacles/Rubble")) {
			invincibilityBlink();
			takeDamage(DamageSource.Rubble);
		}
	}

	private void redrawMissedCiviliansCounter()
	{
		gameState.updateMissedCivilians(missedCivilians.current, missedCivilians.max);
	}

	private void redrawHearts()
	{
		healthbarPainterScript.UpdateHearts();
	}

	private void deathAndGameOver()
	{	
		// if (gameState.gameOverStopper == 0) return;

		Debug.Log("deathAndGameOver() after the if");
		gameState.GetComponent<InputDelegator>().disableSpidyInput();
		gameState.gameOverStopper = 0;
		// inputDelegator.disableSpidyInput();

		spidyTrail.emitting = false;
		spidyTrail.enabled = false;
		spidyTrail.Clear();

		gameState.gameSpeed.updateCurrent(1);
		// StopAllCoroutines();
		
		Vector3 target = new Vector3(transform.position.x, gameState.boundsLow.y - generateBuildingsScript.blockSize.y);
		// Vector3 source = transform.position;

		StartCoroutine(deathFallCoroutine(transform.position, target));
		Debug.DrawLine(target, transform.position, Color.red, 2);

		// Destroy(gameObject);
	}

	private IEnumerator deathFallCoroutine(Vector3 source, Vector3 target) 
	{
		collider.enabled = false;

		setAnimation("Spidy_jump_down");
		float elapsedTime = 0;

		while (elapsedTime < 3) {
			elapsedTime += 0.5f * Time.deltaTime;
			transform.position = Vector3.Lerp(source, target, elapsedTime);
			yield return null;
		}

		Time.timeScale = 0;
		// gameState.gameSpeed.updateCurrent(0);
		// Time.timeScale = 0;
		gameState.gameOver();
	}

	IEnumerator damageCauseBlinking(CanvasRenderer renderer) {
		float timeElapsed = 0;

		while (gameState.gameOverStopper == 0 || timeElapsed < damageInvincibiltyDuration) {

			renderer.SetAlpha(blinkingOpacity);
			//.enabled = false;
			yield return new WaitForSeconds(blinkingInterval);
			
			renderer.SetAlpha(1);
			// GetComponent<Collider2D>().isTrigger = true;
			// spriteRenderer.enabled = true;
			yield return new WaitForSeconds(blinkingInterval);
			if (gameState.gameOverStopper != 0) timeElapsed += 2*blinkingInterval;
		}
	}


	internal CivilianState heldCivilianStateScript = null;
	internal void catchCivilian(CivilianState state) {
		heldCivilianStateScript = state;

		gameState.civilianHoldingCoefficient = 0.70f;
	}

	internal void dropCivilian()
	{
		if (midJump || webCartridges.isEmpty()) return;

		// heldCivilianStateScript.setState(CivilianState.State.Bound);
		heldCivilianStateScript.animator.Play("Civilian_bound");
		// heldCivilianStateScript.rigidBody.velocity = CivilianState.velocity;
		// heldCivilianStateScript.rigidBody.Sleep();
		heldCivilianStateScript.gameObject.AddComponent<MoveDownwards>();
		heldCivilianStateScript.transform.parent = null;
		heldCivilianStateScript = null;

		webCartridges.updateCurrent(webCartridges.current - 1);

		restoreCivilianHoldingSpeed();
	}

	internal void restoreCivilianHoldingSpeed() {
		gameState.civilianHoldingCoefficient = 1;
	}


	private void updateGameSpeed()
	{
		// throw new NotImplementedException();
		Debug.Log("updateGameSpeed: NOT IMPLEMENTED");
	}

	private void updatePlayerStats()
	{
		// TODO update speed, web cartridges max, missed people max
		// throw new NotImplementedException();

		// Debug.Log("updatePlayerStats: NOT IMPLEMENTED");

		missedCivilians.max = originalMissedCiviliansMax + blackHalfHearts.current / 2;
		webCartridges.max = originalWebCartridgesMax + 3 * blackHalfHearts.current / 2;
		forceStatsRefresh();
		
		gameState.symbioteBoost = 1 + /*Mathf.CeilToInt(*/blackHalfHearts.current / 4.0f/*)*/; // keep int

		if ( ! blackHalfHearts.isEmpty()) {
			animationNamePrefix = "Black";
			animator.PlayInFixedTime(animationNamePrefix + playedLast, -1, animator.playbackTime);
			
			spidyTrail.colorGradient = blackSpidyGradient;
			// set current animation to black
			// set future animations to black
			// increase stats // TODO
		}
		else {
			animationNamePrefix = "";
			// spidyTrail.colorGradient.colorKeys[0].color = Color.black; // Color.red;
			animator.PlayInFixedTime(animationNamePrefix + playedLast, -1, animator.playbackTime);
			
			spidyTrail.colorGradient = redSpidyGradient;
			// set current animation to red
			// set future animations to red
			// decrease stats // TODO
		}
	}

	void forceStatsRefresh() {
		// missedCivilians.onUpdatedAction();
		missedCivilians.updateCurrent(missedCivilians.current);
		webCartridges.updateCurrent(webCartridges.current);
		// webCartridges.onUpdatedAction();
	}

	// private void redrawBlackHearts()
	// {
	// 	throw new NotImplementedException();
	// }

	// private void enableUnloadingOfCivilianIfHasWebs()
	// {
	// 	// throw new NotImplementedException();
	// }

	private void redrawWebsCounter()
	{
		gameState.updateAvailableWebCartridges(webCartridges.current, webCartridges.max);
	}

	// private void disableUnloadingOfCivilian()
	// {
	// 	throw new NotImplementedException();
	// }

	[SerializeField] float blinkingInterval = 0.15f;
	[SerializeField] float damageInvincibiltyDuration = 2.5f; 
	[SerializeField] static float blinkingOpacity = 0.25f; 
	static Color transparentWhite = new Color(1,1,1,blinkingOpacity);

	[ContextMenu("Test Blinking")]
	void invincibilityBlink() {
		StartCoroutine(damageAnimation());
	}

	IEnumerator damageAnimation() {
		float timeElapsed = 0;
		
		toggleInvincibility();

		while (timeElapsed < damageInvincibiltyDuration) {
			spriteRenderer.color = transparentWhite;
			//.enabled = false;
			yield return new WaitForSeconds(blinkingInterval);
			spriteRenderer.color = Color.white;
			// GetComponent<Collider2D>().isTrigger = true;
			// spriteRenderer.enabled = true;
			yield return new WaitForSeconds(blinkingInterval);
			timeElapsed += 2*blinkingInterval;
			collider.isTrigger = true;
		}
		
		collider.isTrigger = false;
		toggleInvincibility();
	}

	void toggleInvincibility() {
		// invincibilityCoefficient = 1 - invincibilityCoefficient;
		isInvincible = !isInvincible;
		// GetComponent<Collider2D>().isTrigger = !GetComponent<Collider2D>().isTrigger;
	}

	internal Vector3 getPositionAsVector3() {
		return spidyPositions[
			spidyCurrentPosition[0], 
			spidyCurrentPosition[1]
		];
	}

	internal void incrementMissedCivilians()
	{
		if (gameState.gameOverStopper == 0) return;

		// StopCoroutine("damageCauseBlinking");
		StartCoroutine("damageCauseBlinking", civiliansBarRenderer);
		missedCivilians.updateCurrent(missedCivilians.current + 1);
	}

	// bool isInvincible() {
	// 	return invincibilityCoefficient == 0;
	// }
}
