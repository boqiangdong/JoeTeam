using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
/// <summary>
/// This script controls the player's status, movement 
/// </summary>
public class PlayerController : MonoBehaviour,IPointerClickHandler
{

	public float Speed= 0.05f; 	// Your speed
	public float MovementLimit 		= 5f; 	// Horizontal movement limit of the player
	private Vector3 	PlayerOriginPos;
    private Vector3 	PlayerStartPos;								// The original position of the player
	private float 		EaseControlFromIntro 	= 0; 			// Used to ease the transition from the Intro to the player-controlled game.
    private int status; //0 stop 1 start 2 fly
	public Rigidbody rd;

    public void OnPointerClick(PointerEventData pointerEventData)
    {   
		//必须有collider：由鼠标点击处发射线，与游戏物体发生碰撞，碰撞到的物体，就是你点击到的物体。
		startFly();
    }
	private void startFly(){
		if(this.status==0){
			this.status = 1;
		}
	}
	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start ()
	{
        status = 0;
		PlayerOriginPos = transform.position; // Hold the original position of the player, so we can make the payer appear from off screen and eas into position
        PlayerStartPos = new Vector3(0f,20f,3.8f);
    }

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update ()
	{	
        //start fly
        if(status==1){
            // Move towards the origin position
			transform.position -= (transform.position - PlayerStartPos) * 0.1f;
			// Tilt based on your position, and also keep a looping a slight tilt regardless of your position
			transform.Rotate(Vector3.forward, transform.position.x * 1.48f, Space.Self);
			if (Vector3.Distance(transform.position, PlayerStartPos) < 3)
			{
				status = 2;
			}
        }else if(status==2){
            transform.position += new Vector3(0,0,1f);
			Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
			Vector3 m_MousePos = new Vector3(Input.mousePosition.x, pos.y, pos.z);
			transform.position = Camera.main.ScreenToWorldPoint(m_MousePos);
			if (transform.position.x > PlayerOriginPos.x + MovementLimit)
			{
				transform.position = new Vector3(PlayerOriginPos.x + MovementLimit, transform.position.y, transform.position.z);
			}
			if (transform.position.x < PlayerOriginPos.x - MovementLimit)
			{
				transform.position = new Vector3(PlayerOriginPos.x - MovementLimit, transform.position.y, transform.position.z);
			}
			transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, ((Input.mousePosition.x - Screen.width * 0.5f) * 0.0006f + (Mathf.Sin((Time.time) * 3)) * 0.02f) * EaseControlFromIntro, transform.rotation.w);
			if (EaseControlFromIntro < 1)
				EaseControlFromIntro += 0.01f;
		}
	}


	/// <summary>
	/// Stalls game instructions
	/// </summary>
	/// <param name="waitTime">time to wait in seconds.</param>
	/// <returns>IEnumerator</returns>
	IEnumerator WaitAndDelay (float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
	}
}

