using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class PlayerState : MonoBehaviour
{
	Stat redHalfHearts;
	Stat blackHalfHearts;

	Stat missedPeople;
	Stat webs;

	GameState gameState;
	
	GenerateBuildings generateBuildingsScript;
	Dictionary<DamageSource, float> healthDamage;
	Dictionary<DamageSource, float> speedDamage;
	// float[] spidyVerticalPositions;
	// float[] spidyHorizontalPositions;
	Animator animator;

	[SerializeField] AnimationCurve yJumpCurve;
	[SerializeField] AnimationCurve jumpTimeCurve;
	[SerializeField] float jumpDuration = 1f;
	[SerializeField] float yJumpMax;
	internal Vector3[,] spidyPositions;
	internal int[] spidyCurrentPosition;
	bool midJump = false;
	TrailRenderer spidyTrail;

	SpriteRenderer spriteRenderer;

	void Awake()
	{	
		animator = GetComponent<Animator>();
		spidyTrail = GetComponent<TrailRenderer>();
		// spidyTrail.emitting = false;
		// spidyTrail.enabled = false;
		// spidyTrail.Clear();

		spriteRenderer = GetComponent<SpriteRenderer>();

		healthDamage = new Dictionary<DamageSource, float>() {
			{DamageSource.Grenade, 1f},
			{DamageSource.Rubble, 0.5f}
		};
		speedDamage = new Dictionary<DamageSource, float>() {
			{DamageSource.Grenade, 1f},
			{DamageSource.Rubble, 0}
		};
		redHalfHearts = new Stat(
			6,
			0,
			6,
			redrawRedHearts,
			deathAndGameOver,
			null
		);
		blackHalfHearts = new Stat(
			0,
			0,
			float.PositiveInfinity,
			() => {redrawBlackHearts(); updatePlayerStats();},
			updateGameSpeed,
			null
		);
		webs = new Stat(
			5,
			0,
			5,
			() => {redrawWebsCounter(); enableUnloadingOfCivilianIfHasWebs();},
			disableUnloadingOfCivilian,
			null
		);
		missedPeople = new Stat(
			0,
			0,
			5,
			redrawMissedCiviliansCounter,
			null,
			failAndEndGame
		);
		// GetComponent<GameState>();
	}

	void Start() {
		generateBuildingsScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GenerateBuildings>();
		
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
		Stat relevantHealthbar = redHalfHearts;

		if (!blackHalfHearts.isEmpty())
		{
			relevantHealthbar = blackHalfHearts;
		}

		relevantHealthbar.updateCurrent(relevantHealthbar.current - healthDamage[damageSource]);
		gameState.gameSpeed.updateCurrent(gameState.gameSpeed.current - speedDamage[damageSource]); // TODO : might want to make immune to speed loss when have black hearts
	}

	public void collectItem(ItemType itemType)
	{
		throw new NotImplementedException();
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
		
		float normalizedElapsedTime = 0;

		while (normalizedElapsedTime < jumpDuration) {
			normalizedElapsedTime += Time.deltaTime;
			float temp = normalizedElapsedTime / jumpDuration;
			if (normalizedElapsedTime / jumpDuration >= 0.7f) animator.Play("Spidy_jump_down");

			float modifiedTime = jumpTimeCurve.Evaluate(temp);

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
		
		if (newPositionY != spidyCurrentPosition[1]) {
			// animator.Play(y < 0 ? "Spidy_jump_left" : "Spidy_jump_right");
			
			spidyCurrentPosition[1] = newPositionY;
			transform.position = getPositionAsVector3();
			// transform.localPosition = Vector3.zero;//spidyPositions[newPosition, playerCurrentPosition[1]];
			// transform.TransformPoint(Vector2.one);
		}

		// Debug.Log("updated position: " + string.Join(", ", spidyCurrentPosition));
	}

	private void OnTriggerEnter2D(Collider2D collision) {

		// TODO react accordingly
		collision.CompareTag("Symbiote");
		collision.CompareTag("Web Cartridge");
		collision.CompareTag("Civilian");
		collision.CompareTag("Symbiote");
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
		throw new NotImplementedException();
	}

	private void redrawRedHearts()
	{
		throw new NotImplementedException();
	}

	private void deathAndGameOver()
	{
		throw new NotImplementedException();
	}

	private void updateGameSpeed()
	{
		throw new NotImplementedException();
	}

	private void updatePlayerStats()
	{
		throw new NotImplementedException();
	}

	private void redrawBlackHearts()
	{
		throw new NotImplementedException();
	}

	private void enableUnloadingOfCivilianIfHasWebs()
	{
		throw new NotImplementedException();
	}

	private void redrawWebsCounter()
	{
		throw new NotImplementedException();
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
	void testBlinking() {
		StartCoroutine(damageAnimation());
	}

	IEnumerator damageAnimation() {
		float timeElapsed = 0;

		while (timeElapsed < damageInvincibiltyDuration) {
			spriteRenderer.color = transparentWhite;
			//.enabled = false;
			yield return new WaitForSeconds(blinkingInterval);
			spriteRenderer.color = Color.white;
			// spriteRenderer.enabled = true;
			yield return new WaitForSeconds(blinkingInterval);
			timeElapsed += 2*blinkingInterval;
		}
	}

	internal Vector3 getPositionAsVector3() {
		return spidyPositions[
			spidyCurrentPosition[0], 
			spidyCurrentPosition[1]
		];
	}
}
