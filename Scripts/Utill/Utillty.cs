using UnityEngine;
using System.Collections;

public static class Utillity{    

    // 색상 값 -> 16진수
	public static string ConversionToHex(Color32 color)
	{       
		string hex = string.Format ("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
		return hex;
	}
    
    // TimeScale에 영향을 받지 않음  
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        
        while(Time.realtimeSinceStartup < start + time)
            yield return null;
    }

    public static ushort BitMaskType2(int bit)
    {
        switch (bit)
        {
            case 1: return 0x0001;
            case 2: return 0x0002;
            case 3: return 0x0004;
            case 4: return 0x0008;
            case 5: return 0x0010;
            case 6: return 0x0020;
            case 7: return 0x0040;
            case 8: return 0x0080;
            case 9: return 0x0100;
            case 10: return 0x0200;
            case 11: return 0x0400;
            case 12: return 0x0800;
        }
        return 0x00;
    }
}
