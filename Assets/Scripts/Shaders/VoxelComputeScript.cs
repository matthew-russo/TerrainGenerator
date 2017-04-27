using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelComputeScript : MonoBehaviour
{
    public ComputeShader computeShader;

    public ComputeBuffer computeBuffer;


	void Start ()
	{
	    int index = computeShader.FindKernel("CSMain");
		computeShader.Dispatch(index,1,1,1);
	}
	
	void Update () {
		
	}
}
