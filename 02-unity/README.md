# Unity 프로젝트 구조 제안 (DDD 기반)

이 문서는 `02-unity` 폴더 아래에서 **도메인 중심 설계(DDD)** 관점으로 코드를 어떻게 나눌지에 대한 기본 가이드를 제안합니다.  
우리 프로젝트에서는 **도메인(기능 영역) → 레이어 구조**를 기본 원칙으로 **채택할 것을 제안합니다.**

## 1. 전체 디렉토리 개요 (도메인/레이어)

- **목적**: 여러 사람이 동시에 작업할 때 **서로 코드 충돌을 최소화하면서 개발할 수 있는 폴더 구조**를 제안합니다.

Unity `Assets/` 폴더는 대략 다음과 같이 구성하는 것을 목표로 합니다.

```text
Assets/
  Player/
    Domain/
    Application/
    Presentation/
    Infrastructure/
  Monster/
    Domain/
    Application/
    Presentation/
    Infrastructure/
  Battle/           # 전투
    Domain/
    Application/
    Presentation/
    Infrastructure/
  Common/           # 공통으로 사용하는 것들
    Domain/
    Application/
    Presentation/
    Infrastructure/
  ...
```

- **도메인 폴더**: `Player`, `Monster`, `Battle` 등  
  - “이 기능(도메인)에 관련된 것은 이 폴더 안에 모두 모은다”는 느낌으로 사용
- **레이어 폴더**: 각 도메인 폴더 안에서 다시 `Domain / Application / Presentation / Infrastructure`로 나눔

> 공통 원칙  
> - 의존성 방향: **Presentation / Infrastructure → Application → Domain** (안쪽으로만 의존)  
> - 도메인 간 직접 참조는 최소화하고, 꼭 필요하면 `Common`이나 인터페이스를 통해 의존
> - 의존성 주입(DI)은 **VContainer**를 기본 컨테이너로 사용 (후술 예정)

## 2. 레이어 역할 요약

- **목적**: 각 레이어(Domain / Application / Presentation / Infrastructure)가 **무엇을 담당하는지 공통 이해를 맞추는 것**이 목표입니다.

- **Domain (제안)**
  - Unity 엔진에 의존하지 않는 **순수 C# 도메인 모델/규칙**을 정의
  - 게임 규칙, 스탯 계산, 전투/협상/루머 전파 로직 등
  - **해당 폴더에 포함되면 좋은 파일들 (예시)**
    - **비즈니스 객체**: 엔티티, 값 객체, 애그리게잇 등 도메인 모델
    - **도메인 서비스**: 특정 엔티티 하나에 넣기 애매한 도메인 규칙/로직
    - **Client 인터페이스**: 외부 서비스(예: 서버 API, 플랫폼 SDK 등)와 연동하기 위한 추상 인터페이스
    - **Repository 인터페이스**: `IPlayerRepository` 같은 저장소 인터페이스 (구현은 인프라에 위치)
    - **도메인 컴포넌트(`Components/`)**: 도메인 서비스를 잘게 쪼개어 캡슐화한 순수 C# 클래스들(정책/전략/규칙 객체 등)

- **Application (제안)**
  - 도메인을 사용해 **구체적인 유즈케이스(기능 단위)**를 제공
  - 예: “전투 시작하기”, “협상 시도하기”, “플레이어에게 루머 전파하기”
  - **해당 폴더에 포함되면 좋은 파일들 (예시)**
    - **어플리케이션 서비스**: 도메인들을 조합하여 사용하는 UseCase 로직들 정의
    - **명령/응답 DTO**: UI·외부에서 들어오는 요청(Command)과 결과(Response)를 표현하는 데이터 객체
    - **애플리케이션 이벤트**: 도메인/애플리케이션에서 발생한 의미 있는 사건을 다른 레이어에 전달하기 위한 이벤트 정의
    - **애플리케이션 컴포넌트(`Components/`)**: 복잡한 유즈케이스 로직을 분리/재사용하기 위한 순수 C# 헬퍼/전략 클래스들

- **Presentation (제안)**
  - Unity 씬, UI, 입력 처리 등 **사용자와 직접 상호작용**하는 부분
  - 버튼 클릭/입력 → 애플리케이션 레이어의 유즈케이스 호출 → 결과를 화면에 반영
  - `MonoBehaviour`는 가급적 **입력/뷰/바인딩에만 집중**하도록 유지
  - **해당 폴더에 포함되면 좋은 파일들 (예시)**
    - **뷰/컨트롤러 스크립트**: `MonoBehaviour` 기반 UI/씬 컨트롤러 (입력 처리, 화면 갱신)
    - **바인딩/어댑터**: 애플리케이션 레이어의 명령/응답 DTO를 UI 요소와 연결하는 코드
    - **프리팹/씬 구성 관련 스크립트**: UI 레이아웃/씬 전환을 관리하는 스크립트

