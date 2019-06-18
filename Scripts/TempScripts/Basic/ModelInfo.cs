using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public class ModelInfo
{
	// 현재 모델 Script정보에는 Class와 Pos(장착위치)에 대한 정보가 없기때문에,
	// 장비Script 읽어오면서 두 정보를 지정
	// EquipmentInfo의 SetInfo에서 사용
	// 반드시 모델Script를 먼저 세팅해야 ModelData를 지정해 줄 수 있음
	public struct ModelData
	{
		public UInt16 u2ClassID;
		public EquipmentInfo.EQUIPMENT_POS ePosID;
		public void SetData(UInt16 classID, EquipmentInfo.EQUIPMENT_POS posID)
		{
			try{
			this.u2ClassID = classID;
			this.ePosID = posID;
			}catch(Exception ex){
				DebugMgr.LogError(classID);
			}
		}
	}
	public ModelData cModelData;

	public const int MAX_MODEL_OF_EQUIP = 5;

	public UInt16 u2ID;
	public string sModelName;
	public string sModelPathname;
	public string sModelPathName2;
	public byte u1ModelCount;
	public EQUIP_MODEL[] acModels;
	public string sImagePath;
	public struct EQUIP_MODEL
	{
		public string sModelFilename;
		public string sTextureFilename;
		public SocketInfo cSocketInfo;

		public bool bIsViewModel;

		public Vector3 cViewScale;
		public Vector3 cViewPosition;
		public Quaternion cViewRotation;

		public void SetViewerModel(string[] cols, ref int idx)
		{
			bIsViewModel = true;
			idx++;
			cViewScale = new Vector3((float)Convert.ToDouble(cols[idx]),
			                         (float)Convert.ToDouble(cols[idx]),
			                         (float)Convert.ToDouble(cols[idx]));
			idx++;
			cViewRotation = new Quaternion();
//			DebugMgr.Log( "Rot : " + (float)Convert.ToDouble(cols[idx]) + "," + 
//			          (float)Convert.ToDouble(cols[idx+1]) + "," + 
//			          (float)Convert.ToDouble(cols[idx+2]) 
//			          ) ;
			cViewRotation.eulerAngles = new Vector3((float)Convert.ToDouble(cols[idx++]), 
			                                        (float)Convert.ToDouble(cols[idx++]), 
			                                        (float)Convert.ToDouble(cols[idx++]) );
//			cViewRotation.eulerAngles.Set( (float)Convert.ToDouble(cols[idx++]), 
//			                                   (float)Convert.ToDouble(cols[idx++]), 
//			                                   (float)Convert.ToDouble(cols[idx++]) 
//			                          ) ;
//			DebugMgr.Log("rota : " + cViewRotation.eulerAngles);
			cViewPosition = new Vector3( (float)Convert.ToDouble(cols[idx++]),
			                            (float)Convert.ToDouble(cols[idx++]),
			                            (float)Convert.ToDouble(cols[idx++]) );
		}
	}

	public ModelInfo()
	{
//		acModels = new EQUIP_MODEL[MAX_MODEL_OF_EQUIP];
	}
	public UInt16 SetInfo(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;	// comment

		sModelName = cols[idx++];
		sModelPathname = cols[idx++];
		sModelPathName2 = cols[idx++];
		idx++; // 장착 부위 현재 쓰이지 않음
		u1ModelCount = Convert.ToByte(cols[idx++]);
		acModels = new EQUIP_MODEL[u1ModelCount];
		for (Byte i = 0; i < u1ModelCount; i++)
		{
			acModels[i].sModelFilename = cols[idx++];
			UInt16 u2SocketID = Convert.ToUInt16(cols[idx++]);
			if (u2SocketID != 0)
				acModels[i].cSocketInfo = SocketInfoMgr.Instance.GetInfo(u2SocketID);
			else
				acModels[i].cSocketInfo = null;
		}
		idx=17;
		sImagePath = cols[idx++].Replace("/r", "");

		return u2ID;
	}
}
