using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemSO itemSO;
    public bool isCanHold = true;
    public GameObject itemGO => gameObject;
}
