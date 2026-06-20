using System.Collections.Generic;
using _01_Script.Pool;
using UnityEngine;

namespace _01_Script.Map
{
    public class Grass : MonoBehaviour, IPoolable
    {
        [SerializeField] private List<GameObject> objects;
        public void OnPop()
        {
            int ran = Random.Range(0, objects.Count);
            for (int i = 0; i < objects.Count; i++)
            {
                if(i == ran)
                    objects[ran].SetActive(true);
                else
                    objects[i].SetActive(false);
            }
        }

        public void OnPush()
        {
        }
    }
}