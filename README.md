# 🍉 Suika Game (수박게임)

> Unity 6로 개발한 2D 물리 기반 과일 합체 퍼즐 게임

![Unity](https://img.shields.io/badge/Unity-6000.4.1f1-black?logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-purple)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow)

---

## 📖 게임 소개

수박게임은 위에서 과일을 떨어뜨려 같은 종류끼리 합체시키면 더 큰 과일이 되는 물리 기반 퍼즐 게임입니다.  
가장 작은 **체리(레벨 0)** 부터 시작해 **수박(레벨 11)** 까지 총 12단계의 과일이 존재합니다.  
과일이 화면 위쪽 경계선을 3초 이상 넘으면 게임오버가 됩니다.

---

## 🎮 게임 규칙

- 마우스로 과일의 위치를 조정하고 클릭하면 떨어집니다
- 같은 종류의 과일끼리 충돌하면 합체되어 다음 레벨 과일이 됩니다
- 수박(레벨 11)끼리 합체하면 두 과일이 사라집니다
- 과일이 경계선을 3초 이상 넘으면 게임오버입니다
- 합체할수록 점수가 올라가며 상위 10개의 점수가 저장됩니다

---

## ⭐ 특수 능력

특수 능력은 기본적으로 사용 불가 상태이며, **황금 과일**을 합체하면 랜덤으로 능력을 획득할 수 있습니다.  
**Tab 키**로 능력창을 열고 닫을 수 있습니다.

| 능력 | 설명 | 획득 확률 | 사용 횟수 | 쿨다운 |
|------|------|----------|----------|--------|
| 💣 폭탄 (Bomb) | 클릭한 위치 반경 내 과일 일괄 제거 | 10% | 3회 | 5초 |
| 🎯 픽 (Pick) | 0~11 레벨 중 원하는 과일 직접 선택 | 10% | 3회 | 8초 |
| 🔽 레벨다운 (Down) | 클릭한 과일을 한 단계 낮은 과일로 변환 | 50% | 3회 | 6초 |
| 🌍 지진 (Quake) | 통 안의 모든 과일을 랜덤으로 흔들어 재배치 | 30% | 3회 | 10초 |

### 황금 과일
- 일반 과일과 동일한 레벨 시스템
- 황금 과일이 합체에 포함되면 랜덤 능력 획득
- 황금 + 황금 합체 → 다음 레벨 황금 과일 생성
- 황금 + 일반 합체 → 다음 레벨 일반 과일 생성
- 등장 확률: **3%**

---

## 🛠 개발 환경

| 항목 | 내용 |
|------|------|
| 엔진 | Unity 6 (6000.4.1f1) |
| 언어 | C# |
| 렌더링 | Universal Render Pipeline (URP) |
| 물리 | Rigidbody 2D / Circle Collider 2D |
| UI | TextMeshPro (NotoSansKR 폰트) |
| 입력 | Unity Input System |
| 씬 관리 | SceneManager |

---

## 📁 프로젝트 구조

```
Assets/
├── Prefabs/
│   ├── Fruit_0 ~ Fruit_11       # 일반 과일 Prefab
│   └── GoldFruit_0 ~ GoldFruit_11  # 황금 과일 Prefab
├── Scenes/
│   ├── StartScene               # 시작 화면 (랭킹, 도움말)
│   └── GameScene                # 메인 게임 씬
├── Scripts/
│   ├── GameManager.cs           # 게임 전반 관리 (드롭, 합체, 점수, 게임오버)
│   ├── Fruit.cs                 # 과일 충돌 및 합체 감지
│   ├── AbilityManager.cs        # 특수 능력 4종 관리
│   ├── DeadLine.cs              # 게임오버 경계선 감지
│   └── StartManager.cs          # 시작화면 및 랭킹 관리
├── Sprites/                     # 과일 스프라이트 이미지
└── Fonts/                       # NotoSansKR 한글 폰트
```

---

## 🚀 설치 및 실행

### 요구 사항
- Unity 6 (6000.4.1f1) 이상
- Unity Input System 패키지
- TextMeshPro 패키지

### 실행 방법

```bash
# 저장소 클론
git clone https://github.com/username/suika-game.git

# Unity Hub에서 프로젝트 열기
# Open → 클론한 폴더 선택
```

1. Unity Hub에서 **Open** 클릭
2. 클론한 폴더 선택
3. Unity 6으로 프로젝트 열기
4. `Scenes/StartScene` 을 열고 플레이 버튼 클릭

---

## ✅ 개발 현황

### 완성된 기능
- [x] 과일 12종 + 황금 과일 12종 Prefab
- [x] Rigidbody 2D 물리 엔진 적용
- [x] 마우스 드롭 조작 (Unity Input System)
- [x] 과일 합체 시스템 (Coroutine 기반 중복 방지)
- [x] 점수 시스템 (합체 레벨에 따라 점수 누적)
- [x] 게임오버 판정 (3초 카운트다운 표시)
- [x] 재시작 및 타이틀 복귀 버튼
- [x] 특수 능력 4종 (폭탄 / 픽 / 레벨다운 / 지진)
- [x] 황금 과일 랜덤 등장 및 능력 획득 시스템
- [x] Tab 키 능력창 토글
- [x] 시작 화면 (랭킹 Top 10, 도움말 버튼)
- [x] PlayerPrefs 점수 저장 (Top 10)
- [x] 한글 폰트 적용 (NotoSansKR)

### 예정 기능
- [ ] 과일 스프라이트 이미지 적용
- [ ] 게임 배경 및 통 디자인 적용
- [ ] UI 디자인 개선
- [ ] 합체 파티클 이펙트
- [ ] 효과음 및 배경음악
- [ ] 도움말 페이지 완성
- [ ] 모바일 터치 지원

---

## 📝 개발 노트

### 주요 버그 수정 이력
- **합체 중복 호출 문제** → `isMerging` 플래그 + Coroutine으로 해결
- **능력 사용 중 드롭 버그** → `IsBusy` 프로퍼티로 상태 통합 관리
- **황금 과일 합체 불가 버그** → `ForceNextFruit()` 에서 `isMerging` 초기화
- **수박 합체 불가 버그** → `level >= 10` 조건문 제거
- **데드라인 오탐지 문제** → 과일 낙하 속도 감지로 해결

---

## 📜 라이선스

이 프로젝트는 학습 목적으로 제작되었습니다.
