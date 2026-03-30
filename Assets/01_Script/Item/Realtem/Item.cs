using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemSO itemSO;
    public GameObject itemGO => gameObject;
}
