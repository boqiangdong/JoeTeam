using UnityEngine;
using System.Collections;

/// <summary>
/// This script displays the total score and a restart button
/// </summary>
public class GUInterfaceEnd : MonoBehaviour
{
	public GUISkin mySkin; // The GUISkin we'll use, which holds info about the fonts and styles in the interface

	private int Score; // The total score value

	void Start ()
	{
		// Find the TotalScore game object and put its value into Score
		Score = GameObject.Find("TotalScore").GetComponent<TotalScore>().TotalScoreCount;

		Destroy(GameObject.Find("TotalScore")); // Remove the total score object
	}

	void OnGUI ()
	{
		// Set the general GUI style we're going to use
		GUI.skin = mySkin;

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if (GUI.Button(new Rect(Screen.width * 0.7f - 150, Screen.height * 0.8f, 300, 60), "Restart"))
		{
			Application.LoadLevel("Start");
		}

		// Score 
		GUI.Box(new Rect(Screen.width * 0.7f - 200, Screen.height * 0.2f, 400, 200), Score.ToString());
	}
}
