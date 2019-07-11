using ExitGames.Client.Photon;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Server
{
    public class Handler : Singleton<Handler>
    {
        private static byte objToByte(object obj)
        {
            byte u1Byte = 0;
            u1Byte = (byte)obj;

            return u1Byte;
        }
        private static Byte[] objToDictionary(object obj)
        {
            byte[] b = (byte[])obj;
            return b;
        }
        public void HandleResponse(OperationResponse operationResponse)
        {
            switch(operationResponse.OperationCode)
            {
                case (Byte)MSGs.CREATE_CARD:
                    byte[] data;
                    object obj = operationResponse.Parameters[operationResponse.OperationCode];

                    IDictionary idict = (IDictionary)obj;
                    Dictionary<byte, byte[]> newDict = new Dictionary<byte, byte[]>();
                    byte u1Key = 0;
                    if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
                    {
                        foreach (object key in idict.Keys)
                        {
                            newDict.Add(objToByte(key), objToDictionary(idict[key]));
                            u1Key = (byte)key;
                        }
                    }
                    data = newDict[u1Key];
                    UInt16 u2SeqNo = 0;
                    MemoryStream memBuf = new MemoryStream(data);
                    BinaryReader brln = new BinaryReader(memBuf, Encoding.Unicode);
                    CARD_INIT cardInit = new CARD_INIT();
                    brln.BaseStream.Position = 0;
                    brln.ReadByte();
                    u2SeqNo = brln.ReadUInt16();
                    cardInit.bAura = brln.ReadBoolean();
                    cardInit.frameName = brln.ReadString();
                    cardInit.imgName = brln.ReadString();
                    cardInit.u1Count = brln.ReadByte();

                    if(ServerMgr.Instance.GetRequestQueue().Peek().u2SeqNo.Equals(u2SeqNo))
                    {
                        InitHandler.InitCard(ERROR_ID.NONE, ServerMgr.Instance.GetRequestQueue().Peek().callback);
                    }
                    break;
            }
        }
    }

    public class InitHandler
    {
        public static ERROR_ID InitCard(ERROR_ID err, OnResponse callback)
        {
            if(err == ERROR_ID.REQUEST_DUPLICATION)
            {
                err = ERROR_ID.NONE;
            }
            if(err.Equals(ERROR_ID.NONE))
            {
                ServerMgr.Instance.GetRequestQueue().Dequeue();
            }
            callback(err);
            return err;
        }
    }
}
