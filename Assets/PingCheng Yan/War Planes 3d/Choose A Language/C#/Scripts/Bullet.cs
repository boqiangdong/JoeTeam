using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the bullet and it's collision with other objects.
/// </summary>
public class Bullet : MonoBehaviour
{
	public Transform 	HitEffect; 	//the effect to be displayed when this object hits an enemy
	public AudioClip 	FlyBySound; //The sound of the bullet when it passes by the player, without hitting it
	public AudioClip 	HitSound; 	//The sound of the bullet when it hits an object
	public int 				Damage = 1; //How much damage a bullet causes

	internal string BulletSource; //The source from which the bullet was shot

	private GameObject 	Player; 				//Holds the player object
	private bool 				FlyBy = false; 	//Checks if the bullet passed by the player
	private GUInterface CameraUI; 			// Reference to GUInterface on the main camera.
	private CameraShake CameraShaker; 	// reference to the CameraShake script on the main camera.

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start ()
	{
		Player = GameObject.Find("Player"); //Set the player object

		CameraUI 			= Camera.main.GetComponent<GUInterface>();
		CameraShaker 	= Camera.main.GetComponent<CameraShake>();

		//Destroy the bullet object a few seconds after it is created
		Destroy(gameObject, 4);

		//If the bullet was shot from a Zeppelin Cannon, make it's sound's pitch a little lower, resulting in a deeper sound.
		if (BulletSource == "EnemyZeppelin(Clone)")
		{
			transform.GetComponent<AudioSource>().pitch = Random.value * 0.2f + 0.4f;
		}
		else
		{
			transform.GetComponent<AudioSource>().pitch = Random.value * 0.2f + 0.8f;
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update ()
	{
		//If the bullet passes by the player, make a special FlyBy sound
		if (!FlyBy)
		{
			if (transform.position.z < 0)
			{
				if (Player)
				{
					if (Vector3.Distance(transform.position, Player.transform.position) < 3)
					{
						//print("bullet flyby!");
						FlyBy = true;

						GetComponent<AudioSource>().PlayOneShot(FlyBySound);
					}
				}
			}
		}
	}

	/// <summary>
	/// Checks for collision with different enemy objects, including Planes, Zeppelins, and the player.
	/// </summary>
	/// <param name="collision">Detected Collision</param>
	public void OnCollisionEnter (Collision collision)
	{
		//print("hit something");
		ContactPoint contact = collision.contacts[0]; //the contact point of the object and the collider
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal); //set the rotation
		Vector3 pos = contact.point; //holds the contact point

		//Play a hit sound
		if (GetComponent<AudioSource>().enabled)
		{
			GetComponent<AudioSource>().PlayOneShot(HitSound);
		}
		
		//If we hit either an enemy or the player do the following...
		if (collision.collider.tag == "Enemy" || collision.collider.tag == "Player")
		{
			//print("hit enemy");

			if (HitEffect)
				Instantiate(HitEffect, pos, rot); //create an explosionObject

			Destroy(gameObject); //remove the object from the scene

			// Try and get EnemyZeppelin reference.
			EnemyZeppelin enemyZeppelin = collision.transform.GetComponent<EnemyZeppelin>();

			//If we hit a Zeppelin, reduce its health, and add to the game's score
			if (enemyZeppelin)
			{
				//Decrease the value of the player's health
				enemyZeppelin.Health -= Damage;

				//Add to the player's score
				CameraUI.Score += 10;

				//Increase the value of the player's barrage meter, which shoots a support barrage once filled up
				//Player.transform.GetComponent("PlayerControls").BarrageMeterCount += 1;
			}

			// Try and get a reference to the enemy plane if exists.
			EnemyPlane enemyPlane = collision.transform.GetComponent<EnemyPlane>();

			//If we hit an Enemy Plane, reduce its health, and add to the game's score
			if (enemyPlane)
			{
				//Decrease the value of the player's health
				enemyPlane.Health -= Damage;

				//Add to the player's score
				CameraUI.Score += 20;

				//Increase the value of the player's barrage meter, which shoots a support barrage once filled up
				//Player.transform.GetComponent("PlayerControls").BarrageMeterCount += 1;
			}

			//If we hit an Player's Plane, reduce its health, and shake the camera
			if (collision.collider.name == "Player")
			{
				//Shake the camera
				CameraShaker.Shake = 50 * (int)Random.value + 50;
				// Get player controls reference.
				PlayerControls playerControls = collision.transform.GetComponent<PlayerControls>();

				if (playerControls)
				{
					//Decrease the value of the player's health
					playerControls.Health -= Damage;

					//Set the regenerate state for the player to true, so he can start regainning health
					playerControls.RegainHealth = true;

					//Restart the counter value for the delay before starting to regenerate health
					playerControls.RegainHealthCount = 0;
				}
			}
		}
	}
}

