# 3. Диаграмма классов

Диаграммы в формате **Mermaid** — GitHub рисует их автоматически. Также можно вставить код в [mermaid.live](https://mermaid.live).

Обозначения:

| Стрелка | Значение |
|---|---|
| `A --\|> B` | A наследуется от B |
| `A ..\|> B` | A реализует интерфейс B |
| `A *-- B` | A создаёт и хранит B внутри себя (композиция) |
| `A --> B` | A хранит ссылку на B (поле в Inspector или `GetComponent`) |
| `A ..> B` | A просто использует B (вызов метода, `Instance`) |

Стереотипы: `<<MonoBehaviour>>` — компонент на объекте, `<<ScriptableObject>>` — файл с данными, `<<static>>` — статический класс.

Показаны главные поля и методы; полный список — в [справочнике](08-scripts-reference.md).

---

## 3.1 Общая картина

```mermaid
classDiagram
direction LR
class PlayerController
class EnemyAI
class BattleEncounter
class BattleTransition
<<singleton>> BattleTransition
class BattleManager
class BattleUnit
class Health
class BattleInventory
class InventoryActions
class InventoryWindow
class SceneTransition
class SceneTransitionManager
<<singleton>> SceneTransitionManager

BattleEncounter ..> BattleTransition : TryBegin()
BattleTransition --> BattleUnit : Player / Enemy
BattleManager ..> BattleTransition : берёт участников
BattleManager ..> BattleUnit : урон, MP
BattleUnit --> Health
BattleUnit --> BattleInventory
BattleUnit --> InventoryActions
InventoryWindow --> BattleInventory
InventoryActions --> InventoryWindow : подписка на события
PlayerController --> WorldInputGate
InventoryWindow --> WorldInputGate
SceneTransition ..> SceneTransitionManager : ChangeScene()
BattleManager ..> LootDrop : GiveTo() после победы
PauseMenu --> InventoryWindow
PauseMenu --> WorldInputGate
BattleTransition ..> PlayerController : пауза
BattleTransition ..> EnemyAI : пауза
```

## 3.2 Бой

```mermaid
classDiagram
direction TB

class BattleRandom {
  +BattleRandom Unity$
  +BattleRandom(int seed)
  +Next01() float
}

class Health {
  <<MonoBehaviour>>
  +int maxHealth
  +int currentHealth
  +TakeDamage(int)
  +Heal(int) int
}
class BattleUnit {
  <<MonoBehaviour>>
  +string displayName
  +int attack, defense
  +int maxMP, magicAttack, magicDefense
  +BattleSpell[] spells
  +float criticalChance, missChance
  +int EffectiveAttack
  +int EffectiveDefense
  +int CurrentMP
  +bool IsDefending
  +TrySpendMP(int) bool
  +RestoreMP(int) int
  +BeginTurn()
  +Defend()
}
BattleUnit --> Health : RequireComponent
BattleUnit o-- BattleSpell

class BattleTransition {
  <<MonoBehaviour>>
  +BattleTransition Instance$
  +BattleRules rules
  +BattleUnit Player, Enemy
  +bool EncounterActive
  +TryBegin(BattleUnit, BattleUnit) bool
  +ReturnAfterVictory() bool
  +ReturnAfterEscape() bool
  +RestartGame()
  +ReturnToMainMenu()
}
class BattleEncounter {
  <<MonoBehaviour>>
  -OnCollisionEnter2D()
}
BattleEncounter ..> BattleTransition
BattleTransition --> BattleRules
BattleTransition ..> BattleScreenFade

class BattleManager {
  <<MonoBehaviour>>
  +BattleUI ui
  +BattlePresentation presentation
  +BattleVFXController vfx
  +BattleAudioController audioController
  +BattleState State
  +int Round
  -BattleRandom random
  -Start() IEnumerator
  +TryPlayerAction(BattleAction) bool
  +OpenMagicMenu() bool
  +TryPlayerSpell(BattleSpell) bool
}
BattleManager --> BattleUI
BattleManager --> BattlePresentation
BattleManager --> BattleVFXController
BattleManager --> BattleAudioController
BattleManager --> BattleRandom
BattleManager ..> BattleTransition
BattleManager ..> BattleCalculator

class BattleCalculator {
  <<static>>
  +ResolvePhysical(...) PhysicalAttackResult
  +MagicalDamage(...) int
  +ChooseEnemyAction(BattleRules, BattleRandom) BattleAction
  +CanCast(...) bool
  +TryCast(...) bool
  +CanUseItem(BattleUnit, BattleConsumable) bool
  +TryUseItem(...) bool
}
BattleCalculator ..> BattleRandom

class BattleUI {
  <<MonoBehaviour>>
  +BattleHUD hud
  +ShowMenu(string[], Action[], ...)
  +ShowMessage(string)
  +ShowFeedback(...)
  +RefreshStats(...)
}
class BattleHUD {
  <<MonoBehaviour>>
  +Bind(BattleUnit, BattleUnit)
  +RevealScreen(float) IEnumerator
}
class BattlePresentation {
  <<MonoBehaviour>>
  +Bind(BattleUnit, BattleUnit)
  +Windup(...) IEnumerator
  +Recover(...) IEnumerator
  +Finish(...) IEnumerator
}
class BattleVFXController {
  <<MonoBehaviour>>
  +Play(bool, BattleVFXProfile, Color)
}
class BattleAudioController {
  <<MonoBehaviour>>
  +BattleAudioProfile profile
  +Play(BattleSound) bool
}
BattleUI --> BattleHUD
BattleUI --> BattleAudioController
BattlePresentation --> BattleUI
BattleVFXController --> BattleUI
BattleVFXController ..> BattleVFXProfile
BattleAudioController --> BattleAudioProfile

class BattleRules
<<ScriptableObject>> BattleRules
class BattleSpell {
  <<ScriptableObject>>
  +int mpCost
  +int power
  +BattleVFXProfile visualEffect
}
class BattleVFXProfile
<<ScriptableObject>> BattleVFXProfile
class BattleAudioProfile
<<ScriptableObject>> BattleAudioProfile
class BattleScreenFade
<<MonoBehaviour>> BattleScreenFade
```

## 3.3 Инвентарь и экипировка

```mermaid
classDiagram
direction TB

class ItemData {
  <<ScriptableObject>>
  +string displayName
  +Sprite icon
  +ItemCategory category
  +int maxStack
  +EquipmentSlot equipmentSlot
  +ItemStatBonus bonuses
}
class BattleConsumable {
  <<ScriptableObject>>
  +int healing
  +int manaRestoration
}
ItemData <|-- BattleConsumable
class ItemStatBonus {
  <<struct>>
  +int attack
  +int defense
}
ItemData *-- ItemStatBonus

class InventorySlot {
  +ItemData Item
  +int Count
}
class InventoryState {
  +Capacity = 30$
  +event Changed
  +GetSlot(int) InventorySlot
  +TryAdd(ItemData, int) bool
  +TryRemove(ItemData, int) bool
  +TryMoveOrMerge(int, int) bool
}
InventoryState *-- "30" InventorySlot
InventorySlot --> ItemData

class EquipmentState {
  +event Changed
  +ItemStatBonus Bonuses
  +GetSlot(EquipmentSlot) ItemData
  +TryEquip(InventoryState, int, EquipmentSlot) bool
  +TryUnequip(InventoryState, EquipmentSlot) bool
}
EquipmentState --> "2" ItemData
EquipmentState ..> InventoryState

class BattleInventory {
  <<MonoBehaviour>>
  +List~StartingItem~ startingItems
  +InventoryState State
  +TryConsume(BattleConsumable) bool
}
BattleInventory *-- InventoryState

class InventoryActions {
  <<MonoBehaviour>>
  +EquipmentState State
  +TryEquip(int, EquipmentSlot?) bool
  +TryUnequip(EquipmentSlot) bool
  +CanUse(int) bool
  +TryUse(int) bool
}
InventoryActions *-- EquipmentState
InventoryActions --> BattleInventory
InventoryActions ..> BattleCalculator : TryUseItem

class InventoryWindow {
  <<MonoBehaviour>>
  +InventorySlotView[] slots
  +event UseRequested
  +event EquipRequested
  +event UnequipRequested
  +Open() bool
  +Close()
  +Refresh()
}
InventoryWindow --> BattleInventory
InventoryWindow --> WorldInputGate
InventoryWindow *-- "30" InventorySlotView
InventoryActions --> InventoryWindow : подписка

class LootDrop {
  <<MonoBehaviour>>
  +ItemData item
  +int count
  +GiveTo(BattleUnit) string
}
LootDrop --> ItemData
LootDrop ..> BattleInventory : TryAdd

class InventorySlotView
<<MonoBehaviour>> InventorySlotView
class EquipmentSlotView
<<MonoBehaviour>> EquipmentSlotView
class ItemPresentation
<<static>> ItemPresentation
class WorldInputGate {
  <<MonoBehaviour>>
  +bool IsBlocked
  +Acquire(object)
  +Release(object)
}
InventorySlotView --> InventoryWindow
EquipmentSlotView --> InventoryWindow
InventorySlotView ..> ItemPresentation : цвет ячейки
```

## 3.4 Мир, сцены, меню

```mermaid
classDiagram
direction LR
class PlayerController {
  <<MonoBehaviour>>
  -float moveSpeed
  -Update()
  -FixedUpdate()
}
class EnemyAI {
  <<MonoBehaviour>>
  +float moveSpeed
  -OnTriggerEnter2D()
  -OnTriggerExit2D()
}
class CameraFollow {
  <<MonoBehaviour>>
  +SetTarget(Transform)
  +SnapToTarget()
}
class SceneTransition {
  <<MonoBehaviour>>
  -string targetSceneName
  -Vector2 spawnPosition
}
class SceneTransitionManager {
  <<MonoBehaviour>>
  +Instance$
  +GetOrCreate()$
  +ChangeScene(string, bool, Vector2) bool
}
class ManaHUD {
  <<MonoBehaviour>>
  +BattleUnit unit
}
class MainMenuController {
  <<MonoBehaviour>>
  -GameObject settingsRoot
  +OnStart()
  +OnSettings()
  +OnExit()
  +HideSettings()
  +OnVolumeChanged(float)
  +ToggleFullscreen()
}
class PauseMenu {
  <<MonoBehaviour>>
  -GameObject panel
  -InventoryWindow inventory
  -WorldInputGate inputGate
  +bool IsPaused
  +Pause()
  +Resume()
  +OpenMainMenu()
}
PauseMenu --> InventoryWindow : выключает на паузе
PauseMenu --> WorldInputGate : Acquire / Release
PauseMenu ..> BattleTransition : нет ли боя
PlayerController --> WorldInputGate
SceneTransition ..> SceneTransitionManager
SceneTransitionManager ..> CameraFollow
SceneTransitionManager ..> PlayerController
ManaHUD --> BattleUnit
```

Перечисления (`enum`): `BattleAction` {Attack, Defend, Magic, Flee}, `BattleState` {Starting, Resolving, AwaitingPlayer, Victory, Defeat, Escaped}, `ItemCategory` {Consumable, Weapon, Armor}, `EquipmentSlot` {Weapon, Chest}, `BattleSound`.
