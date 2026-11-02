# After: 조명 적용 (2026-11-02)

Before와 같은 조건(GameScene, Play 모드, 시작 위치, 1920x1080, 좀비 멈춤)으로 촬영.

| 파일 | 방향 |
|---|---|
| 01_GameView_Front.png | 정면 (yaw 0) |
| 02_GameView_Right.png | 오른쪽 (yaw 90) |
| 03_GameView_Left.png | 왼쪽 (yaw 270) |
| 04_GameView_Back.png | 뒤 (yaw 180) |
| 05_GameView_WorkLight.png | 작업등(Baked) 근처, (16, 1, 2)에서 촬영 |

## 조명 콘셉트
해 질 녘 산업 지대. 낮게 깔린 주황빛 해와 푸른 안개·하늘의 대비.

## 바꾼 것
| 항목 | Before | After |
|---|---|---|
| 해 (`Map_v1/Light/Sun (Mixed)`) | Mixed, 흰색, 1.5, 높이 42° | **Mixed**, 주황 (1, 0.72, 0.48), 1.8, 높이 20° (긴 그림자) |
| 보조광 (`Sky Fill (Baked)`) | Realtime, 0.35, Soft Shadow | **Baked**, 푸른색, 0.45 (반대편에서 채움, 실시간 그림자 비용 없음) |
| 작업등 (`Lighting/WorkLights (Baked)`) | 없음 | **Baked** 주황 Spot 10개, 5 m 높이, 강도 30, 범위 16 |
| Lighting Mode | (설정 에셋 없음) | **Shadowmask**: 가까운 그림자는 실시간, 50 m 밖은 구운 그림자 |
| Lightmap | 0장 | 2048 1장 (+ Shadowmask 1장), 8 texel/m, Non-Directional, AO 켬, 2 bounce, GPU Lightmapper |
| Light Probe | 0개 | **1,088개** (`Lighting/LightProbeGroup`, 6 m 간격 x 높이 0.6/2.2/5 m, NavMesh 위) |
| Reflection Probe | 없음 | Baked 1개 (맵 전체, 128px) |
| 작은 소품 | 라이트맵 | 상자·팔레트·드럼통·배관·에어컨·발전기·철망 130여 개는 **Light Probe로 조명** (라이트맵 공간 절약) |

- 좀비, 플레이어, 바닥의 Drum/Box처럼 움직이는 물체는 Light Probe로 구운 조명(보조광, 작업등, 반사광)을 받는다.
- 총구 화염 Point Light는 그대로 Realtime.
- 설정 에셋: `Assets/01.Scenes/GameScene/GameSceneLighting.lighting`
- 굽기 시간: 약 21~33초.

## 성능 영향 (예상, Profiler로 확인 필요)
- 실시간 그림자를 드리우던 Directional Light가 2개에서 1개로 줄었다.
- 텍스처 메모리가 라이트맵 16 MB + Shadowmask 정도 늘었다.
