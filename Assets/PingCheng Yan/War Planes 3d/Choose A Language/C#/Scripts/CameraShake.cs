using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the shaking effect, and the tilting of the camera.
/// </summary>
public class CameraShake : MonoBehaviour
{
	internal int 				Shake = 0; 			// How long and how shaky the camera gets.

	private Vector3 		CameraPosition; // Original position of the camera
	private GameObject 	Player; 				// The player game object

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		CameraPosition 	= transform.position; 				// Set the original position of the camera so we can return to it after shaking the camera.
		Player 					= GameObject.Find("Player"); 	// Get a reference to the "Player" game object.
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		if (Shake > 0)
		{
			// Decrease the shake value
			Shake--;

			// print("shake" + Shake);
			transform.position = CameraPosition + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * (float)Shake * 0.002f;
		}

		// Tilt the camera based on the player position
		if (Player)
			transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, Player.transform.rotation.z * -0.1f, transform.rotation.w);
	}
}
