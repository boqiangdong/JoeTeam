using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script contorls the Zeppelin's status and movement, but not the cannons it holds, which are
/// controlled by a script in the cannon object. Make sure you check it out.
/// </summary>
public class EnemyZeppelin : MonoBehaviour
{
	public Transform	BurningEffect;							// The effect that is created when the object is burning
	public Transform 	ZepplinGib;									// The debris object that flies from the crashed object
	public Transform 	ExplosionEffect;						// The effect created when exploding
	public Transform 	SplashEffect;								// The splash efefct displayed when the Zeppelin hits the water
	public AudioClip 	BurningSound;								// The sound played while the zeppelin is falling
	public float 			Speed 						= 0.03f;	// The objects mvoement speed
	public int 				Health 						= 30;			// Holds the health of this object

	private GUInterface 	CameraUI; 								// Holds a reference to the GUI Interface on the main camera.
	private CameraShake 	CameraShaker; 						// Holds a reference to the Camera Shake script on the main camera.
	private MeshRenderer 	CrashMeshRenderer; 				// Holds the reference to the MeshRenderer for a zeppelin crash.
	private MeshRenderer 	ZeppelinMeshRenderer; 		// Holds the reference to the MeshRenderer for a normal zeppelin.

	private List<EnemyCannon> Cannons; // Collection of EnemyCannon objects.

	private Transform ZepplinGibCopy;								// A copy of the zeppelin gib. A part that is created as flying debri from the crashed object
	private float 		AttackRange;									// The range from which the Zeppelin's cannons start shooting
	private float 		CrashSpeed 					= 0;			// Used to give the zeppelin an acceleration when crashing
	private bool 			Crashed 						= false;	// Used to check wether an object crashed or not
	private bool 			SplashCheck 				= false;	// Check if the Zeppelin hit the water
	private int 			AttackStatus 				= 0;			// 0-Out of range, 1-Start attacking, 2-Stop attacking, 3-Remove from game

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		GameObject Player = GameObject.Find("Player"); 								// Reference to "Player" named game object.
		Cannons 					= new List<EnemyCannon>(); 									// Instantiate list for cannon objects to be used later.
		CameraUI 					= Camera.main.GetComponent<GUInterface>(); 	// Reference to GUInterface script on main camera.
		CameraShaker 			= Camera.main.GetComponent<CameraShake>(); 	// Reference to CameraShake script on main camera.

		ZeppelinMeshRenderer 	= transform.Find("Zeppelin").GetComponent<MeshRenderer>(); 				// Reference to the mesh renderer for the normal zeppelin mesh.
		CrashMeshRenderer 		= transform.Find("Zeppelin Crash").GetComponent<MeshRenderer>(); 	// Reference to the mesh renderer for the crashed zeppelin mesh.

		// Get references to each cannon, disable last added so the Zeppelin doesn't come in shooting, wait for range to start firing.
		foreach (Transform cannon in transform)
		{
			if (cannon.name == "Enemy Cannon")
			{
				Cannons.Add(cannon.GetComponent<EnemyCannon>()); // Add EnemyCannon reference to list/collection of EnemyCannon objects.
				Cannons[Cannons.Count - 1].enabled = false; // Since we know we added it last and a list is zero based, we disable the last added.
			}
		}

		// Make the crashed zeppelin object invisible
		CrashMeshRenderer.enabled = false;

