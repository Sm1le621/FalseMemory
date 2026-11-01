# Состав сцен и взаимодействие объектов

Актуально на **2026-10-31**. Документ описывает сохранённые объекты трёх сцен, компоненты и связи. Источник истины для UI, текста и эффектов — сцены, а не код, создающий объекты в рантайме.

## 1. Общее правило

- Canvas, панели, кнопки, подписи, портреты, частицы VFX и затемнение заранее сохранены в сценах.
- Скрипты читают ссылки и меняют состояние: текст, цвет, позицию, `SetActive`, значения полос.
- Игровой текст — английские строки прямо в TMP-компонентах сцен. Unity StringTable, `LocalizedLabel` и `GameText` удалены.
- Код не создаёт GameObject интерфейса; исключение — временные `Sprite`/`Texture2D` для отображения кадров.

## 2. Сцена MainMenu

```
Main Camera
Global Light 2D
EventSystem                      EventSystem + InputSystemUIInputModule
MainMenuCanvas                   Canvas + CanvasScaler + GraphicRaycaster
  Background                     Image (18,12,28)
  MenuRoot                       VerticalLayoutGroup, 360×280
    Title                        TMP "FALSE MEMORY"
    StartButton                  Image + Outline + Button + Label "NEW GAME"
    SettingsButton               Image + Outline + Button + Label "SETTINGS"
    ExitButton                   Image + Outline + Button + Label "EXIT"
  SettingsDim                    Image (чёрный 65%), изначально выключен
    SettingsPanel                Image + Outline + VerticalLayoutGroup, 380×280
      SettingsTitle              TMP "SETTINGS"
      VolumeLabel                TMP "VOLUME"
      VolumeSlider               Slider (Fill Area/Fill, Handle Slide Area/Handle)
      FullscreenButton           Image + Outline + Button + Label
      BackButton                 Image + Outline + Button + Label "BACK"
Menu                             MainMenuController + SettingsMenu
```

Связи:

- `MainMenuController.gameSceneName = SampleScene`; `settingsMenu` указывает на `SettingsMenu` того же объекта.
- `SettingsMenu.root = SettingsDim`, `volumeSlider = VolumeSlider`, `fullscreenLabel` — подпись кнопки Fullscreen.
- Persistent-события: Start → `OnStart`, Settings → `OnSettings`, Exit → `OnExit`, VolumeSlider → `OnVolumeChanged`, Fullscreen → `ToggleFullscreen`, Back → `Hide`.
- `SettingsMenu.Start` скрывает `SettingsDim` и применяет `PlayerPrefs` (`FalseMemory.Volume`, `FalseMemory.Fullscreen`).
- Canvas: Scale With Screen Size 640×360, Match Height; шрифт — `Assets/Settings/UI/PixelFont.asset`.

Поток: загрузка `MainMenu` → `SettingsMenu.Start` скрывает настройки → New Game грузит `SampleScene`, Settings показывает панель, Exit завершает приложение/Play Mode.

## 3. Сцена SampleScene (исследование)

```
Main Camera
Global Light 2D
Player                    SpriteRenderer, Rigidbody2D, Collider2D, Health, BattleUnit,
                          PlayerController, BattleInventory, InventoryWindow,
                          WorldInputGate, EquipmentController, WorldItemUseController
Grid/Tilemap
Bushs, Borders
Enemy                     SpriteRenderer, Rigidbody2D, 2 Collider2D, Health, BattleUnit,
                          EnemyAI, EnemyAttack (выключен), BattleEncounter,
                          EnemySpriteAnimation
Health                    Slider + Fill/HPText, ManaSlider + Fill/MPText, ManaHUD
EventSystem
BattleTransition          BattleTransition
  BattleFadeCanvas        Canvas (ScreenSpaceOverlay, sortingOrder 1000) + BattleScreenFade
    Fade                  Image (alpha 0, raycastTarget=false)
InventoryCanvas           Canvas + CanvasScaler + GraphicRaycaster
  InventoryBackdrop       Image (модальное затемнение)
  InventoryPanel          Image + Outline; BagPanel, Title, BagTitle, Gold,
                          Slot0…Slot29, Details, Resources, Message, Use/Close/Equip,
                          EquipmentWeapon, EquipmentChest, EquipmentStats
  InventoryTooltip        Image + TMP Content
  DragIcon                Image (выключен)
```

Связи:

