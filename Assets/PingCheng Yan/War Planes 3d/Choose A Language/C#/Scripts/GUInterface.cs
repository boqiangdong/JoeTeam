using UnityEngine;
using System.Collections;

/// <summary>
/// This script displays the health bar, and the score
/// </summary>
public class GUInterface : MonoBehaviour
{
	public GUISkin 		mySkin;							// The GUISkin we'll use, which holds info about the fonts and styles in the interface
	public Texture2D 	HealthGreenTexture;	// The texture of the green part of the health bar on the left
	public string 		BriefingText;				// The text to be displayed at the start of the level

	internal int Score = 0; // Player score.

	private PlayerControls 	PlayerControl;								// Player Controls reference from the player.
	private GameObject 			Player;												// The player's game object
	private float 					BriefingPosX 	= Screen.width;	// The position of the briefing text
	private float 					HUDPosX 			= -70f;					// Used to animate the HUD popping from the left
	private float 					MaxHealth 		= 0; 						// The maximum health value

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start ()
	{
		Player = GameObject.Find("Player"); // We set the player object

		if (Player)
		{
			PlayerControl = Player.GetComponent<PlayerControls>();
			MaxHealth = PlayerControl.Health;
		}
	}

	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	public void OnGUI ()
	{
		// Set the general GUI style we're going to use
		GUI.skin = mySkin;

		// At the start of the level, animate the entry of the GUI elements
		if (Time.timeSinceLevelLoad < 1)
		{
			HUDPosX -= (HUDPosX - 10) / 16;
		}

		// Create a an empty box for the health bar base.
		GUI.Box(new Rect(HUDPosX, 10, 300, 32), "", "HealthBarRed");

		// If the player object exists, do the following
		if (Player)
		{
			// If the health bar green is within the bar limit, display it onscreen
			if (PlayerControl.Health > 0)
			{
				// Create a box with the green health bar texture
				GUI.Box(new Rect(HUDPosX + 4, 10, (PlayerControl.Health / MaxHealth) * 300 - 8, 32), "", "HealthBarGreen");
			}

			// Create a an empty box for the barrage meter base.
			//GUI.Box ( Rect ( HUDPosX , 42, 300, 16), "", "BarrageMeterGray");

			// If the barrage meter white is within the bar limit, display it onscreen
			if (PlayerControl.BarrageMeterCount >= 0 && PlayerControl.BarrageMeterCount <= PlayerControl.BarrageMeter)
			{
				// Create a box with the barrage meter white texture
				//GUI.Box ( Rect ( HUDPosX + 4 , 42, (PlayerControl.BarrageMeterCount/PlayerControl.BarrageMeter) * 300 - 8, 16), "", "BarrageMeterWhite");
			}
			else
			{
				// Create a box with the barrage meter white texture
				//GUI.Box ( Rect ( HUDPosX + 4 , 42, 300 - 8, 16), "", "BarrageMeterWhite");
			}
		}

		// Score
		GUI.Label(new Rect(Screen.width * 0.7f - HUDPosX, 5, 240, 48), Score.ToString(), "BarrageMeterGray");

		// Animate the interface entering teh screen
		if (Time.timeSinceLevelLoad > 2 && Time.timeSinceLevelLoad < 5)
		{
			BriefingPosX -= BriefingPosX * 0.05f;
		}
		else if (Time.timeSinceLevelLoad > 5 && Time.timeSinceLevelLoad < 8)
		{
			BriefingPosX -= (BriefingPosX + Screen.width) * 0.05f;
		}

		// Set a briefing label
		GUI.Label(new Rect(Screen.width * 0.5f + BriefingPosX - 400, Screen.height * 0.5f, 800, 10), BriefingText);
	}
}
