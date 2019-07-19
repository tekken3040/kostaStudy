using System.Collections;

public class Defines
{
    // 액션 타입 enum
    public enum ACTION_TYPE
    {
        ATTACK = 0,                             // 공격
        GUARD,                                  // 가드
        GUARD_BREAK,                            // 가드 브레이크
        HEAVY_ATTACK,                           // 필살기(강공격)
        UNKNOWN,                                // 미확인(플레이어 : 아직 선택하지 않음/ 적: 알수없음)
    }

    // 플레이어와 적 구분 enum
    public enum TARGET
    {
        NONE = 0,
        PLAYER,
        ENEMY,
    }

    public static byte MaxRound = 5;            // 최대 라운드 수
    public static byte RoundWin = 3;            // 승리에 필요한 라운드 수
    public static byte Action_Cnt = 5;          // 액션 슬롯 수
    public static byte MaxPowerStack = 3;       // 최대 파워 스택 수
}
