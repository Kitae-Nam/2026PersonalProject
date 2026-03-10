using System.Collections.Generic;
using UnityEngine;

public class ObjPositionManager : MonoSingleton<ObjPositionManager>
{
    private List<Vector3> _objPositions = new List<Vector3>();

    public void AddObjPosition(Vector3 position)
    {
        _objPositions.Add(position);
    }
    public Vector3 GetNearestObjPosition(Vector3 position, float radius)
    {
        float nearestDistance = float.MaxValue;
        Vector3 nearestone = Vector3.zero;
        bool isFound = false;

        float radiusSqr = radius * radius;

        foreach (var obj in _objPositions)
        {
            float distance = (obj - position).sqrMagnitude;

            if (distance <= radiusSqr)
            {
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestone = obj;
                    isFound = true;
                }

            }
        }

        return isFound ? nearestone : Vector3.zero;
    }
}