- **Infrastructure (제안)**
  - 애플리케이션/도메인에서 정의한 인터페이스를 **실제 기술로 구현**하는 영역
  - 예: 파일 저장, JSON 직렬화, 서버 통신, Unity Addressables, Audio 시스템 등
  - **해당 폴더에 포함되면 좋은 파일들 (예시)**
    - **레포지터리 구현체**: `IPlayerRepository` 등 도메인/애플리케이션에서 정의한 저장소 인터페이스의 구현
    - **외부 서비스 클라이언트 구현체**: 서버 API, 플랫폼 SDK 등 외부 서비스와 연동하는 클래스
    - **기술 세부 구현 유틸**: 파일 I/O, 네트워크 통신, Addressables/리소스 로딩, 로깅 등 기술적인 세부 처리 코드

## 3. 예시: Player 도메인 구조

- **목적**: 앞에서 설명한 구조를 **실제 Player 도메인에 적용했을 때의 한 가지 예시**를 보여줍니다.

```text
Assets/Player/
  Domain/
    Player.cs
    PlayerDomainService.cs
    IPlayerRepository.cs
  Application/
    PlayerUseCase.cs
  Presentation/
    PlayerController.cs
  Infrastructure/
    FilePlayerRepository.cs
    NetworkPlayerRepository.cs
```

- UI 쪽 스크립트(`PlayerController`)는  
  `PlayerUseCase` 등 **애플리케이션 레이어만 의존**하도록 유지합니다.
- 실제 저장/조회, 외부 서비스 연동 등은 `Infrastructure`에서 구현하고,  
  애플리케이션에서는 도메인만 바라보고,
  도메인에서는 인터페이스(`IPlayerRepository`)만 바라봅니다.

## 4. Assets 작업 (`01-assets`와 연계)

- **목적**: 코드와 에셋 작업을 **서로 간섭을 최소화하는 방식으로 분리**하는 기준을 제안합니다.

- `01-assets` 폴더에는 **원천 리소스(원본 PSD, WAV, 블렌더 파일 등)**를 두고,
- Unity 프로젝트(`02-unity`)의 `Assets/`에는 **빌드에 사용되는 가공 리소스**만 둔다.
- 에셋 작업 이슈/브랜치도 **에셋 작업만 따로** 가져가고,  
  코드/도메인/인프라 작업과는 가능하면 분리해서 진행한다.

## 5. 협업 시 원칙 정리 (요약)

- **목적**: 위에서 정의한 구조를 실제 협업에 적용할 때의 **실행 순서와 체크리스트**를 요약합니다.

- 새로운 기능을 시작할 때:
  1. **해당 기능이 속할 도메인 폴더부터 결정**한다. (예: `Battle`, `Player`, `Negotiation` 등)
  2. 그 도메인의 `Domain`/`Application`에서 **인터페이스와 유즈케이스를 먼저 설계 & 합의**한다.
  3. 이후에 **UI, 인프라, 에셋 작업을 병렬로** 진행한다.
  4. 브랜치는 가능하면 **하나의 도메인 + 하나의 레이어(또는 에셋 영역)에만 집중**해서 딴다.

이 구조를 기본 템플릿으로 두고, 실제 개발을 진행하면서 필요한 세부 폴더나 네이밍 규칙을  
`00-docs/collaboration-rules.md`와 이 README에 계속 업데이트해 나가면 됩니다.

## 6. VContainer 도입 (제안)

- **목적**: 기존에 모든 것을 붙잡고 있던 **각종 Manager(싱글톤, God Object 등)를 줄이고, 역할별로 잘게 나눈 객체들을 VContainer로 조립·관리하기 위해 VContainer가 필요하다는 이유**를 설명합니다.

- **의존성 관리 일원화**
  - 도메인, 애플리케이션, 인프라, 프레젠테이션 레이어 간 의존성을 **VContainer 하나로 관리**하면,
  - 각 레이어가 서로를 직접 `new`로 생성하거나 `FindObjectOfType`으로 찾지 않아도 됩니다.
- **테스트 용이성 향상**
  - 도메인/애플리케이션 레이어는 **인터페이스에만 의존**하고,  
    실제 구현(예: `FilePlayerRepository`, `NetworkPlayerRepository`)은 VContainer에서 주입받도록 하면,
  - 단위 테스트 시에는 **가짜 구현(Mock/Fake)**을 쉽게 바꿔 끼울 수 있습니다.
- **유니티 라이프사이클과의 통합**
  - VContainer는 Unity 씬/라이프사이클과 통합되어,  
    씬 전환, 스코프(예: 게임 전체 / 씬 단위 / 일시적인 유즈케이스 단위) 별로
    오브젝트 생명주기를 관리하기에 적합합니다.
- **레이어 구조와 잘 어울리는 DI 컨테이너**
  - 이 문서에서 제안하는 **Presentation / Infrastructure → Application → Domain** 의존 방향과도 궁합이 좋아,
  - 각 레이어별로 등록 모듈(Installer)을 나누어 **의존성 그래프를 명시적으로 설계**할 수 있습니다.