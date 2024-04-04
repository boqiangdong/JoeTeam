using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the Plane's status, movement, and shooting.
/// </summary>
public class EnemyPlane : MonoBehaviour
{
	public float 			Speed 					= 0.1f;	// The plane's movement speed
	public int 				Health 					= 10;   // Holds the health of this plane
	public Transform 	BurningEffect;         	// The effect that is created when the plane is burning
	public Transform 	Debris;                	// The debris object that flies from the crashed plane
	public Transform 	ExplosionEffect;       	// The effect created when the plane explodes
	public Transform 	SplashEffect;          	// The effect created when the plane hits the water
	public AudioClip 	FlyBySound;            	// The sound of the plane when it enters the screen
	public AudioClip 	FlyAwaySound;          	// The sound of the plane when it leaves the screen

	public Transform 	BulletPrefab;          	// The bullet shot from the plane
	public Transform 	MuzzlePrefab;          	// The flash effect that appears at the mouth of a gun when it fires.
	public int 				BulletSpeed 		= 5;    // Bullet's speed when shot
	public int 				ShotCoolDown 		= 20;   // How long to wait between each new bullet shot
	public int 				AttackInterval 	= 0;    // How long to wait between each new attack
	public int 				ShotsPerAttack 	= 1;    // How many bullets to shoot each attack

	private GUInterface 	CameraUI; 								// Holds a reference to the GUI Interface on the main camera.
	private CameraShake 	CameraShaker; 						// Holds a reference to the Camera Shake script on the main camera.
	private MeshRenderer 	CrashMeshRenderer; 				// Holds the reference to the MeshRenderer for a plane crash.
	private MeshRenderer 	PlaneMeshRenderer; 				// Holds the reference to the MeshRenderer for a normal plane.
	private int 					ShotCoolDownCount 	= 20; // Used to hold the original cooldown value set by us
	private int 					AttackIntervalCount = 0;  // Used to hold the original Attack Interval set by us
	private int 					ShotsPerAttackCount = 1;  // Used to hold the original Shots Per Attack set by us

	private GameObject 	Player;									// Holds the player object
	private Transform 	DebrisCopy;							// A copy of the plane gib, A part that is created as flying debri from the crashed object
	private Vector3 		OriginPos;							// Holds the origin of the plane. When animating the entry of the plane, it will go towards this point
	private int 				TiltAngle;							// The tilt of the plane
	private bool 				SplashCheck 	= false;	// Used to check if the plane entered the water or not
	private float 			CrashSpeed 		= 0;			// Used to give the plane an acceleration when crashing
	private bool 				Crashed 			= false;	// Used to check wether an plane crashed or not
	private int 				AttackStatus 	= 0;			// 0-Entering the screen, 2-Start attacking, 3-Leave the screen
	private float 			Scramble 			= 0;			// Used to animate the flying away of the plane

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start ()
	{
		Player 						= GameObject.Find("Player"); // Get the reference to the Player game object.
		CameraUI 					= Camera.main.GetComponent<GUInterface>(); // Get the reference to the GUInterface script on the main camera.
		CameraShaker 			= Camera.main.GetComponent<CameraShake>(); // Get a reference to the CameraShake script on the main camera.
		PlaneMeshRenderer = transform.Find("WarPlane").GetComponent<MeshRenderer>(); // Get reference to the normal plane mesh renderer.
		CrashMeshRenderer = transform.Find("WarPlaneCrash").GetComponent<MeshRenderer>(); // Get reference to the crash mesh renderer.

		if (CrashMeshRenderer)
		{
			CrashMeshRenderer.enabled = false; // Disable the war plane crash mesh, will use later when you down the enemy.
		}

		OriginPos = transform.position; // Set the plane's origin

		transform.position = new Vector3(Random.value * 20 - 10, 5, 0); // Put the plane off screen, so we can animate its entry

		transform.LookAt(OriginPos); // Make the plane look at the origin point

		CameraShaker.Shake = (int)(40 * Random.value + 40); // Shake the camera for a nice effect

		GetComponent<AudioSource>().PlayOneShot(FlyBySound); // Play a FlyBy audio
	}

