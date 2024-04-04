using UnityEngine;
using System.Collections;

/// <summary>
/// This script serves just one purpose. It holds the score, so we can pass it to the end scene and display a 
/// total score. Basically it is attached to an empty game object in the Game scene, and at the end of the 
/// scene it holds the value of the score so far. Then we make it indestructible and load the End scene. 
/// The object will carry on to the next scene, then we take the score from it and destroy it.
/// </summary>
public class TotalScore : MonoBehaviour
{
	public int TotalScoreCount = 0;
}
