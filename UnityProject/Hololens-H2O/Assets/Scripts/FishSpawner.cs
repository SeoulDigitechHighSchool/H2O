using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour {
    public GameObject nomalFish;

    public int nomalFishCount = 100;
    public int spawnerRange = 50;
    public int spawnerRangeHeight = 25;

    private List<Vector3> FishVectors = new List<Vector3>();


    private void Start()
    {
        SpawnNomalFish();
    }

    public void SpawnNomalFish()
    {
        for(int i=0; i < nomalFishCount; i++)
        {
            Vector3 vector = new Vector3(getRandomFloat(), (Random.value * (spawnerRangeHeight + 3)) - 2.5f, getRandomFloat());
            Quaternion quaternion = Quaternion.Euler(0, Random.value * 360, 0);
            if (FishVectors.Contains(vector))
            {
                i -= 1;
            }
            else
            {
                Instantiate(nomalFish, vector, quaternion);
            }
        }
    }

    private float getRandomFloat()
    {
        return Random.Range(spawnerRange * -0.5f, spawnerRange * 0.5f);
    }
}
