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
                    
                    MemoryStream memBuf = new MemoryStream(data);
                    BinaryReader brln = new BinaryReader(memBuf, Encoding.Unicode);
                    CARD_INIT cardInit = new CARD_INIT();
                    brln.BaseStream.Position = 0;
                    brln.ReadByte();
                    cardInit.bAura = brln.ReadBoolean();
                    cardInit.frameName = brln.ReadString();
                    cardInit.imgName = brln.ReadString();
                    cardInit.u1Count = brln.ReadByte();
                    //cardInit.callback
                    //SimplePhotonChat simplePhotonChat = new SimplePhotonChat();
                    //OnResponse _callback = OnResponse.CreateDelegate(ERROR_ID.NONE, simplePhotonChat, brln.ReadString());

                    //cardInit.callback = 
                    //(CARD_INIT)operationResponse.Parameters[operationResponse.OperationCode];
                    //Debug.Log(cardInit.callback.Method.ToString());
                    //InitHandler.InitCard((ERROR_ID)operationResponse.ReturnCode, cardInit.callback);
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

            }
            callback(err);
            return err;
        }
    }
}
