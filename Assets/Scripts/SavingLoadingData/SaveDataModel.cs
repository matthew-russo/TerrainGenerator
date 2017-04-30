using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class I used to save data that has fields for all the data I need
/// I was having issues with serializing Vector3's so I just made a container struct to hold the info.
/// </summary>

[Serializable]
public class SaveDataModel
{
    public string name; //the name of the save that the player gives it in the input field
    public SerializableVector3 playerPosition;
    public float xSeed; // x value of the random seed
    public float ySeed; // y value of the random seed

    // Standard constructor
    //
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

    // Stardard constructor
    //
    public SerializableVector3(float inputX, float inputY, float inputZ)
    {
        x = inputX;
        y = inputY;
        z = inputZ;
    }

    // Copy Constructor -- Vector 3
    //
    public SerializableVector3(Vector3 vector3ToCopy)
    {
        x = vector3ToCopy.x;
        y = vector3ToCopy.y;
        z = vector3ToCopy.z;
    }
}
