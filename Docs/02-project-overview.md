# 2. Обзор проекта

## 2.1 Игровой цикл

```
MainMenu ──New Game──► SampleScene (мир) ──касание скелета──► BattleScene (бой)
                         ▲   │                                   │
                         │   └──вход в пещеру──► Dungeon         │
                         └────────── победа / побег ◄────────────┘
                                      поражение ──► MainMenu или New Game
```

## 2.2 Сцены

| Сцена | Что в ней | Главные скрипты |
|---|---|---|
| `MainMenu` | Фон, кнопки New Game / Settings / Exit, панель настроек | `MainMenuController` |
| `SampleScene` | Мир: карта, игрок, скелет, HUD, окно инвентаря, меню паузы, вход в пещеру | `PlayerController`, `EnemyAI`, `BattleEncounter`, `BattleTransition`, `InventoryWindow`, `PauseMenu` |
| `BattleScene` | Экран боя: портреты, полосы HP/MP, меню команд, эффекты | `BattleManager`, `BattleUI`, `BattleHUD`, `BattlePresentation`, `BattleVFXController` |
| `Dungeon` | Подземелье: камни, скелеты, босс `BossSkeleton`, выход обратно | `SceneTransition` (на выходе), `LootDrop` (на боссе) |

Фоновая музыка — встроенный компонент `AudioSource` в каждой сцене, без своего скрипта.

## 2.3 Главные объекты и их компоненты

### Player (SampleScene)

| Компонент | Зачем |
|---|---|
| `Rigidbody2D`, `Collider2D`, `SpriteRenderer`, `Animator` | физика, столкновения, картинка, анимация ходьбы |
| `PlayerController` | движение WASD/стрелками |
| `Health` | HP (единственное место, где хранится HP) |
| `BattleUnit` | характеристики боя: атака, защита, MP, заклинания |
| `BattleInventory` | инвентарь (30 слотов) |
| `InventoryWindow` | окно инвентаря (клавиша **I**) |
| `InventoryActions` | действия из окна: надеть/снять оружие и броню, выпить зелье |
| `WorldInputGate` | «замок» на движение, пока открыто окно |

### Enemy — скелет (SampleScene)

`Rigidbody2D`, 2 × `Collider2D` (триггер-зона видимости + тело), `Health`, `BattleUnit`, `EnemyAI` (погоня), `BattleEncounter` (касание → бой). Спрайт статичный (один кадр Idle).

### Остальное в SampleScene

| Объект | Компоненты |
|---|---|
| `Main Camera` | `Camera`, `CameraFollow` |
| `Health` (HUD) | полосы HP/MP, `ManaHUD` |
| `BattleTransition` | `BattleTransition` + дочерний Canvas с `BattleScreenFade` (чёрный экран) |
| `InventoryCanvas` | панель инвентаря: 30 ячеек `InventorySlotView`, 2 ячейки `EquipmentSlotView` |
| `PauseCanvas` | `Canvas` + `PauseMenu`; внутри `PausePanel` (полупрозрачный чёрный фон) → `PauseWindow` → `Title`, `ResumeButton`, `MainMenuButton` |
| вход в пещеру | `BoxCollider2D` (Is Trigger) + `SceneTransition` |

### BossSkeleton (Dungeon)

Те же компоненты, что у обычного скелета, плюс `LootDrop` (предмет = `Excalibur`). Характеристики: HP 200, ATK 20, DEF 10.

### BattleManager (BattleScene)

Один объект, на нём сразу: `BattleManager`, `BattleUI`, `BattleHUD`, `BattlePresentation`, `BattleVFXController`, `BattleAudioController`, `AudioSource`. Внутри — `BattleCanvas` со всеми элементами интерфейса боя.

## 2.4 Данные (ScriptableObject-ассеты)

| Ассет | Класс | Значения |
|---|---|---|
| `Settings/Battle/DefaultBattleRules` | `BattleRules` | множители урона, крит ×1.5, защита ×0.5, пауза 0.8 с |
| `Settings/Battle/Spells/Fire` | `BattleSpell` | 8 MP, сила 10 |
| `Settings/Battle/Items/Potion` | `BattleConsumable` | +30 HP |
| `Settings/Inventory/IronSword` | `ItemData` | оружие, ATK +5 |
| `Settings/Inventory/LeatherArmor` | `ItemData` | броня, DEF +4 |
| `Settings/Inventory/Excalibur` | `ItemData` | оружие, ATK +10, выпадает из босса. Иконка пока как у Iron Sword |
| `Settings/Battle/Audio/DefaultBattleAudio` | `BattleAudioProfile` | звуки боя: Attack, Defend, Flee, Fire |

Характеристики в сцене: **Player** — HP 100, ATK 15, DEF 8, MP 30, MATK 12, MDEF 8. **Skeleton** — HP 60, ATK 12, DEF 5, MDEF 3.

## 2.5 Папки

```
Assets/
  Scripts/              ← игровые скрипты
    Battle/             ← бой (19 файлов)
    Inventory/          ← инвентарь, экипировка, лут (10)
    Dangeon/            ← переходы между сценами (2)
    UI/                 ← главное меню, пауза, полоса маны (3)
    PlayerController.cs, EnemyAI.cs, Health.cs, CameraFollow.cs
  Tests/                ← автотесты (EditMode и PlayMode)
  Scenes/               ← 4 сцены
  Settings/             ← ScriptableObject-ассеты (правила, предметы, заклинания)
  Art/, Animations/, Sounds/
```

## 2.6 Архитектурная идея одной фразой

**Данные** (`ItemData`, `BattleSpell`, `BattleRules`) лежат в ассетах, **логика без Unity** (`InventoryState`, `BattleCalculator`) — в обычных классах, **компоненты** (`BattleManager`, `InventoryWindow`) связывают логику с объектами сцены, а **UI только показывает** — ничего не решает сам.
