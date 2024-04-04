using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public List<Transform> prefabFloor;
    public List<Transform> floors;

    void Start()
    {
        
    }

   
    void Update()
    {
        CreateDestroyFloor();
    }

    void CreateDestroyFloor()
    {
        Transform lastFloor = floors[floors.Count - 1];
       
        if (lastFloor.position.z < transform.position.z + 50)
        {
            Transform prefab = prefabFloor[Random.Range(0, prefabFloor.Count)];
            Transform newFloor = Instantiate(prefab, null);
            newFloor.position = lastFloor.position + new Vector3(0, 0, 50);
            floors.Add(newFloor);
        }

        
        Transform firstFloor = floors[0];
        if(firstFloor.position.z < transform.position.z -50)
        {
            floors.RemoveAt(0);
            Destroy(firstFloor.gameObject);
        }
    }
}
