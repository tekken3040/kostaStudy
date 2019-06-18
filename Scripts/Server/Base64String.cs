using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using System.IO;
using System.Security.Cryptography;


namespace Server
{
	public static class Base64String
	{
		/// <summary>
		/// 문자열을 Byte Array로 만든다
		/// </summary>
		public static byte[] ToByteArray(string text)
		{
			return System.Convert.FromBase64String(text);
		}

		/// <summary>
		/// ByteArray를 문자열로 만든다
		/// </summary>
		public static string ToBase64String(byte[] byteArray, Int32 length)
		{
			return System.Convert.ToBase64String(byteArray, 0, length);
		}


	}
}