	public void Update ()
	{
		if (AttackStatus == 0) // Intro, approaching from off screen above and behind the player, going staright to a spot above your origin point set on creation
		{
			if (Vector3.Distance(transform.position, (OriginPos)) > 8)
			{
				// Move towards a point above the origin point, and spin the plane a little
				transform.position -= (transform.position - (OriginPos + new Vector3(0, 5, 0))) * 0.02f;
				transform.Rotate(Vector3.forward, transform.position.x * 1.48f, Space.Self);
			}
			else if (Vector3.Distance(transform.position, (OriginPos)) < 8 && Vector3.Distance(transform.position, (OriginPos)) > 5.1f)
			{
				// Straighten up and prepare to change direction
				transform.position -= (transform.position - (OriginPos + new Vector3(0, 5, 0))) * 0.02f;
				transform.eulerAngles -= new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, (transform.eulerAngles.z - 180) * 0.04f);
			}
			else if (Vector3.Distance(transform.position, (OriginPos)) < 5.1f && Vector3.Distance(transform.position, OriginPos) > 0.5f)
			{
				// Rotate around to look back in the direction of the player
				transform.position -= (transform.position - (OriginPos)) * 0.02f;
				transform.Rotate(Vector3.left, 1.5f, Space.Self);
			}
			else if (Vector3.Distance(transform.position, OriginPos) < 0.5f)
			{
				// Change to attack status
				AttackStatus = 2;
			}
		}
		else if (AttackStatus == 2)
		{
			// As long as your health is above 0, keep moveing towards the player and shooting at him in bursts
			if (Health > 0)
			{
				// Rotate all propellers. In this case it's just the plane's single propeller, but for example using this same code moves all the zeppelin's proppellers
				foreach (Transform Propeller in transform)
				{
					if (Propeller.name == "Propeller")
						Propeller.Rotate(Vector3.forward, 15, Space.Self);
				}

				// If the player exists in the scene, look straight at him, otherwise look straight at a point to the center and offscreen
				if (Player)
				{
					transform.LookAt(Player.transform.position);
				}
				else
				{
					transform.LookAt(new Vector3(0, 0, -5));
				}

				transform.Translate(Vector3.forward * Speed, Space.Self); //Move in the direction you're looking

				// If the player exists, make the plane rotate towards it
				if (Player)
				{
					// Set the tilt angle by checking the angle between the player and the plane
					TiltAngle = (int)Quaternion.Angle(transform.rotation, Player.transform.rotation);

					// If the plane is on the right side of the player, make it tilt right, otherwise tilt left
					if (Player.transform.position.x > transform.position.x)
					{
						transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, (180 - TiltAngle) * 2);
					}
					else
					{
						transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, (180 - TiltAngle) * -2);
					}

					// If you're within shooting range of the player, shoot burst at him, with intervals in between
					if (Vector3.Distance(Player.transform.position, transform.position) > 10 && Vector3.Distance(Player.transform.position, transform.position) < 40)
					{
						// First, we wait for a set amount of frame updates, and then start shooting a burst
						if (AttackIntervalCount > 0)
						{
							AttackIntervalCount--;
						}
						else
						{
							// The rate of fire is set by the value of ShotCooldown, and is set in frame updates
							if (ShotCoolDownCount > 0)
							{
								// Decrease the cooldown counter
								ShotCoolDownCount--;
							}
							else
							{
								// Shoot a bullet from the left gun
								Shoot(-0.8f);

								// Shoot a bullet from the right gun
								Shoot(0.8f);

								// The number of shots per burst is set by ShotsPerAttack
								if (ShotsPerAttackCount > 0)
								{
									ShotsPerAttackCount--;
								}
								else
								{
									ShotsPerAttackCount = ShotsPerAttack;
									AttackIntervalCount = AttackInterval;
								}
							}
						}
					}

					// If you get too clos to the player, switch to status 3, fly away
					if (Vector3.Distance(Player.transform.position, transform.position) <= 10)
					{
						AttackStatus = 3;

						// Play a FlyAway audio
						if (GetComponent<AudioSource>() && GetComponent<AudioSource>().enabled)
						{
							GetComponent<AudioSource>().PlayOneShot(FlyAwaySound);
						}
					}
				}
			}
			else
			{
				if (!Crashed) // If you crashed, perform this code just once
				{
					Crashed = true;

					// Make hte plane object invisible, and the crashed plane object visible
					PlaneMeshRenderer.enabled = false;
					CrashMeshRenderer.enabled = true;

					// Create a debris object at the position of the plane, and throw it away
					DebrisCopy = Instantiate(Debris, transform.position + new Vector3(-1, 0, 0), transform.rotation) as Transform;

					DebrisCopy.Rotate(Vector3.up, 180, Space.Self);
					DebrisCopy.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, 300, -300), transform.position);

					Destroy(DebrisCopy.gameObject, 3);

					// Create an explosion
					Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1.6f, 0), transform.rotation);

					// Shake the camera
					CameraShaker.Shake = (int)(70 * Random.value + 50);

					// Add to the player's score
					CameraUI.Score += 200;
				}

				// Create a constant burning effect trailing from the plane
				if (BurningEffect)
				{
					if (Random.value > 0.8f)
					{
						Instantiate(BurningEffect, transform.position + new Vector3(Random.value * 1 - 0.5f, Random.value * 1 - 0.5f, Random.value * 1 - 0.5f), transform.rotation); // Create an explosion object
					}
				}

				// Keep moving the plane in the direction it's looking at
				transform.Translate(Vector3.forward * Speed, Space.Self);

				// Decrease the speed slightly, but not to 0
				if (CrashSpeed < 0.01f)
					CrashSpeed -= 0.0001f;

				// Move the plane down
				transform.Translate(Vector3.up * CrashSpeed, Space.World);

				//tilt the player a little
				transform.Rotate(Vector3.left, CrashSpeed * 8, Space.Self);
				transform.Rotate(Vector3.forward, CrashSpeed * 100, Space.Self);

				// If you hit the water surface, create a Splash effect
				if (SplashEffect)
				{
					if (transform.position.y < GameObject.Find("Water").transform.position.y + 1 && !SplashCheck)
					{
						SplashCheck = true;

						Instantiate(SplashEffect, transform.position, Quaternion.identity); // Create an explosion object

						// Remove the object from the scene after a few seconds
						Destroy(gameObject, 2);
					}
				}
			}

		}
		else if (AttackStatus == 3) // Escaping from the scene
		{
			if (Scramble < 0.5f)
			{
				Scramble += 0.01f;

				// Move the plane away from the scene
				transform.Translate(Vector3.forward * (Speed + Scramble), Space.Self);

				transform.Rotate(Vector3.left, Speed * 5, Space.Self);
			}
			else
			{
				Destroy(gameObject, 2);
			}
		}
	}

	/// <summary>
	/// Creates a bullet object, and shoots it forward from the plane
	/// </summary>
	/// <param name="Offset">Offset of the bullet position so it doesn't hit what it's not suppose to.</param>
	private void Shoot (float Offset)
	{
		// If a Bullet object was set by the player in the inspector, create it.
		if (BulletPrefab)
		{
			// Create a copy of a bullet from the library. Also move the bullet a little forward, so it doesn't shoot straight out of the plane's body
			Transform BulletCopy = Instantiate(BulletPrefab, transform.position, transform.rotation) as Transform;

			// Offset the bullet's starting point by the value of Offset
			BulletCopy.Translate(Vector3.left * Offset);

			// Move the source of the bullet a little forward
			BulletCopy.Translate(Vector3.forward * 1.5f, Space.Self);

			// If a Muzzle object was set by the player in the inspector, create it.
			if (MuzzlePrefab)
			{
				// Create a muzzle effect at the position of the bullet, and offset a little towards the plane, and lower it a little
				Instantiate(MuzzlePrefab, BulletCopy.position, BulletCopy.rotation);
			}

			// Give the bullet some forward force
			BulletCopy.transform.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed * 100);

			// Set the name of the bullet source to this object's name, so we avoid collision with the object the bullet it being shot from
			BulletCopy.GetComponent<Bullet>().BulletSource = transform.name;

			// Reset the cooldown timer for the bullet
			ShotCoolDownCount = ShotCoolDown;
		}
	}
}
