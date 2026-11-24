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
class EquipmentController
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
BattleUnit --> EquipmentController
InventoryWindow --> BattleInventory
EquipmentController --> InventoryWindow : подписка на события
PlayerController --> WorldInputGate
InventoryWindow --> WorldInputGate
SceneTransition ..> SceneTransitionManager : ChangeScene()
BattleTransition ..> PlayerController : пауза
BattleTransition ..> EnemyAI : пауза
```

## 3.2 Бой

```mermaid
classDiagram
direction TB

class IBattleParticipant {
  <<interface>>
  +bool IsAlive
}
class IBattleRandom {
  <<interface>>
  +Next01() float
}
class UnityBattleRandom
class SeededBattleRandom
IBattleRandom <|.. UnityBattleRandom
IBattleRandom <|.. SeededBattleRandom

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
IBattleParticipant <|.. BattleUnit
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
  -BattleTurnOrder~BattleUnit~ turns
  -BattleCommandGate gate
  -IBattleRandom random
  -Start() IEnumerator
  +TryPlayerAction(BattleAction) bool
  +OpenMagicMenu() bool
  +TryPlayerSpell(BattleSpell) bool
}
BattleManager --> BattleUI
BattleManager --> BattlePresentation
BattleManager --> BattleVFXController
BattleManager --> BattleAudioController
BattleManager *-- BattleTurnOrder~T~
BattleManager *-- BattleCommandGate
BattleManager --> IBattleRandom
BattleManager ..> BattleTransition
BattleManager ..> BattleCalculator
BattleManager ..> BattleAbilities
BattleManager ..> EnemyBattleAI

class BattleTurnOrder~T~ {
  +BeginRound(IEnumerable~T~)
  +TryNext(out T) bool
}
class BattleCommandGate {
  +Open()
  +Close()
  +TryConsume() bool
}
class BattleCalculator {
  <<static>>
  +ResolvePhysical(...) PhysicalAttackResult
  +MagicalDamage(...) int
}
class BattleAbilities {
  <<static>>
  +CanCast(...) bool
  +TryCast(...) bool
  +CanUseItem(...) bool
  +TryUseItem(...) bool
}
class EnemyBattleAI {
  <<static>>
  +Choose(BattleRules, IBattleRandom) BattleAction
}
BattleAbilities ..> BattleCalculator
EnemyBattleAI ..> IBattleRandom

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

class EquipmentController {
  <<MonoBehaviour>>
  +EquipmentState State
  +TryEquip(int, EquipmentSlot?) bool
  +TryUnequip(EquipmentSlot) bool
}
EquipmentController *-- EquipmentState
EquipmentController --> BattleInventory

class InventoryWindow {
  <<MonoBehaviour>>
  +InventorySlotView[] slots
  +InventoryTooltip tooltip
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
InventoryWindow --> InventoryTooltip
EquipmentController --> InventoryWindow : подписка

class WorldItemUseController {
  <<MonoBehaviour>>
  +CanUse(int) bool
  +TryUse(int) bool
}
WorldItemUseController --> InventoryWindow : подписка
WorldItemUseController ..> BattleAbilities

class InventorySlotView
<<MonoBehaviour>> InventorySlotView
class EquipmentSlotView
<<MonoBehaviour>> EquipmentSlotView
class InventoryTooltip
<<MonoBehaviour>> InventoryTooltip
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
InventoryTooltip ..> ItemPresentation
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
  +OnStart()
  +OnSettings()
  +OnExit()
}
class SettingsMenu {
  <<MonoBehaviour>>
  +Show()
  +Hide()
  +OnVolumeChanged(float)
  +ToggleFullscreen()
}
PlayerController --> WorldInputGate
SceneTransition ..> SceneTransitionManager
SceneTransitionManager ..> CameraFollow
SceneTransitionManager ..> PlayerController
MainMenuController --> SettingsMenu
ManaHUD --> BattleUnit
```

Перечисления (`enum`): `BattleAction` {Attack, Defend, Magic, Flee}, `BattleState` {Starting, Resolving, AwaitingPlayer, Victory, Defeat, Escaped}, `ItemCategory` {Consumable, Weapon, Armor}, `EquipmentSlot` {Weapon, Chest}, `BattleSound`, `SpritePose`, `BattleVFXShape`, `BattleTarget`, `BattleEffect`, `ItemUseLocation`.
