using UnityEngine;
using System.Collections;

/// <summary>
/// This level controls the game progress, intro, game, and restart
/// </summary>
public class LevelController : MonoBehaviour
{
	public Transform 	EnemyType1; 		// Set the first type of enemy
	public Transform 	EnemyType2; 		// Set the second tpye of enemy
	public int 				EnemyCount = 0; // Set the enemy count

	internal int LevelStatus = 0; //0-Intro, 1-game progress, 3-restart

	private Transform EnemyCopy; 				// Used to create a copy of the enemy object
	private int				EnemyDelay 	= 0;	// Delay between each new enemy appearing

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start()
	{
		EnemyCopy = null;
	}

	/// <summary>
	/// Checks the current status of the game, 1 = in game, 2 = died.
	/// </summary>
	public void CheckStatus ()
	{
		//	If the game is in progress, create and enemy
		if (LevelStatus == 1)
			StartCoroutine(CreateEnemy());

		//	If the game ended run the restart function
		if (LevelStatus == 2)
			StartCoroutine(Restart());
	}

	/// <summary>
	/// Creates enemies, delayed added though coroutine so you don't have to many at one time.
	/// </summary>
	/// <returns></returns>
	public IEnumerator CreateEnemy ()
	{
		while (EnemyCount > 0 && LevelStatus == 1)
		{
			// Wait a few seconds before creating another enemy
			yield return StartCoroutine(WaitAndDelay(EnemyDelay));

			// Randomize between enemy type 1 and 2
			if (Random.value > 0.4f)
			{
				// Create an enemy of type 2 in the horizon
				EnemyCopy = (Transform)Instantiate(EnemyType1, new Vector3(Random.value * 10 - 5, 0, 50), Quaternion.Euler(0, 180, 0));

				// Give a random value to delay
				EnemyDelay = ((int)Mathf.Round(Random.value * 3)) + 4;
			}
			else
			{
				// Create an enemy of type 2
				// Randomize between right side and left side
				if (Random.value > 0.5f)
					EnemyCopy = (Transform)Instantiate(EnemyType2, new Vector3(25, 0, 20), Quaternion.Euler(0, Random.value * 40 + 240, 0));
				else
					EnemyCopy = (Transform)Instantiate(EnemyType2, new Vector3(-25, 0, 20), Quaternion.Euler(0, Random.value * -40 - 240, 0));

				// Give a random value to delay
				EnemyDelay = ((int)Mathf.Round(Random.value * 5)) + 5;
			}

			// Reduce 1 from enemy count
			EnemyCount--;
		}
	}

	/// <summary>
	/// Resets total score, loads end scene.
	/// </summary>
	/// <returns></returns>
	public IEnumerator Restart ()
	{
		// Wait a few seconds
		yield return StartCoroutine(WaitAndDelay(3.0f));

		// Put the score in the totalscore game object, so we use it in the next scene
		GameObject.Find("TotalScore").GetComponent<TotalScore>().TotalScoreCount = Camera.main.GetComponent<GUInterface>().Score;

		// Make the total score object indestructible, so we can use it in the next scene
		DontDestroyOnLoad(GameObject.Find("TotalScore"));

		// Load the last scene
		Application.LoadLevel("End");
	}

	/// <summary>
	/// Stalls game instructions
	/// </summary>
	/// <param name="waitTime">time to wait in seconds.</param>
	/// <returns>IEnumerator</returns>
	public IEnumerator WaitAndDelay (float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
	}
}
