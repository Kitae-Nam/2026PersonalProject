using System.Collections.Generic;
using UnityEngine;

public class ObjPositionManager : MonoSingleton<ObjPositionManager>
{
    public List<Transform> _itemObjPosition = new List<Transform>();
    public List<Transform> _harvestableObjPosition = new List<Transform>();

    public void AddItemPosition(Transform position)
    {
        _itemObjPosition.Add(position);
    }
    public void AddHavaObjPosition(Transform position)
    {
        _harvestableObjPosition.Add(position);
    }
    public Transform GetNearestItemPosition(Vector3 position, float radius)
    {
        _itemObjPosition.RemoveAll(t => t == null);

        float nearestDistance = float.MaxValue;
        Transform nearestOne = null;
        bool isFound = false;

        float radiusSqr = radius * radius;

        foreach (var obj in _itemObjPosition)
        {
            float distance = (obj.position - position).sqrMagnitude;

            if (distance <= radiusSqr)
            {
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestOne = obj;
                    isFound = true;
                }

            }
        }

        return isFound ? nearestOne : null;
    }
    public Transform GetNearestHavaPosition(Vector3 position, float radius)
    {
        _harvestableObjPosition.RemoveAll(t => t == null);

        float nearestDistance = float.MaxValue;
        Transform nearestOne = null;
        bool isFound = false;

        float radiusSqr = radius * radius;

        foreach (var obj in _harvestableObjPosition)
        {
            float distance = (obj.position - position).sqrMagnitude;

            if (distance <= radiusSqr)
            {
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestOne = obj;
                    isFound = true;
                }

            }
        }

        return isFound ? nearestOne : null;
    }
}
