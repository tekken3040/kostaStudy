using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public class SocketInfo
{
	public UInt16 u2ID;
	public string sSocBone;

	public Vector3 fSocLoc;

	public Vector3 fSocRot;

	public UInt16 SetInfo(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment
		sSocBone = cols[idx++];

		fSocLoc.x = (float)Convert.ToDouble(cols[idx++]);
		fSocLoc.y = (float)Convert.ToDouble(cols[idx++]);
		fSocLoc.z = (float)Convert.ToDouble(cols[idx++]);
		fSocRot.x = (float)Convert.ToDouble(cols[idx++]);
		fSocRot.y = (float)Convert.ToDouble(cols[idx++]);
		fSocRot.z = (float)Convert.ToDouble(cols[idx++]);

		return u2ID;
	}
}

