using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHorse : MonoBehaviour
{
    public Transform player;
    public List<Horse> prefabHorses;
    public float timeGap = 3;
    public float spawnDistance = 30;
    void Start()
    {
        StartCoroutine(GreateHorse());
    }

    IEnumerator GreateHorse()
    {
        while(true)
        {
            int i = Random.Range(0, prefabHorses.Count);
            Horse prefab = prefabHorses[i];

            Vector3 pos = new Vector3(Random.Range(-22f, 6f), 3.3f, spawnDistance);
            pos.z += player.position.z;

             Horse horse = Instantiate(prefab, pos, Quaternion.identity);
             yield return new WaitForSeconds(timeGap);
        }
    }
    
}
