using UnityEngine;
using System.Collections;

/// <summary>
/// This script moves the island object.
/// </summary>
public class MoveTerrain : MonoBehaviour
{
	void Update ()
	{
		//Move towards the camera
		transform.Translate(-0.1f * Vector3.forward, Space.Self);

		//Move the island up slowly so it appears to be getting closer from the horizon
		transform.position -= new Vector3(0.0f, (transform.position.y + 8) * 0.01f, 0.0f);

		//If the island goes off screen behind the player, reset it at a low point in the horizon
		if (transform.position.z < -100)
		{
			transform.position = new Vector3(transform.position.x, -30, 150);

			transform.Rotate(Vector3.up, Random.value, Space.Self);
		}
	}
}
