# 3. 프로젝트 최적화 (2026-11-02)

목표 기기: 중급 PC, 1080p 60fps. 조명·포스트 프로세싱 화질은 그대로 두고, 보이지 않는 낭비를 줄였다.
계획: 플랜 모드에서 CPU·GPU·RAM 병목을 조사한 뒤 효과가 클 항목만 골랐다.

## 조사로 찾은 병목
| 분류 | 찾은 것 |
|---|---|
| GPU / Draw Call | **좀비 한 마리가 부위별 SkinnedMeshRenderer 14개**로 그려진다. 부위마다 화면·그림자(4 cascade)·SSAO 사전 패스에 따로 그려진다. |
| GPU | 미니맵 카메라가 매 프레임 그림자·SSAO·깊이/불투명 텍스처까지 켜고 씬 전체를 그린다(렌더링 1.2 ms, 메인 카메라와 비슷). |
| GPU | 그림자 4 cascade·50 m, 쓰지 않는 추가 라이트 그림자 켜짐, 메인 카메라 Far 1000 m(안개 때문에 70 m 밖은 안 보임). |
| RAM | Synty 텍스처 4096² 5장 등. 쓰는 곳은 로우폴리 Drum·Box와 꺼진 TestTerrain뿐. 모델 29개 Read/Write 켜짐. 폭발음 PCM. |
| CPU | 스크립트는 이미 가볍다(Update 전체 0.15 ms). 남은 것은 쏠 때마다 피탄·피 이펙트 Instantiate/Destroy, 매번 `new WaitForSeconds`. |

## 적용한 것
| 분류 | 항목 | 내용 |
|---|---|---|
| GPU | 좀비 LOD Group | `Zombie.prefab`의 Model에 LODGroup. LOD0 = 기존 부위 14개(화면 높이 25% 이상), LOD1 = 한 덩어리 메시 `ZombieLOD.FBX`(2% 이상), 그보다 작으면 안 그림. |
| GPU | 미니맵 전용 렌더러 | `Assets/Settings/Minimap_Renderer.asset`(렌더러 기능 없음 → SSAO 없음). 미니맵 카메라 그림자·깊이/불투명 텍스처 끔, Far 30 m. |
| GPU | 그림자 | cascade 4 → 2, 거리 50 → 40 m(그 밖은 Shadowmask 구운 그림자), 추가 라이트 그림자 끔. |
| GPU | 메인 카메라 Far | 1000 → 150 m. |
| GPU | SSAO | Downsample 켬. |
| RAM | 텍스처 | Synty 텍스처 16장 Max Size 4096·2048 → 1024. |
| RAM | 모델 | FBX 29개 Read/Write 끔. |
| RAM | 오디오 | 폭발음 PCM → Vorbis(품질 70%). |
| CPU | 이펙트 풀링 | 새 `Weapon/EffectPool.cs`(프리팹별 ObjectPool, 피탄·피 이펙트 10개씩 미리 만듦, 최대 30). `PooledEffect`를 BombPool 전용에서 범용으로 바꿈. `PlayerGunFire`·`Zombie`의 Instantiate/Destroy를 풀로 교체. |
| CPU | WaitForSeconds 캐싱 | `PlayerGunFire`의 불꽃·레이저·재장전 대기 객체를 한 번만 만든다. |

## 결과 1: 좀비 8마리를 거리별로 세운 장면 (같은 방식, 시선 고정)
원본: `ProfilerCaptures/3DFPSGame_2026-11-02_BeforeOpt_Lineup.data`, `..._AfterOpt_Lineup.data`
스크린샷: `00_Lineup_Before.png`, `00_Lineup_After.png`

| 항목 | Before | After | 변화 |
|---|---|---|---|
| **Batches** | 828 | 372 | **-55%** |
| **Draw Calls** | 1,332 | 609 | **-54%** |
| SetPass Calls | 134 | 104 | -22% |
| **Triangles** | 88.5만 | 44.2만 | **-50%** |
| **Shadow Casters** | 1,205 | 374 | **-69%** |
| Visible Skinned Meshes | 113 | 49 | -57% |
| GPU Frame Time | 5.90 ms | 5.09 ms | -14% |
| CPU Render Thread | 2.13 ms | 1.94 ms | -9% |
| PlayerLoop (게임 CPU) | 6.28 ms | 5.86 ms | -7% |
| MinimapCamera 렌더링 | 1.26 ms | 1.00 ms | -21% |
| MeshSkinning.Update | 0.15 ms | 0.09 ms | -39% |
| **Texture Memory** | 806 MB | 496 MB | **-310 MB (-38%)** |
| Gfx Used Memory | 766 MB | 598 MB | -168 MB |

## 결과 2: 평소 플레이 (조명·PostFX 때와 같은 방식, 프레임 10~351 평균)
원본: `ProfilerCaptures/3DFPSGame_2026-11-02_AfterPostFX.data`(Before), `..._AfterOpt.data`(After)

