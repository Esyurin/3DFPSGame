# 포스트 프로세싱 적용 (2026-11-02)

조명 After(`Screenshots/After`)와 같은 조건(GameScene, Play 모드, 시작 위치, 1920x1080, 좀비 멈춤)으로 촬영.
비교 순서: `Screenshots/Before` → `Screenshots/After`(조명) → `Screenshots/PostFX`(조명 + 포스트 프로세싱)

## 콘셉트
해 질 녘의 버려진 산업 지대. 영화 같은 대비, 차가운 그늘과 따뜻한 햇빛, 살짝 바랜 색, 거친 질감.

## 설정
Volume Profile: `Assets/Settings/GameScenePostFX.asset` (기존 `SampleSceneProfile`을 복사해 GameScene 전용으로 만듦. 원본은 그대로)

| 효과 | Before | After | 의도 |
|---|---|---|---|
| Tonemapping | Neutral | **ACES** | 영화 같은 대비와 밝은 부분의 부드러운 롤오프 |
| Bloom | 강도 0.25, Scatter 0.5 | **강도 0.6**, Threshold 1, Scatter 0.65, 주황빛 Tint | 총구 화염·밝은 하늘이 은은하게 번짐 |
| Color Adjustments | 없음 | **Post Exposure +0.65**, Contrast +8, Saturation -12, 따뜻한 Color Filter | ACES로 어두워진 만큼 보정, 색을 살짝 바래게 |
| Shadows Midtones Highlights | 없음 | **그늘은 푸르게, 밝은 곳은 주황으로** | 해 질 녘의 색 대비 강조 |
| Vignette | 강도 0.2 | **강도 0.26**, Smoothness 0.45 | 화면 가장자리를 눌러 시선을 가운데로 |
| Film Grain | 없음 | **Thin1, 강도 0.2** | 거친 공포 분위기 |
| Chromatic Aberration | 없음 | **0.08** | 렌즈 느낌 (아주 약하게) |
| Ambient Occlusion (SSAO, `PC_Renderer`) | 강도 0.4, 반경 0.3 | **강도 0.8**, 반경 0.35, Falloff 60 | 모서리·바닥 접점의 그늘을 진하게. 움직이는 좀비에도 적용 |
| Motion Blur | 꺼짐 | 꺼짐 (유지) | FPS에서 어지러움 방지 |

## 조정 기록
- 처음 Exposure +0.25, Contrast 12, Vignette 0.32로 넣었더니 ACES 때문에 바닥이 거의 검게 나왔다. Exposure를 +0.65로 올리고 Contrast 8, Vignette 0.26으로 낮췄다.

## 성능 비용 (Profiler)

### 1) 조명 After와 같은 방식으로 녹화 (좀비 움직임, 프레임 10~351 평균)
원본: `ProfilerCaptures/3DFPSGame_2026-11-02_AfterPostFX.data`

| 항목 | 조명 After | PostFX After |
|---|---|---|
| PlayerLoop (게임 CPU) | 5.0 ms | 4.8 ms |
| CPU Render Thread | 2.0 ms | 1.9 ms |
| GPU Frame Time | 4.1 ms | 4.9 ms |
| Batches / SetPass | 161 / 87 | 151 / 87 |
| Texture Memory | 805 MB | 805 MB |
| SSAO / Bloom CPU 비용 | 0.04 / 0.04 ms | 0.04 / 0.04 ms |

GPU가 0.8 ms 늘어 보이지만, 녹화마다 좀비 위치와 시선이 달라서 효과만의 비용으로 보기 어렵다. 그래서 아래처럼 다시 쟀다.

### 2) 같은 Play 세션에서 효과만 켜고 끈 측정 (좀비 멈춤, 시선 고정)
원본: `ProfilerCaptures/3DFPSGame_2026-11-02_PostFX_AB.data`, `..._PostFX_CD.data`

| 구성 | GPU 평균 | Batches |
|---|---|---|
| A. 전부 켬 (Volume + SSAO) | 5.25 ms | 150 |
| B. Volume 끔, SSAO 켬 | 5.20 ms | 150 |
| C. 전부 끔 | 5.15 ms | 115 |
| D. Volume 켬, SSAO 끔 | 5.03 ms | 115 |

- **Volume 효과(ACES, Bloom, 색 보정, Vignette, Film Grain, Chromatic Aberration)**: 같은 세션의 A-B, D-C 차이가 ±0.1 ms로 측정 오차 안이다. 사실상 비용이 없다.
- **SSAO**: Batches가 **+35**(깊이·노멀 사전 패스), GPU는 약 **+0.1~0.2 ms**. 효과 중 가장 비싸다.
- **메모리**: 변화 없음 (805 MB).
- 전체적으로 포스트 프로세싱 비용은 GPU 5 ms 중 **5% 이하**다. 화면 분위기가 크게 바뀐 데 비해 싸다.
- 측정 중 주의: Play 모드에서 SSAO 렌더러 기능을 끄면 렌더러가 다시 만들어지며 한 프레임이 12.5초 멈췄다. 그래서 C·D는 SSAO를 미리 끈 새 세션에서 쟀다.

### 3) Game 창 Stats (같은 조건, 한 순간의 값)
| 항목 | Before (조명 전) | 조명 After | PostFX After |
|---|---|---|---|
| FPS | 73.3 (13.6 ms) | 74.2 (13.5 ms) | 65.5 (15.3 ms) |
| CPU main / render thread | 13.6 / 5.1 ms | 13.5 / 5.1 ms | 15.3 / 5.6 ms |
| Batches | 176 | 179 | 151 |
| Saved by batching | 328 | 607 | 599 |
| SetPass Calls | 87 | 88 | 87 |
| Tris / Verts | 63.6만 / 57.1만 | 67.0만 / 63.3만 | 66.0만 / 62.4만 |
| Shadow Casters | 323 | 610 | 582 |
| Visible skinned meshes | 15 | 15 | 15 |
| Animator 재생 중 | 2 | 2 | 2 |

- **FPS가 74 → 66으로 떨어졌다(+1.8 ms).** 다만 2번(같은 세션에서 효과를 켜고 끈 측정)에서 효과 자체의 비용은 GPU 0.1~0.2 ms였다. 나머지는 에디터 프레임 시간이 흔들린 몫으로 본다(같은 PostFX 상태에서 86 FPS가 찍힌 적도 있다).
- **render thread +0.5 ms**: Bloom·색 보정·Film Grain 등 포스트 패스를 준비하는 비용이 일부 들어 있다.
- **Batches 179 → 151**: 포스트 프로세싱은 패스를 더하는 쪽이라 Batches를 줄일 수 없다. Stats는 한 순간의 값이라 그 순간 그려진 물체에 따라 흔들린 것으로 본다. 삼각형·SetPass는 거의 같다.
- 효과의 정확한 비용은 Stats보다 2번 측정을 기준으로 한다.

## 남은 확인
- 원래대로 되돌리려면 `Global Volume`의 Profile을 `SampleSceneProfile`로 바꾸면 된다(SSAO 값은 `PC_Renderer`에서 따로 되돌림).