- `BattleTransition.fade` указывает на `BattleFadeCanvas/Fade`; `BattleScreenFade.panel` — на тот же Image.
- Transition при встрече затемняет экран, скрывает корни мира, аддитивно грузит `BattleScene`, возвращает/выгружает её и восстанавливает мир.
- `InventoryWindow` получает ссылки на панель, 30 слотов, тексты, кнопки, `DragIcon`, `InventoryTooltip`, `InventoryBackdrop`, `EquipmentStats` и массив из двух кнопок экипировки.
- `EquipmentWeapon.slot = Weapon`, `EquipmentChest.slot = Chest`; у обоих `EquipmentSlotView` назначен `border` (`Outline`), `icon`, `background`, `slotLabel` и `owner`.
- `ManaHUD` живёт на объекте `Health`, ссылается на `ManaSlider`, `MPText` и `BattleUnit` игрока. HP-текст/полоса и MP-текст/полоса читают те же `Health`/`BattleUnit`.
- `PlayerController` в покое показывает `idleSprite = Idle1.ase`; Animator включается только при движении и содержит `Player_Walk`. Idle-клипа нет.
- `EnemySpriteAnimation` читает позицию и `Health`, меняя только `SpriteRenderer` по `SpriteAnimationSet` (`Skeleton64`); idle-кадров в наборе нет.
- Объектов `TestLoot`, `InteractionPrompt`, `PickupNotification`, `CharacterPanel`, портрета и лишних слотов экипировки больше нет.

## 4. Сцена BattleScene (бой)

```
Battle Camera
Battle Light
BattleEventSystem          EventSystem + InputSystemUIInputModule
BattleManager              BattleManager, BattleUI, BattleHUD, BattlePresentation,
                           BattleVFXController, BattleAudioController, AudioSource
  BattleCanvas             Canvas (Scale With Screen Size 960×540, Match Height)
    Backdrop
    BattleContent
      EncounterPanel, BattleMessage, Round, SessionResources
      Battlefield
      PlayerBattleSprite   Image-портрет игрока
      SkeletonBattleSprite Image-портрет врага
      PlayerGuard, EnemyGuard
      StatusPanel          PlayerStatus, EnemyStatus, PlayerHPBar/Fill,
                           PlayerMPBar/Fill, LowHealth
      CommandPanel         CommandRows: AttackButton, Command3 (Magic),
                           Command4 (Defend), Command5 (Flee), Back, MenuTitle
      Controls
      BattleResults        VictoryResultText
      AbilityGlow, AbilityAmount
      VFXLayer             PixelSpellFrame + BattleParticle0…23 (выключены)
```

Связи:

- `BattleUI` содержит тексты, портреты, четыре кнопки команд и подписи, Back, `BattleResults`, `AbilityGlow/AbilityAmount`, `BattleHUD`, `BattleAudioController`.
- `BattleHUD` содержит три `Slider` (HP/MP игрока, HP врага), `SessionResources`, `LowHealth`, `MenuTitle`, `Controls`, `CommandRows` и `CanvasGroup` меню.
- `BattlePresentation` двигает портреты, читает `SpriteAnimationSet` (`Skeleton64`) и `BattleUnit`, показывает `PlayerGuard`/`EnemyGuard`.
- `BattleVFXController` использует `PixelSpellFrame` и 24 `BattleParticle*`; профили `FireVisual`/`HealVisual` — ScriptableObject. `HealVisual` остаётся, но заклинания Heal больше нет.
- `BattleManager` — единственный владелец хода, урона, MP, предметов и исхода; UI/анимации только отображают результат. Наград EXP нет.

## 5. Полный поток

1. `MainMenu` → New Game → `SampleScene`.
2. Игрок ходит (`PlayerController`), открывает инвентарь (`InventoryWindow`), экипирует Weapon/Chest, использует Potion.
3. Контакт с врагом → `BattleEncounter` → `BattleTransition.TryBegin` → fade → аддитивная `BattleScene`.
4. `BattleManager` получает сохранённых `Player`/`Enemy`, ведёт раунды; UI/HUD/Presentation/VFX/Audio показывают бой. Команды: Attack, Magic, Defend, Flee.
5. Победа → `ReturnAfterVictory` → выгрузка `BattleScene`; поражение → `ReturnToMainMenu`; побег → `ReturnAfterEscape` с защитой.
6. Рестарт/меню используют `BattleScreenFade`, который переживает Single-загрузку и удаляется после проявления.

## 6. Где что менять

- Текст и подписи — прямо в TMP-компонентах сцены, по-английски.
- Позиции/размеры/цвета UI — в сцене, без Build-кода.
- Новые VFX — новый `BattleVFXProfile`; частицы/кадр уже есть в `VFXLayer`.
- Новые позы/кадры — `SpriteAnimationSet` и PNG 64×64.
- Новые команды боя — кнопка в `CommandRows`, ссылка в `BattleUI.commandButtons/commandLabels`, обработка в `BattleManager`.
- Новый предмет — ScriptableObject `ItemData`/`BattleConsumable`, добавить в `startingItems` или получить по сюжету (подбора с земли нет).

## 7. Проверки

- EditMode **59/59**, PlayMode **36/36** (2026-10-31), 0 failed/skipped.
- MCP: Missing Script = 0; `ManaHUD` ссылки заполнены; `equipmentButtons.Length == 2`; `slots.Length == 30`; `border` назначен; в `BattleScene` нет Items/Heal/LV/EXP.
- PlayMode покрывает меню/настройки, инвентарь, экипировку, MP-полосу, вход в бой, позы, VFX, победу/поражение/побег и HUD.
- Ручная визуальная оценка меню, полосы MP и эффектов остаётся за владельцем.
