using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the bullet and it's collision with other objects.
/// </summary>
public class Missile : MonoBehaviour
{
	/*
    public Transform hitEffect; //the effect to be displayed when this object hits an enemy
    public AudioClip FlyBySound; //The sound of the bullet when it passes by the player, without hitting it
    public AudioClip HitSound; //The sound of the bullet when it hits an object

    private string BulletSource; //The source from which the bullet was shot
    private GameObject Player; //Holds the player object
    private bool FlyBy = false; //Checks if the bullet passed by the player
    private Object Target; //The object that the missile should chase
    private float TargetDistanceMax; //Holds the maximum distance between the missile and the target

    void Start()
    {
        Player = GameObject.Find("Player"); //Set the player object

        //Destroy the bullet object a few seconds after it is created
        Destroy(gameObject, 4);

        //Put all enemies currently on screen in an array
        GameObject[] EnemiesArray = GameObject.FindGameObjectsWithTag("Enemy");

        //Choose a random enemy object as the target for the missile
        Target = EnemiesArray[(int)Mathf.Round(Random.value * (EnemiesArray.Length - 1))];

        //Set the maximum distance between the missile and the target
        TargetDistanceMax = Vector3.Distance(transform.position, (Target as GameObject).transform.position);

        //If the target still exists, look at it
        if (Target)
        {
            transform.LookAt((Target as GameObject).transform.position);
        }
    }

    void Update()
    {
        //If the target still exists, follow it
        if (Target)
        {
            if ((Target as GameObject).transform.name == "EnemyPlane(Clone)")
            {
                if ((Target as GameObject).GetComponent<EnemyPlane>().Health > 0)
                {
                    transform.position = new Vector3((transform.position.x - (Target as GameObject).transform.position.x) * 0.05f,
                                                     (transform.position.y - (Target as GameObject).transform.position.y) * 0.05f,
                                                     (transform.position.z - (Target as GameObject).transform.position.z) * 0.05f);
                }
                else
                {
                    transform.Translate(Vector3.forward, Space.Self);
                }
            }
            else if ((Target as GameObject).transform.name == "EnemyZeppelin(Clone)")
            {
                if ((Target as GameObject).GetComponent<EnemyZeppelin>().Health > 0)
                {
                    transform.position = new Vector3((transform.position.x - (Target as GameObject).transform.position.x) * 0.05f,
                                                     (transform.position.y - (Target as GameObject).transform.position.y) * 0.05f,
                                                     (transform.position.z - (Target as GameObject).transform.position.z) * 0.05f);
                }
                else
                {
                    transform.Translate(Vector3.forward, Space.Self);
                }
            }

        }
        else
        {
            transform.Translate(Vector3.forward, Space.Self);
        }

        //Rotate the missile around the origin position
        transform.Rotate(Vector3.forward, 10, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        //print("hit something");
        ContactPoint contact = collision.contacts[0]; //the contact point of the object and the collider
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal); //set the rotation
        Vector3 pos = contact.point; //holds the contact point

        //Play a hit sound
        audio.PlayOneShot(HitSound);

        //If we hit either an enemy or the player do the following...
        if (collision.collider.tag == "Enemy" || collision.collider.tag == "Player")
        {
            //print("hit enemy");

            if (hitEffect) Instantiate(hitEffect, pos, rot); //create an explosionObject

            Destroy(gameObject); //remove the object from the scene

            //If we hit a Zeppelin, reduce its health, and add to the game's score
            if (collision.transform.GetComponent<EnemyZeppelin>())
            {
                //Decrease the value of the player's health
                collision.transform.GetComponent<EnemyZeppelin>().Health -= 20;

                //Add to the player's score
                Camera.main.GetComponent<GUInterface>().Score += 200;
            }

            //If we hit an Enemy Plane, reduce its health, and add to the game's score
            if (collision.transform.GetComponent<EnemyPlane>())
            {
                //Decrease the value of the player's health			
                collision.transform.GetComponent<EnemyPlane>().Health -= 20;

                //Add to the player's score
                Camera.main.GetComponent<GUInterface>().Score += 400;
            }
        }
    }
    */
}
