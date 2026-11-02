# Before: 조명·그래픽 품질 (실습 전, 2026-11-02)

GameScene, Play 모드, 1920x1080, 플레이어 시작 위치에서 4방향 촬영.
좀비는 촬영 중 공격하지 않도록 멈춰 둠(씬 저장 안 함).

| 파일 | 방향 |
|---|---|
| 01_GameView_Front.png | 정면 (yaw 0) |
| 02_GameView_Right.png | 오른쪽 (yaw 90) |
| 03_GameView_Left.png | 왼쪽 (yaw 270) |
| 04_GameView_Back.png | 뒤 (yaw 180) |

## 당시 설정
- 렌더 파이프라인: URP `PC_RPAsset`, 품질 레벨 `PC`
- 환경광: Trilight, 강도 1 / 스카이박스: Default-Skybox / 안개: 켜짐
- 라이트: Directional Light 2개 (Realtime 0.35, Mixed 1.5, 둘 다 Soft Shadow)
- 라이트맵: 0개 (굽지 않음) / 라이트 프로브: 0개
- 포스트 프로세싱 (Global Volume `SampleSceneProfile`): Tonemapping, Bloom, Vignette, Motion Blur(꺼짐)
- Ambient Occlusion, Color Adjustments: 없음
