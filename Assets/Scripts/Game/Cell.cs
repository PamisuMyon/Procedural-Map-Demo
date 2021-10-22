using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Joint
{
    public enum Type
    {
        Up, Right, Down, Left
    }

    public Type type;
    public Transform transform;
    public bool isUsed;

    public Type GetLocalLeft()
    {
        int value = (int) type;
        int left = (value + 3) % 4;
        return (Type) left;
    }

    public Type GetLocalRight()
    {
        int value = (int) type;
        int right = (value + 1) % 4;
        return (Type) right;
    }
}

public class Cell : MonoBehaviour 
{
    public enum Type 
    {
        Room, Corridor, Corner, TShaped, Hall
    }

    public Type type;
    public Joint[] joints;
    public Transform[] spawnPoints;

    public void GetAvailableJoints(List<Joint> results)
    {
        results.Clear();
        foreach (var item in joints)
        {
            if (!item.isUsed)
                results.Add(item);
        }
    }

    public int GetAvailableJointsCount()
    {
        int count = 0;
        foreach (var item in joints)
        {
            if (!item.isUsed)
                count++;
        }
        return count;
    }

    public Joint GetJoint(Joint.Type jointType)
    {
        foreach (var item in joints)
        {
            if (item.type == jointType)
            {
                return item;
            }
        }
        return null;
    }

    public bool HasJoint(Joint.Type jointType)
    {
        return GetJoint(jointType) != null;
    }

    public bool IsJointAvailable(Joint.Type jointType)
    {
        var joint = GetJoint(jointType);
        return joint != null && !joint.isUsed;
    }

}