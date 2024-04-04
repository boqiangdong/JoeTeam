using UnityEngine;
using System.Collections;

/// <summary>
/// This script controls the player's status, movement and shooting.
/// </summary>
public class PlayerControls : MonoBehaviour
{
	public Transform 	BulletPrefab; 							// The bullet shot from the plane
	public Transform 	MuzzlePrefab; 							// The flash effect that appears at the mouth of a gun when it fires.
	public Transform 	BarragePrefab; 							// The missile shot from the support barrage
	public Transform 	ExplosionPrefab; 						// The explosion effect when the player is destroyed
	public Transform 	PlaneGib; 									// The crashed object that will be created when the player is destroyed
	public Transform 	SupportPlane; 							// The support plane object that flys over after a support barrage
	public float 			Speed 						= 0.05f; 	// Your speed
	public float 			Health 						= 100; 		// Your health
	public int 				RegainHealthAfter = 200; 		// How frame updates to wait before regaining health, as long as you are not hit
	public float 			RegainHealthRate 	= 0.04f; 	// How many health points to regain each frame update
	public int 				BulletSpeed 			= 10; 		// Bullet's speed when shot
	public int 				ShotCoolDown 			= 10; 		// How long to wait between each new bullet shot
	public int 				ShotsPerBarrage 	= 50; 		// How many missiles to be shot when a barrage is activated
	public float 			BarrageMeter 			= 50; 		// How many points you must get before unlocking a barrage
	public float 			MovementLimit 		= 3.5f; 	// Horizontal movement limit of the player
	public int 				CameraPreset 			= 1; 			// Holds the current camera preset, either 1 or 2

	internal bool 		RegainHealth 			= false; 	// Check if the player should regenerate his health
	internal int 			RegainHealthCount = 0; 			// Used to count from 0 to RegainHealthAfter, so we can start regaining health after that
	internal float 		BarrageMeterCount = 0; 			// Used to count from 0 to BarrageMeter.

	private Transform Propeller;											// Used to holds the propeller which is in this object
	private Vector3 	PlayerOriginPos;								// The original position of the player
	private float 		HealthMax;											// Set the maximum health value, so we don't go over it while regenerating health
	private int 			ShotCoolDownCount;							// Used to hold the original cooldown value set by us
	private int 			BarrageCount 					= 0;			// Used to count from 0 to ShotsPerBarrage.
	private float 		EaseControlFromIntro 	= 0; 			// Used to ease the transition from the Intro to the player-controlled game.
	private bool 			BarrageCheck					= false;	// Check if a support barrage is in progress, and prevents any other barrages from being performed at the same time

	private LevelController LevelControls; 	// Reference to the LevelController script on main camera.
	private CameraShake CameraShaker; 			// Reference to the camera shake script on the main camera.

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start ()
	{
		Propeller 					= transform.Find("Propeller"); // Reference to Propeller object on this object.
		PlayerOriginPos 		= transform.position; // Hold the original position of the player, so we can make the payer appear from off screen and eas into position
		LevelControls 			= Camera.main.GetComponent<LevelController>(); // Get reference to level controller script on main camera.
		CameraShaker				= Camera.main.GetComponent<CameraShake>(); // Get reference to camera shake script on main camera.
		ShotCoolDownCount 	= ShotCoolDown;
		transform.position 	= new Vector3(10, 10, 10); // Put the player off screen

		HealthMax = Health;

		// Set the camera position based on one of two presets
		if (CameraPreset == 1)
		{
			Camera.main.transform.position = new Vector3(0, 2.3f, -4);
		}
		else if (CameraPreset == 2)
		{
			Camera.main.transform.position = new Vector3(0, 3.3f, -4);
		}
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update ()
	{
		// Animate the entry of the player into the scene
		if (LevelControls.LevelStatus == 0)
		{
			// Move towards the origin position
			transform.position -= (transform.position - PlayerOriginPos) * 0.02f;

			// Tilt based on your position, and also keep a looping a slight tilt regardless of your position
			transform.Rotate(Vector3.forward, transform.position.x * 1.48f, Space.Self);

			// If you get close enough change the game's status to 1
			if (Vector3.Distance(transform.position, PlayerOriginPos) < 0.1)
			{
				LevelControls.LevelStatus = 1;
				LevelControls.CheckStatus();
			}
		}

		if (LevelControls.LevelStatus == 1)
		{
			if (Health > 0) // If health is above 0, stay in control
			{
				// Move towards the position of the mouse on screen slowly
				transform.position -= new Vector3(((transform.position.x - ((Input.mousePosition.x - Screen.width * 0.5f) * 0.007f)) * Speed) * EaseControlFromIntro, transform.position.y, transform.position.z);

				// Limit the player's position to a specific horizontal area
				if (transform.position.x > PlayerOriginPos.x + MovementLimit)
				{
					transform.position = new Vector3(PlayerOriginPos.x + MovementLimit, transform.position.y, transform.position.z);
				}

				if (transform.position.x < PlayerOriginPos.x - MovementLimit)
				{
					transform.position = new Vector3(PlayerOriginPos.x - MovementLimit, transform.position.y, transform.position.z);
				}

				// Tilt based on your position, and also keep a looping slight tilt regardless of your posiition
				transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, ((Input.mousePosition.x - Screen.width * 0.5f) * 0.0006f + (Mathf.Sin((Time.time) * 3)) * 0.02f) * EaseControlFromIntro, transform.rotation.w);

				if (EaseControlFromIntro < 1)
					EaseControlFromIntro += 0.01f;

				// If you hold Ctrl or the Left Mouse Button, you start shooting
				if (ShotCoolDownCount > 0)
				{
					// Decrease the cooldown counter
					ShotCoolDownCount--;
				}
				else
				{
					// If you click the Left Mouse Button, or Ctrl, Shoot!
					if (Input.GetButton("Fire1"))
					{
						// If the barrage meter hasn't filled up, shoot a regular bullet
						if (BarrageMeterCount < BarrageMeter)
						{
							// Shoot a bullet from the left gun
							Shoot(-0.8f);

							// Shoot a bullet from the right gun
							Shoot(0.8f);
						}
						else // If the barrage meter is full, shoot a barrage and reset the meter counter
						{
							if (!BarrageCheck)
							{
								// Reset the barrage counter
								BarrageCount = 0;

								// Set the barrage check to true
								BarrageCheck = true;

								// Shoot a barrage of missiles
								StartCoroutine(ShootBarrage());
							}
						}
					}
				}

				// If the player's health regeneration check is true, start adding to its health based on the value of RegainHealthRate
				if (RegainHealth)
				{
					if (RegainHealthCount < RegainHealthAfter)
					{
						RegainHealthCount++;
					}
					else
					{
						// As long the the player's health is lower than the maximum health, keep adding RegainHealthRate to it. Once you get to the maximum value, set the regeneration state back to false
						if (Health < HealthMax)
						{
							Health += RegainHealthRate;
						}
						else
						{
							RegainHealth = false;
							RegainHealthCount = 0;
						}
					}
				}
			}
			else
			{
				// Change to status 2, the game ends
				LevelControls.LevelStatus = 2;
				LevelControls.CheckStatus();

				Destroy(gameObject); // Destroy the player object

				// Create a crashed game object
				Transform PlaneGibCopy = Instantiate(PlaneGib, transform.position + new Vector3(0, 0, 0), transform.rotation) as Transform; // Create an explosion object

				PlaneGibCopy.Rotate(Vector3.up, 180, Space.Self);
				PlaneGibCopy.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(20, 20, 0), transform.position + new Vector3(5, 0, 0));

				Destroy(PlaneGibCopy.gameObject, 3);

				// Create an explosion
				Instantiate(ExplosionPrefab, transform.position + new Vector3(0, 0, 0), transform.rotation); // Create an explosion object
			}
		}

