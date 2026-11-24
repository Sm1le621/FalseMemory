# 1. Основы Unity для C#-программиста

В обычной C#-программе ты сам пишешь `Main`, создаёшь объекты через `new` и вызываешь методы. В Unity всё наоборот: **объекты создаёт движок, а твой код он вызывает сам** в нужные моменты. Ниже — всё, что нужно, чтобы читать скрипты проекта.

---

## 1.1 GameObject и компоненты

**GameObject** — любой объект на сцене: игрок, враг, камера, кнопка. Сам по себе он пустой — у него есть только `Transform` (позиция, поворот, масштаб).

Поведение добавляется **компонентами**. Компонент — это класс, «прикреплённый» к объекту. Встроенные: `SpriteRenderer` (рисует картинку), `Rigidbody2D` (физика), `Collider2D` (форма для столкновений), `AudioSource` (звук). Свои компоненты — это наши скрипты.

Пример — объект `Player` в проекте:

```
Player
 ├─ Transform, SpriteRenderer, Rigidbody2D, Collider2D, Animator   ← встроенные
 └─ PlayerController, Health, BattleUnit, BattleInventory,          ← наши скрипты
    InventoryWindow, EquipmentController, WorldItemUseController, WorldInputGate
```

> Это **композиция вместо наследования**: игрок — не класс `Player : Character`, а набор маленьких компонентов, каждый отвечает за одно.

## 1.2 MonoBehaviour

Чтобы класс можно было повесить на объект, он наследуется от `MonoBehaviour`:

```csharp
public class CameraFollow : MonoBehaviour { ... }
```

Важно:
- **`new CameraFollow()` не пишут никогда.** Компонент создаёт Unity, когда объект загружается со сценой (или через `AddComponent<T>()`).
- Внутри компонента доступны `transform`, `gameObject`, `enabled` (включён ли компонент).

Классы, которым не нужно жить на объекте, — **обычные C#-классы**: `InventoryState`, `EquipmentState`, `BattleCalculator`. Их можно тестировать без Unity.

## 1.3 Жизненный цикл: кто вызывает методы

Unity сама вызывает методы с «магическими» именами, если они есть в классе (даже если `private`):

| Метод | Когда вызывается | Пример в проекте |
|---|---|---|
| `Awake()` | Один раз, сразу при создании объекта | `BattleUnit.Awake` ставит `currentMP = maxMP` |
| `OnEnable()` | Каждый раз при включении компонента/объекта | `EquipmentController` подписывается на события |
| `Start()` | Один раз перед первым кадром | `Health.Start` ставит HP = максимум |
| `Update()` | Каждый кадр | `PlayerController` читает клавиатуру |
| `FixedUpdate()` | Каждый шаг физики (фиксированно, ~50 раз/с) | `PlayerController`, `EnemyAI` двигают `Rigidbody2D` |
| `LateUpdate()` | Каждый кадр, после всех `Update` | `CameraFollow` догоняет игрока |
| `OnDisable()` | При выключении | отписка от событий |
| `OnDestroy()` | При уничтожении | `BattleTransition` очищает `Instance` |

Почему движение в `FixedUpdate`, а не в `Update`: кадров в секунду на разных компьютерах разное число, а шаг физики фиксированный — так скорость одинаковая везде (в проекте это была реальная ошибка, исправленная в `EnemyAI`).

## 1.4 Inspector и ссылки между объектами

**Inspector** — панель Unity, где видны поля компонента. Unity показывает там:
- `public` поля;
- `private` поля с атрибутом `[SerializeField]`.

```csharp
public sealed class BattleManager : MonoBehaviour
{
    public BattleUI ui;                         // видно в Inspector
    [SerializeField] private Image fade;        // тоже видно, но снаружи недоступно
}
```

Значения из Inspector **сохраняются в файле сцены** и **перекрывают** значения по умолчанию из кода. Например, в коде `public int attack = 15;`, а в сцене у скелета стоит `12` — в игре будет 12.

Ссылку на другой объект задают **перетаскиванием** объекта в поле Inspector. Так `BattleManager.ui` указывает на компонент `BattleUI`. Это главный способ «соединить» объекты без кода.

## 1.5 GetComponent и RequireComponent

Если нужный компонент висит **на том же объекте**, его находят кодом:

```csharp
private void Awake()
{
    unit = GetComponent<BattleUnit>();       // найти компонент BattleUnit на этом же GameObject
    window = GetComponent<InventoryWindow>();
}
```

`[RequireComponent(typeof(...))]` над классом говорит Unity: «без этого компонента меня не добавлять» — Unity добавит его автоматически:

```csharp
[RequireComponent(typeof(Health))]
public sealed class BattleUnit : MonoBehaviour { ... }
```

## 1.6 ScriptableObject — файлы с данными

`ScriptableObject` — класс, экземпляры которого хранятся **как файлы** (`.asset`) в папке проекта, а не на сцене. Это «настройки» и «описания».

