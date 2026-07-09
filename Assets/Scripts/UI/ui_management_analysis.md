# FaceTheRandomness UI 시스템 분석

이 문서는 `FaceTheRandomness` 프로젝트 내의 UI 아키텍처와 관리 흐름에 대한 상세한 분석을 제공합니다. 주요 핵심 클래스인 `UIManager`와 `UIBase`의 동작 원리를 파헤치며, 어떻게 설계되었는지 중점적으로 다룹니다.

## 1. 아키텍처 개요

프로젝트의 UI는 **중앙 집중형 관리 시스템**과 **독립된 UI 컴포넌트**의 결합으로 이루어져 있습니다.

- **`UIManager`**: 싱글톤으로 구현되어 전역에서 접근 가능하며, UI 캔버스 렌더링, 런타임 인스턴스화, 파괴를 담당합니다.
- **`UIBase`**: 모든 팝업 및 패널 스크립트의 기초가 되는 부모 클래스입니다.

---

## 2. 주요 클래스 심층 분석

### 2.1 UIManager.cs
`UIManager`는 딕셔너리를 활용하여 UI 인스턴스와 프리팹, 그리고 캔버스를 매칭하는 허브 역할을 합니다.

#### 필드 구조
```csharp
private Dictionary<string, Canvas> _canvasDict = new Dictionary<string, Canvas>();
private Dictionary<string, Canvas> _uiCanvasDict = new Dictionary<string, Canvas>();
private Dictionary<string, GameObject> _uiPrefabDict = new Dictionary<string, GameObject>();
public List<UIBase> UIList = new List<UIBase>();
```
- `_canvasDict`: 이름 기반으로 씬에 존재하는 모든 캔버스를 저장합니다. 카메라 할당(`SetCameras()`) 시 사용됩니다.
- `_uiCanvasDict`: **UI 컴포넌트 이름**을 키로, 해당 UI가 렌더링될 대상 **Canvas**를 값으로 매핑합니다.
- `_uiPrefabDict`: 동적으로 생성할 수 있는 UI 프리팹 원본들을 이름 기반으로 캐싱합니다.
- `UIList`: 현재 화면에 활성화(Open)된 UI 객체들의 레퍼런스를 추적합니다.

#### 초기화 프로세스 (`Awake()`)
> [!NOTE]
> `UIManager`는 씬이 시작될 때 매핑 작업과 씬 정리를 동시에 수행합니다.

1. **캔버스 수집**: 자식 오브젝트의 모든 `Canvas`를 찾아 `_canvasDict`에 보관.
2. **매핑 후 씬 정리(최적화)**:
   에디터 상에서 작업의 편의성을 위해 캔버스 아래에 배치해 둔 `UIBase` 객체들을 찾습니다. 이들의 부모 캔버스가 무엇인지 `_uiCanvasDict`에 기록한 뒤, 씬에 있는 원본 객체들은 모조리 **파괴(Destroy)**합니다. 이를 통해 비활성 상태의 불필요한 객체들이 메모리를 차지하는 것을 방지합니다.
3. **프리팹 로드**: `Resources.LoadAll<GameObject>("UIs")`를 통해 `Resources` 폴더에 위치한 UI 프리팹을 캐싱합니다. 

#### UI 열기/닫기 로직
- **`OpenUI<T>()` / `OpenUI(string)`**:
  요청된 UI 이름으로 `_uiPrefabDict`에서 프리팹을 꺼내고, `_uiCanvasDict`에서 알맞은 캔버스를 찾습니다. `Instantiate`를 통해 UI를 생성한 후 `UIBase.Open()`을 호출합니다.
- **`CloseUI(UIBase)`**:
  해당 인스턴스의 `Close()`를 호출하여 내부 정리 로직을 실행시킨 후, `Destroy`로 메모리에서 완전히 해제시킵니다.

### 2.2 UIBase.cs
단순하지만 강력한 상속 구조의 뼈대입니다.

```csharp
[DefaultExecutionOrder(-1)]
public class UIBase : MonoBehaviour
{
    private void Awake()
    {
        gameObject.name = GetType().Name;
    }

    public virtual void Open() { }
    public virtual void Close() { }
}
```
- **`[DefaultExecutionOrder(-1)]`**: 다른 스크립트보다 일찍 `Awake()`가 실행되도록 하여 의존성 문제를 방지합니다.
- **`Awake()`**: 생성된 게임 오브젝트의 이름을 자신의 클래스 이름으로 고정하여 하이어라키(Hierarchy)의 가독성을 높입니다.

### 2.3 SettingUI.cs (실전 활용 사례)
`UIBase`를 어떻게 활용하는지 보여주는 스크립트입니다.

- **생명주기 오버라이드**: 
  `Open()` 될 때 `GameManager.Instance.CurrentGame.Pause()`로 게임을 일시정지합니다.
  `Close()` 될 때는 반대로 게임을 재개(`Unpause()`)합니다.
- **자체 종료 로직**:
  UI 내부의 닫기 버튼(예: `Resume_Clicked`)이 눌리면 `UIManager.Instance.CloseUI(this)`를 호출해 창 스스로 파괴되도록 구현되어 있습니다.

---

## 3. 구조적 특징 요약

> [!TIP]
> **현재 구조의 장점**
> 1. **작업 편의성과 실행 시 깔끔함**: 에디터 상에 UI를 자유롭게 켜두고 작업하더라도, 게임을 실행하면 매핑만 저장하고 즉시 파괴하므로 씬 상태 관리가 아주 깔끔합니다.
> 2. **메모리 절약**: 필요한 화면(UI)만 실시간으로 Instantiate하여 띄우므로 불필요한 메모리 낭비가 없습니다.
> 3. **안전한 제네릭 호출**: `OpenUI<SettingUI>()` 처럼 제네릭 메서드를 지원하여 문자열 오타로 인한 오류를 방지합니다.

> [!WARNING]
> **고려해볼 만한 점 (개선 가능성)**
> - 현재 띄우고 닫는 과정에서 매번 `Instantiate`와 `Destroy`가 발생합니다. 사용 빈도가 매우 높은 UI(예: 인벤토리, 데미지 텍스트 등)의 경우 잦은 메모리 할당/해제로 인해 가비지 컬렉션(GC) 부하가 발생할 수 있습니다. 추후 최적화가 필요하다면 UI 오브젝트 풀링(Object Pooling) 기법의 도입을 고려할 수 있습니다.
