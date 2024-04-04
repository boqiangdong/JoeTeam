using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the Cannon, making it look at the player and shoot at him in bursts.
/// </summary>
public class EnemyCannon : MonoBehaviour
{
	public Transform BulletPrefab; // The bullet prefab object to be shot from the plane
	public Transform MuzzlePrefab; // The flash effect that appears at the mouth of a gun when it fires.

	public int BulletSpeed 					= 5; 	// Bullet's speed when shot
	public int ShotCoolDown 				= 20; // How long to wait between each new bullet shot
	public int AttackInterval 			= 70; // How long to wait between each new attack
	public int ShotsPerAttack 			= 3; 	// How many bullets to shoot each attack

	private GameObject 	Player; 									// Holds the player object
	private Transform 	BulletCopy; 							// Holds a copy of the created bullet object so we can give it some velocity later
	private Transform 	Propeller; 								// Used to holds the propeller which is in this object
	private int 				ShotCoolDownCount 	= 20; // Used to hold the original cooldown value set by us
	private int 				AttackIntervalCount = 70; // Used to hold the original Attack Interval set by us
	private int 				ShotsPerAttackCount = 3; 	// Used to hold the original Shots Per Attack set by us

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		Player = GameObject.Find("Player"); // Set the player object
	}

	/// <summary>
	/// Update this instance of the enemy connon.
	/// </summary>
	void Update ()
	{
		if (Player)
		{
			transform.LookAt(Player.transform.position); // Always look at the player
		}

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
				Shoot(-0.1f);

				// Shoot a bullet from the right gun
				Shoot(0.1f);

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

	/// <summary>
	/// Creates a bullet object, and shoots it forward.
	/// </summary>
	/// <param name="Offset">Bullet offset</param>
	void Shoot (float Offset)
	{
		// If a Bullet object was set by the player in the inspector, create it.
		if (BulletPrefab)
		{
			// Create a copy of a bullet from the library. Also move the bullet a little forward, so it doesn't shoot straight out of the object's center
			BulletCopy = Instantiate(BulletPrefab, transform.position + Vector3.left * Offset, transform.rotation) as Transform;

			// Move the source of the bullet a little forward
			BulletCopy.Translate(Vector3.forward * 1.5f, Space.Self);

			// If a Muzzle object was set by the player in the inspector, create it.
			if (MuzzlePrefab)
			{
				// Create a muzzle effect at the position of the bullet
				Instantiate(MuzzlePrefab, BulletCopy.position, BulletCopy.rotation);
			}

			// Give the bullet some forward force
			BulletCopy.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed * 100);

			// Set the name of the bullet source to this object's name, so we can keep track of the source of the bullet
			BulletCopy.GetComponent<Bullet>().BulletSource = transform.name;

			// Reset the cooldown timer for the bullet
			ShotCoolDownCount = ShotCoolDown;
		}
	}
}