		// Rotate the propeller to create a nice effect
		Propeller.Rotate(Vector3.forward, 15, Space.Self);
	}

	/// <summary>
	/// Creates a bullet object, and shoots it forward from the plane
	/// </summary>
	/// <param name="Offset">Offset of the bullet so it doesn't hit the shooting plane</param>
	void Shoot (float Offset)
	{
		// If a Bullet object was set by the player in the inspector, create it.
		if (BulletPrefab)
		{
			// Create a copy of a bullet from the library. Also move the bullet a little forward, so it doesn't shoot straight out of the plane's body
			Transform BulletCopy = Instantiate(BulletPrefab, transform.position + Vector3.forward * 0.5f, transform.rotation) as Transform;

			// Offset the bullet's starting point by the value of Offset
			BulletCopy.Translate(Vector3.left * Offset);

			// If a Muzzle object was set by the player in the inspector, create it.
			if (MuzzlePrefab)
			{
				// Create a muzzle effect at the position of the bullet, and offset a little towards the plane, and lower it a little
				Instantiate(MuzzlePrefab, BulletCopy.position - Vector3.forward * 0.15f, BulletCopy.rotation);
			}

			// Give the bullet some forward force
			BulletCopy.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed * 100);

			// Set the name of the bullet source to this object's name, so we avoid collision with the object the bullet it being shot from
			BulletCopy.GetComponent<Bullet>().BulletSource = transform.name;

			// Reset the cooldown timer for the bullet
			ShotCoolDownCount = ShotCoolDown;
		}
	}

	/// <summary>
	///
	/// </summary>
	/// <returns></returns>
	IEnumerator ShootBarrage ()
	{
		while (BarrageCount < ShotsPerBarrage)
		{
			// Wait a little between each shot
			yield return StartCoroutine(WaitAndDelay(Random.value * 0.1f + 0.1f));

			// If a missile object was set by the player in the inspector, create it.
			if (BarragePrefab)
			{
				// Create a copy of a misisle from the library. Also move the missile backwards off screen, so it shoots from offscreen (from a huge support ship)
				Transform BulletCopy = Instantiate(BarragePrefab, transform.position + Vector3.forward * -2, transform.rotation) as Transform;

				// Also move the missile to a random spot around the screen
				BulletCopy.Translate(new Vector3(0, 10, 0), Space.Self);

				// Shake the camera
				CameraShaker.Shake = (int)(40 * Random.value + 40);

				BarrageCount++;
			}
			else
			{
				BarrageCount = ShotsPerBarrage;
			}
		}

		// Reset the meter counter
		BarrageMeterCount = 0;

		// Set the barrage to false
		BarrageCheck = false;

		// Create a Support Plane, that will fly over the player and into the horizon
		if (SupportPlane)
		{
			Instantiate(SupportPlane, new Vector3(0, 4, -8), Quaternion.identity);
		}

		// Shake the camera
		CameraShaker.Shake = (int)(100 * Random.value + 100);
	}


	/// <summary>
	/// Stalls game instructions
	/// </summary>
	/// <param name="waitTime">time to wait in seconds.</param>
	/// <returns>IEnumerator</returns>
	IEnumerator WaitAndDelay (float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
	}
}
