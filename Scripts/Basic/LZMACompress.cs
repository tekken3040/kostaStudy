using UnityEngine;
using System;
using System.IO;

public class LZMACompress{


    public static long CompressFileLZMA(string inFile, string outFile)
    {
        try
        {                   
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);
            
    //        DebugMgr.Log(input.Length);
            
            // Write the encoder properties
            coder.WriteCoderProperties(output);
            
            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);                

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);        
            
            long compressSize = output.Length;      
            
            output.Flush();
            output.Close();
            input.Close();        
        
            return compressSize;
        }
        catch (Exception e)
        {            
            DebugMgr.Log(e);
            return -1;
        }
    }
    
    public static void DecompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);
        
        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);
        
        // Read in the decompress file size.
        byte [] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        // Decompress the file.
        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();
        output.Close();
        input.Close();
    } 
    
    public static bool DecompressFileLZMA(byte[] inFile, string outFile)
    {
        try
        {              
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();                
            
            MemoryStream input = new MemoryStream(inFile);
            FileStream output = new FileStream(outFile, FileMode.Create);
            
            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);
            
            // Read in the decompress file size.
            byte [] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
            input.Close();
            return true;
        }
        catch (Exception e)
        {            
            DebugMgr.Log(e);
            return false;
        }        
    }     
}
