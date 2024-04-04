using UnityEngine;
using System.Collections;

public class SupportPlane : MonoBehaviour
{
	private float PullUp = 0;

	void Update ()
	{
		//Rotate all propellers 
		foreach (Transform Propeller in transform)
		{
			if (Propeller.name == "Propeller")
			{
				Propeller.Rotate(Vector3.forward, 15, Space.Self);
			}
		}

		transform.Translate(Vector3.forward * 0.05f, Space.Self);

		if (PullUp < 0.09)
		{
			PullUp += 0.0001f;

			transform.Rotate(Vector3.left, PullUp, Space.Self);
			transform.Rotate(Vector3.forward, PullUp, Space.Self);
		}

		if (PullUp >= 0.1)
		{
			Destroy(gameObject);
		}
	}
}
