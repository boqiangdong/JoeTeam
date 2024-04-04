using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class CameraTest: MonoBehaviour{
 
    public Transform player;
 
    float distance_z;
    float distance_y;
    
    void Start(){
 
        distance_z = transform.position.z - player.position.z;
        distance_y = transform.position.y - player.position.y;
 
    }
 
    void LateUpdate(){
 
        transform.position = new Vector3(0,player.position.y+distance_y,player.position.z+distance_z);// player.position + distance;
 
    }
 
}