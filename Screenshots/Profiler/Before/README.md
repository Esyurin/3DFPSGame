# Before: Profiler 수치 (최적화 전, 2026-11-02)

원본: `ProfilerCaptures/3DFPSGame_2026-11-02_15-10-07.data` (352프레임, 에디터 Play 모드)
처음 10프레임(로딩)은 빼고 342프레임 평균. 에디터에서 잰 값이라 빌드보다 무겁다.

## 프레임 시간 / CPU / GPU
| 항목 | 평균 | 비고 |
|---|---|---|
| 프레임 시간 | 24.7 ms (약 40 FPS) | 중앙값 18.4 ms, 95% 38.2 ms |
| EditorLoop | 17.6 ms | 에디터 자체 비용. 게임과 무관 |
| PlayerLoop (게임 실제 비용) | 6.8 ms | CPU Main Thread Frame Time과 같음 |
| CPU Render Thread | 2.5 ms | |
| GPU Frame Time | 6.3 ms | 최대 25.6 ms |
| 렌더링 (DoRenderLoop) | 4.3 ms | Main Camera 1.7 ms, MinimapCamera 1.4 ms |
| 스크립트 Update 전체 | 0.31 ms | Zombie.Update 0.10 ms |
| Animators.Update | 0.54 ms | |
| Physics.Simulate | 0.14 ms | |

## 렌더링
| 항목 | 평균 | 최대 |
|---|---|---|
| Batches | 357 | 612 |
| SetPass Calls | 93 | 159 |
| Draw Calls | 668 | 983 |
| Triangles | 61만 | 494만 |
| Vertices | 56만 | 316만 |
| Shadow Casters | 518 | 719 |

## 메모리 (에디터 포함 값)
| 항목 | 평균 |
|---|---|
| Total Used | 6.1 GB |
| Texture | 773 MB |
| Mesh | 71 MB |
| Gfx Used | 820 MB |
| GC Used | 1.23 GB |
| GC Alloc / 프레임 | 평균 38 KB (최대 6.2 MB, 시작 직후) |

## Game 창 Stats (한 순간의 값)
Profiler 평균과 다른 순간에 찍었기 때문에 Batches 같은 값이 Profiler 평균과 다르다.

| 항목 | 값 |
|---|---|
| FPS | 73.3 (13.6 ms) |
| CPU main / render thread | 13.6 ms / 5.1 ms |
| Batches | 176 (Saved by batching 328) |
| SetPass Calls | 87 |
| Tris / Verts | 63.6만 / 57.1만 |
| Shadow Casters | 323 |
| Visible skinned meshes | 15 |
| Animation / Animator 재생 중 | 0 / 2 |
| 화면 | 1920x1080, 23.7 MB |
| 오디오 | Level -74.8 dB, Clipping 0%, DSP load 0.1%, Stream load 0% |

## 눈에 띄는 점
- 게임 쪽 CPU 비용(6.8 ms)보다 에디터 비용(17.6 ms)이 훨씬 크다. 최적화 비교는 PlayerLoop, GPU, Batches처럼 게임 쪽 수치로 한다.
- 미니맵 카메라가 메인 카메라와 거의 같은 렌더링 비용(1.4 ms)을 쓴다.
- 그림자를 드리우는 오브젝트가 평균 518개로 많다.
- 텍스처 메모리가 773 MB로 메시(71 MB)의 10배가 넘는다.
- 스크립트 Update는 0.3 ms로 작아서 CPU 스크립트 최적화 효과는 작게 나올 가능성이 크다.