```csharp
[CreateAssetMenu(menuName = "FalseMemory/Battle Spell")]
public sealed class BattleSpell : ScriptableObject
{
    public string displayName = "Fire";
    public int mpCost = 8;
    public int power = 10;
}
```

`[CreateAssetMenu]` добавляет пункт в меню Create. В проекте так сделаны: заклинание `Fire`, зелье `Potion`, предметы `IronSword` / `LeatherArmor`, правила боя `DefaultBattleRules`, профили звука и эффектов. Один файл `Potion.asset` используют все зелья — в инвентаре хранится **ссылка** на него и количество.

## 1.7 События и подписки

Чтобы один скрипт сообщал другим «что-то случилось», не зная, кто его слушает, используются C#-события:

```csharp
// InventoryState — издатель
public event Action Changed;
...
if (Changed != null)
{
    Changed.Invoke();        // «инвентарь изменился» — вызвать всех подписчиков
}

// InventoryWindow — подписчик
private void OnEnable()  { Inventory.State.Changed += Refresh; }   // подписка
private void OnDisable() { Inventory.State.Changed -= Refresh; }   // отписка
```

Правило Unity: **подписываться в `OnEnable`, отписываться в `OnDisable`**. Иначе выключенный или уничтоженный объект продолжит получать вызовы → ошибки `MissingReferenceException`.

Кроме C#-событий есть **UnityEvent** — события кнопок (`Button.onClick`). Их можно настроить прямо в Inspector (кнопка «New Game» → `MainMenuController.OnStart`) или кодом: `useButton.onClick.AddListener(RequestUse);`.

## 1.8 Корутины — «методы с паузами»

Корутина — метод, который возвращает `IEnumerator` и может **приостановиться** на `yield return`, а потом продолжить в следующем кадре. Так пишутся длинные процессы: анимации, затемнение, весь бой.

```csharp
private IEnumerator EnterBattle()
{
    yield return FadeTo(1f);                    // ждать, пока экран потемнеет
    var load = SceneManager.LoadSceneAsync(...);
    yield return load;                          // ждать загрузки сцены
    yield return FadeTo(0f);                    // проявить экран
}

StartCoroutine(EnterBattle());                  // запуск
```

| Что после `yield return` | Что значит |
|---|---|
| `null` | подождать один кадр |
| `new WaitForSecondsRealtime(0.8f)` | подождать 0.8 с |
| другая корутина / `AsyncOperation` | подождать, пока она закончится |

`BattleManager.Start()` — тоже корутина (`IEnumerator Start`), внутри неё весь цикл боя.

## 1.9 Физика: триггеры и столкновения

У `Collider2D` есть галочка **Is Trigger**:
- **выключена** → объекты сталкиваются, Unity вызывает `OnCollisionEnter2D(Collision2D)`;
- **включена** → объекты проходят насквозь, вызывается `OnTriggerEnter2D(Collider2D)` / `OnTriggerExit2D`.

В проекте:
- большой триггер у скелета — «зона видимости» → `EnemyAI` начинает погоню;
- обычный коллайдер скелета — касание → `BattleEncounter` начинает бой;
- триггер у входа в пещеру → `SceneTransition` загружает `Dungeon`.

Проверка `other.CompareTag("Player")` — сравнение **тега** объекта (метка, задаётся в Inspector).

## 1.10 Сцены

**Сцена** — файл `.unity` с набором объектов (уровень, меню, бой).

- `SceneManager.LoadScene("MainMenu")` — загрузить сцену **вместо** текущей (режим **Single**, старые объекты уничтожаются).
- `LoadSceneAsync(name, LoadSceneMode.Additive)` — загрузить **поверх** текущей, старая остаётся. Так в проекте открывается бой: мир не уничтожается, а прячется.
- `DontDestroyOnLoad(obj)` — объект **переживёт** загрузку новой сцены. Так игрок с инвентарём переходит в подземелье.

## 1.11 Singleton через static Instance

Когда объект должен быть один и доступен отовсюду:

```csharp
public static BattleTransition Instance { get; private set; }

private void Awake() { Instance = this; }

// в любом другом скрипте:
BattleTransition.Instance.TryBegin(player, enemy);
```

В проекте так сделаны `BattleTransition` и `SceneTransitionManager`.

## 1.12 Шпаргалка: 5 способов, которыми скрипты находят друг друга

| Способ | Когда | Пример |
|---|---|---|
| Поле в Inspector | объекты на одной сцене, связь задаётся руками | `BattleManager.ui` → `BattleUI` |
| `GetComponent<T>()` | компоненты на одном объекте | `InventoryWindow` → `BattleInventory` |
| `Instance` (singleton) | один глобальный объект | `BattleTransition.Instance` |
| Событие `event` | «сообщить, не зная кому» | `InventoryWindow.UseRequested` |
| Статический класс | чистые функции без состояния | `BattleCalculator.MagicalDamage(...)` |
