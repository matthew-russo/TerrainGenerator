using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveDataModel
{
    public string name;
    public SerializableVector3 playerPosition;
    public float xSeed;
    public float ySeed;

    public SaveDataModel(string n, SerializableVector3 playerPos, float xS, float yS)
    {
        name = n;
        playerPosition = playerPos;
        xSeed = xS;
        ySeed = yS;
    }
}

[Serializable]
public struct SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(float inputX, float inputY, float inputZ)
    {
        x = inputX;
        y = inputY;
        z = inputZ;
    }

    public SerializableVector3(Vector3 vector3ToCopy)
    {
        x = vector3ToCopy.x;
        y = vector3ToCopy.y;
        z = vector3ToCopy.z;
    }
}
