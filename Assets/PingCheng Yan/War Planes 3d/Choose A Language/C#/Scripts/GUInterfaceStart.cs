using UnityEngine;
using System.Collections;

/// <summary>
/// This script displays a title screen, some description, and a start button
/// </summary>
public class GUInterfaceStart : MonoBehaviour
{
	public GUISkin 		mySkin;						// The GUISkin we'll use, which holds info about the fonts and styles in the interface
	public Texture2D 	TitleTexture;			// The title image
	public string 		DescriptionText;	// The description text

	void OnGUI ()
	{
		// Set the general GUI style we're going to use
		GUI.skin = mySkin;

		// Title graphic
		GUI.Label(new Rect(Screen.width * 0.5f - 256, Screen.height * 0.3f - 256, 512, 512), TitleTexture);

		// Description text
		GUI.Label(new Rect(Screen.width * 0.5f - 400, Screen.height * 0.5f, 800, 60), DescriptionText);

		// The start button
		if (GUI.Button(new Rect(Screen.width * 0.5f - 250, Screen.height * 0.8f, 500, 60), "Click to Start"))
			Application.LoadLevel("Game"); // Load the game level
	}
}
