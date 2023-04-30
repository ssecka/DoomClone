using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject AI;
    public float waveDelay;
    public float enemiesPerWave;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 2 ; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
            Instantiate(AI, randomPosition, Quaternion.identity);
        }
        StartCoroutine(InstantiatePrefabs());
    }

    IEnumerator InstantiatePrefabs()
    {
        while (true)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-10f, 10f));
                Instantiate(AI, randomPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(waveDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
