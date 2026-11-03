namespace Game
{
    public enum GameState
    {
        Ready, // 준비: 시작 연출 중. 아무도 움직이지 않는다.
        Playing, // 진행: 플레이 중
        Pause, // 일시정지: 게임 시간과 입력이 멈춘다.
        GameOver, // 게임 오버: 모두 멈춘다.
    }
}