| 항목 | Before | After | 변화 |
|---|---|---|---|
| Batches | 151 | 99 | -34% |
| SetPass Calls | 87 | 57 | -34% |
| **Draw Calls** | 472 | 196 | **-59%** |
| Triangles | 64.7만 | 28.9만 | -55% |
| **Shadow Casters** | 579 | 118 | **-80%** |
| GPU Frame Time | 4.87 ms | 4.59 ms | -6% |
| CPU Render Thread | 1.90 ms | 1.76 ms | -7% |
| PlayerLoop | 4.75 ms | 4.52 ms | -5% |
| MinimapCamera 렌더링 | 1.17 ms | 0.92 ms | -21% |
| **Texture Memory** | 768 MB | 488 MB | **-280 MB (-36%)** |
| Gfx Used Memory | 737 MB | 583 MB | -154 MB |

평소 플레이는 녹화마다 화면 속 좀비 수가 달라서(Before 평균 스킨드 메시 8, After 1) 좀비 쪽 수치는 결과 1을 기준으로 본다.

## 결과 3: Game 창 Stats (같은 조건, 한 순간의 값)
| 항목 | Before (조명 전) | 조명 After | PostFX After | **최적화 After** | PostFX → 최적화 |
|---|---|---|---|---|---|
| FPS | 73.3 (13.6 ms) | 74.2 (13.5 ms) | 65.5 (15.3 ms) | 66.6 (15.0 ms) | 거의 같음 |
| CPU main / render thread | 13.6 / 5.1 ms | 13.5 / 5.1 ms | 15.3 / 5.6 ms | 15.0 / 5.3 ms | -0.3 / -0.3 ms |
| **Batches** | 176 | 179 | 151 | **99** | **-34%** |
| Saved by batching | 328 | 607 | 599 | 155 | -74% |
| **SetPass Calls** | 87 | 88 | 87 | **57** | **-34%** |
| **Tris / Verts** | 63.6만 / 57.1만 | 67.0만 / 63.3만 | 66.0만 / 62.4만 | **29.2만 / 27.1만** | **-56% / -57%** |
| **Shadow Casters** | 323 | 610 | 582 | **120** | **-79%** |
| Visible skinned meshes | 15 | 15 | 15 | 1 | -14 |
| Animator 재생 중 | 2 | 2 | 2 | 2 | 같음 |

- Batches·SetPass·삼각형·그림자 대상이 Profiler 평균(결과 2)과 거의 같은 비율로 줄었다. 두 측정이 서로 맞는다.
- **Visible skinned meshes 15 → 1**: 좀비 부위 14개가 따로 그려지던 것이 LOD로 사라졌다. 좀비가 멀리 있으면 한 덩어리 메시나 컬링으로 바뀐다(Animator 2는 그대로라 좀비는 여전히 있다).
- **Saved by batching 599 → 155**: 그림자로 그리던 물체 자체가 줄어서 묶을 거리도 준 것이다. 나빠진 것이 아니다.
- **FPS는 거의 같다(65.5 → 66.6).** CPU main 15 ms 대부분이 에디터 비용이라 그리는 양이 절반으로 줄어도 FPS가 잘 변하지 않는다. 그리는 양의 감소는 빌드나 낮은 사양 GPU에서 FPS 차이로 나타난다.

## 결과 4: 이펙트 풀링 동작 확인
- Play 중 한 프레임에 피 이펙트 40번, 총 40발을 쏘게 했다. 이펙트가 끝난 뒤 풀에 60개(종류별 최대 30)만 남고 모두 꺼져 재사용 대기 상태였다. 콘솔 에러 없음.
- 평소 연사(초당 10발)에서는 미리 만든 10개 안에서 돌기 때문에 게임 중 Instantiate/Destroy가 생기지 않는다.
- 평소 플레이 녹화는 총을 쏘지 않아 GC Alloc/프레임은 Before·After 모두 18 KB로 같다(에디터 몫 포함).

## 화질
`01~04_GameView_*.png`(4방향)는 `Screenshots/PostFX`와 거의 같다. 텍스처를 줄인 Drum·Box도 차이가 보이지 않는다. 좀비는 가까우면 부위 메시, 멀면 한 덩어리 메시로 바뀐다(`00_Lineup_After.png`). 미니맵도 그대로 보인다.

## 해석
- **가장 큰 효과는 Draw Call과 그림자.** 좀비 LOD와 그림자 cascade 축소가 겹쳐 그림자 대상이 70~80% 줄었다. 에디터 GPU가 넉넉해 ms 차이는 작지만, 좀비가 많은 장면이나 낮은 사양 GPU에서 차이가 커진다.
- **RAM은 텍스처 280~310 MB 절약.** 쓰지도 않는 4096² 텍스처가 가장 큰 낭비였다.
- **Mesh Memory는 에디터에서 변화가 없다(68 → 68 MB).** 에디터는 Read/Write 설정과 상관없이 메시의 CPU 사본을 들고 있다. Read/Write를 끈 효과는 빌드에서 나타난다.
- **CPU 스크립트는 원래 가벼워서** 수치 변화는 작다. 풀링은 연사·피격 순간의 멈칫함(스파이크)을 막는 쪽의 개선이다.

## 남은 것
- 빌드(Development Build)에서 재면 에디터 비용 없이 Mesh Memory 감소와 실제 FPS를 확인할 수 있다.
- 미니맵 카메라를 몇 프레임에 한 번만 그리면 렌더링 비용을 더 줄일 수 있다(이번에는 하지 않음).
