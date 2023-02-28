using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PlayerState : MonoBehaviour
{
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

	[SerializeField] float jumpDuration = 1f;
	[SerializeField] float yJumpMax;
	internal Vector3[,] spidyPositions;
	internal int[] spidyCurrentPosition;
	bool midJump = false;
	TrailRenderer spidyTrail;

	SpriteRenderer spriteRenderer;
	// internal int invincibilityCoefficient = 1;
	internal bool isInvincible = false;

	void Awake()
	{	
		// Time.timeScale = 2;

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
			deathAndGameOver,
			null
		);
		blackHalfHearts = new Stat(
			0,
			0,
			int.MaxValue,
			() => {redrawHearts(); updatePlayerStats();},
			updateGameSpeed,
			null
		);
		webCartridges = new Stat(
			3,
			0,
			5,
			() => {redrawWebsCounter(); enableUnloadingOfCivilianIfHasWebs();},
			disableUnloadingOfCivilian,
			null
		);
		missedCivilians = new Stat(
			0,
			0,
			5,
			redrawMissedCiviliansCounter,
			null,
			failAndEndGame
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
		generateBuildingsScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GenerateBuildings>();
		healthbarPainterScript = GameObject.FindGameObjectWithTag("HUD/Stats/Healthbar").GetComponent<HealthbarPainter>();

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
		
		rotatePlayerInDirection(x);
		
		animator.Play("Spidy_shoot");
	}

	public void EndFire(int x) {
		// Debug.Log("Fired " + (context.ReadValue<float>() < 0 ? "left" : "right"));

		animator.Play("Spidy_climb");
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
		midJump = true;
		animator.Play("Spidy_jump_up");
		
		float elapsedTime = 0;

		while (elapsedTime < jumpDuration) {
			elapsedTime += Time.deltaTime;
			float normalizedElapsedTime = elapsedTime / jumpDuration;

			if (elapsedTime / jumpDuration >= 0.7f) animator.Play("Spidy_jump_down");

			float modifiedTime = jumpTimeCurve.Evaluate(normalizedElapsedTime);

			transform.position = new Vector3(
				Mathf.Lerp(start.x, end.x, modifiedTime),
				start.y + yJumpMax * yJumpCurve.Evaluate(modifiedTime)
			);

			yield return null;
		}

		animator.Play("Spidy_climb");
		midJump = false;
	}

	internal void MoveVertically(int y)
	{
		if (midJump) return;
		
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

	private IEnumerator MoveVerticallyCoroutine(Vector3 start, Vector3 end) 
	{
		midJump = true;
		if (end.y < start.y) animator.Play("Spidy_jump_down");
		
		float elapsedTime = 0;

		while (elapsedTime < jumpDuration) {
			elapsedTime += Time.deltaTime;
			float normalizedElapsedTime = elapsedTime / jumpDuration;
			// if (normalizedElapsedTime / jumpDuration >= 0.7f) animator.Play("Spidy_jump_down");

			float modifiedTime = verticalJumpTimeCurve.Evaluate(normalizedElapsedTime);

			transform.position = new Vector3(
				// Mathf.Lerp(start.x, end.x, modifiedTime),
				// start.y + yJumpMax * yJumpCurve.Evaluate(modifiedTime)
				start.x,
				Mathf.Lerp(start.y, end.y, modifiedTime)
			);

			yield return null;
		}

		animator.Play("Spidy_climb");
		midJump = false;
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
		else if (collision.CompareTag("Collectables/Web Cartridge")) {
			collectItem(ItemType.WebCartridge);
			Destroy(collision.gameObject);

			// TODO collect
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

	internal void dropCivilian()
	{
		throw new NotImplementedException();
	}

	private void failAndEndGame()
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
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

		Debug.Log("updatePlayerStats: NOT IMPLEMENTED");
	}

	// private void redrawBlackHearts()
	// {
	// 	throw new NotImplementedException();
	// }

	private void enableUnloadingOfCivilianIfHasWebs()
	{
		// throw new NotImplementedException();
	}

	private void redrawWebsCounter()
	{
		gameState.updateAvailableWebCartridges(webCartridges.current, webCartridges.max);
	}

	private void disableUnloadingOfCivilian()
	{
		throw new NotImplementedException();
	}

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
			GetComponent<Collider2D>().isTrigger = true;
		}
		
		GetComponent<Collider2D>().isTrigger = false;
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

	// bool isInvincible() {
	// 	return invincibilityCoefficient == 0;
	// }
}
