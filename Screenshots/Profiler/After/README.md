# After: 조명 적용 후 Profiler 수치 (2026-11-02)

원본: `ProfilerCaptures/3DFPSGame_2026-11-02_AfterLighting.data` (524프레임, 에디터 Play 모드)
Before와 같은 구간(프레임 10~351, 342프레임)의 평균으로 비교.
좀비는 멈추지 않고 평소처럼 플레이되게 두고 녹화. Before와 After 모두 Profiler 창이 열린 상태였다.

## Before / After 비교
| 항목 | Before | After | 변화 |
|---|---|---|---|
| 프레임 시간 | 24.7 ms | 12.5 ms | 에디터 비용 차이가 커서 비교에 쓰지 않음 (아래 참고) |
| EditorLoop (에디터 비용) | 17.6 ms | 7.2 ms | 에디터 상태 차이. 조명과 무관 |
| **PlayerLoop (게임 CPU)** | 6.8 ms | 5.0 ms | -26% |
| CPU Render Thread | 2.5 ms | 2.0 ms | -20% |
| **GPU Frame Time** | 6.3 ms | 4.1 ms | -35% |
| Main Camera 렌더링 | 1.7 ms | 1.5 ms | -10% |
| MinimapCamera 렌더링 | 1.4 ms | 1.3 ms | 거의 같음 |
| **Batches** | 357 | 161 | -55% |
| SetPass Calls | 93 | 87 | -6% |
| Draw Calls | 668 | 487 | -27% |
| Triangles | 61만 | 66만 | +7% |
| Shadow Casters | 518 | 590 | +14% |
| **Texture Memory** | 773 MB | 805 MB | +32 MB |
| Mesh Memory | 71 MB | 71 MB | 같음 |
| GC Alloc / 프레임 | 38 KB | 18 KB | 플레이 상황 차이 |

## Game 창 Stats 비교 (같은 조건, 한 순간의 값)
| 항목 | Before | After | 변화 |
|---|---|---|---|
| FPS | 73.3 (13.6 ms) | 74.2 (13.5 ms) | 거의 같음 |
| CPU main / render thread | 13.6 / 5.1 ms | 13.5 / 5.1 ms | 같음 |
| Batches | 176 | 179 | 같음 |
| Saved by batching | 328 | 607 | +85% |
| SetPass Calls | 87 | 88 | 같음 |
| Tris / Verts | 63.6만 / 57.1만 | 67.0만 / 63.3만 | +5% / +11% |
| **Shadow Casters** | 323 | 610 | **+89%** |
| Visible skinned meshes | 15 | 15 | 같음 |
| Animator 재생 중 | 2 | 2 | 같음 |
| 오디오 | -74.8 dB, DSP 0.1% | -74.8 dB, DSP 0.1% | 같음 |

같은 조건에서 보면 **Batches는 그대로**다. Profiler 평균의 Batches -55%는 조명 효과라기보다 녹화 조건(보인 좀비 수, 시선 방향) 차이가 컸다고 봐야 한다.
Shadow Casters가 거의 두 배인 것은 해를 낮춰 그림자가 길어진 영향이다. 그림자 패스가 더 많이 그려서 Saved by batching(그림자 패스에서 묶인 수 포함)도 같이 늘었다.

## 해석
- **GPU가 35% 가벼워졌다.** 실시간 그림자를 드리우던 보조광이 Baked로 바뀌었고, 간접광·보조광·작업등이 라이트맵에서 텍스처로 읽히기 때문이다. 작업등 10개는 실행 중 비용이 없다.
- **Batches와 Draw Calls 감소는 조명 효과로 보기 어렵다.** 같은 조건의 Stats에서는 Batches가 176 → 179로 그대로였다. Profiler 평균의 차이는 녹화 조건(화면에 보인 좀비 수, 시선 방향) 차이로 본다.
- **Shadow Casters가 늘었다.** 해의 높이를 42°에서 20°로 낮춰 그림자가 길어졌고, 그림자 범위에 들어오는 물체가 늘었다.
- **텍스처 메모리가 32 MB 늘었다.** 라이트맵(2048, 16 MB), Shadowmask, Reflection Probe 2장이다. 조명 품질을 얻고 메모리를 조금 쓴 셈이다.
- 프레임 시간 전체(24.7 → 12.5 ms)는 대부분 에디터 비용 차이라서, 조명 효과로 보면 안 된다. 비교는 PlayerLoop, GPU, Batches처럼 게임 쪽 수치로 한다.
