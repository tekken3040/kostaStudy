using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public struct VariableRange
{
    public Double fMin;
    public Double fMax;

	public void Set(string[] data, ref UInt16 idx)
	{
		fMin = Convert.ToDouble(data[idx++]);
		fMax = Convert.ToDouble(data[idx++]);
	}
	public float Random
	{
		get
		{
			return UnityEngine.Random.Range((float)fMin, (float)fMax);
		}
	}
}