		// Set the range for the zeppelin to start attacking
		if (Player)
		{
			AttackRange = Player.GetComponent<PlayerControls>().MovementLimit * 4;
		}
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update ()
	{
		// As long as the zeppelin's health is above 0, keep moving and attacking
		if (Health > 0)
		{
			// Rotate all propellers
			foreach (Transform Propeller in transform)
			{
				if (Propeller.name == "Propeller")
				{
					Propeller.Rotate(Vector3.forward, 15, Space.Self);
				}
			}

			// Move in the direction you're looking
			transform.Translate(Vector3.forward * Speed, Space.Self);

			// Make the zeppelin slower as it approches the center of the screen
			Speed = Mathf.Abs(transform.position.x * 0.002f) + 0.02f;

			// If the zeppelin is within range, turn on its cannons
			if (Mathf.Abs(transform.position.x) < AttackRange && AttackStatus == 0)
			{
				AttackStatus++;

				// Enable all the cannons
				foreach (EnemyCannon cannon in Cannons)
					cannon.enabled = true;
			}

			// If the zeppelin gets out of range, turn off its cannons
			if (Mathf.Abs(transform.position.x) > AttackRange && AttackStatus == 1)
			{
				AttackStatus++;

				// Disable all the cannons
				foreach (EnemyCannon cannon in Cannons)
					cannon.enabled = false;
			}

			// If the zeppelin gets very far away from the player, remove it from the scene
			if (Mathf.Abs(transform.position.x) > AttackRange * 4 && AttackStatus == 2)
			{
				AttackStatus++;

				Destroy(gameObject);
			}
		}
		else
		{
			if (!Crashed)
			{
				Crashed = true;

				// Make the crashed object visible, and the normal zeppelin object invisible
				ZeppelinMeshRenderer.enabled = false;
				CrashMeshRenderer.enabled = true;

				// Make all propellers invisible
				foreach (Transform Propeller in transform)
				{
					if (Propeller.name == "Propeller")
					{
						Propeller.GetComponent<MeshRenderer>().enabled = false;
					}
				}

				// Create a zeppelin gib, and throw it away
				ZepplinGibCopy = (Transform)Instantiate(ZepplinGib, transform.position + new Vector3(0, 1.6f, 0), transform.rotation); // Create an explosion object

				ZepplinGibCopy.Rotate(Vector3.up, 180, Space.Self);
				ZepplinGibCopy.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, 300, -300), transform.position);

				Destroy(ZepplinGibCopy.gameObject, 3);

				// Create a zeppelin gib, and throw it away
				ZepplinGibCopy = (Transform)Instantiate(ZepplinGib, transform.position + new Vector3(1.5f, 1.6f, 1), transform.rotation); // Create an explosion object

				ZepplinGibCopy.Rotate(Vector3.up, 180, Space.Self);
				ZepplinGibCopy.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, 300, 300), transform.position);

				Destroy(ZepplinGibCopy.gameObject, 3);

				// Create an explosion
				Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1.6f, 0), transform.rotation);

				// Shake the camera
				CameraShaker.Shake = (int)(120 * Random.value + 80);

				// Add to the player's score
				CameraUI.Score += 500;
			}

			// Create a trailing burning effect as long as the zeppelin is crashing
			if (BurningEffect)
			{
				if (Random.value > 0.4)
				{
					Instantiate(BurningEffect, transform.position + new Vector3(Random.value * 3 - 2, Random.value * 2 - 1, Random.value * 2 - 1), transform.rotation); // Create an explosion object

					if (Random.value > 0.9)
					{
						// Play a burning sound effect
						GetComponent<AudioSource>().PlayOneShot(BurningSound);
					}
				}
			}

			// Disable all the cannons
			foreach (EnemyCannon cannon in Cannons)
				cannon.enabled = false;

			// Keep moving forward
			transform.Translate(Vector3.forward * Speed, Space.Self);

			// Reduce the speed, but not to 0
			if (CrashSpeed < 0.01f)
				CrashSpeed -= 0.0001f;

			// Move down
			transform.Translate(Vector3.up * CrashSpeed, Space.World);

			// Tilt a little
			transform.Rotate(Vector3.left, CrashSpeed * 8, Space.Self);
			transform.Rotate(Vector3.forward, CrashSpeed * 8, Space.Self);

			// If you hit the water surface, create a Splash effect
			if (SplashEffect)
			{
				if (transform.position.y < GameObject.Find("Water").transform.position.y + 0.01f && !SplashCheck)
				{
					SplashCheck = true;

					Instantiate(SplashEffect, transform.position, Quaternion.identity); // Create an explosion object

					// Remove the object from the scene after a few seconds
					Destroy(gameObject, 2);
				}
			}
		}
	}
}
