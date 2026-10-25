# Состав сцен и взаимодействие объектов

Документ описывает, из каких объектов состоят три сцены FalseMemory, какие компоненты на них лежат и как объекты связаны между собой. Источник истины для UI, текста и эффектов — сохранённые сцены, а не код, создающий объекты в рантайме.

## 1. Общее правило

- Все видимые объекты (Canvas, панели, кнопки, подписи, портреты, частицы VFX, затемнение) заранее сохранены в сценах.
- Скрипты только читают ссылки и меняют состояние: текст, цвет, позицию, `SetActive`, значения полос.
- Исключение — создание временных `Sprite`/`Texture2D` для точечного отображения в `BattleUI`; GameObject они не создают.
- Пользовательский текст хранится в Unity StringTable; статические подписи получают компонент `LocalizedLabel` с ключом.

## 2. Сцена MainMenu

```
Main Camera
Global Light 2D
EventSystem                      EventSystem + InputSystemUIInputModule
MainMenuCanvas                   Canvas + CanvasScaler + GraphicRaycaster
  Background                     Image (18,12,28)
  MenuRoot                       VerticalLayoutGroup, 360×280
    Title                        TMP + LocalizedLabel (game.title)
    StartButton                  Image + Outline + Button + Label (menu.new_game)
    SettingsButton               Image + Outline + Button + Label (menu.settings)
    ExitButton                   Image + Outline + Button + Label (menu.exit)
  SettingsDim                    Image (чёрный 65%), изначально выключен
    SettingsPanel                Image + Outline + VerticalLayoutGroup, 380×280
      SettingsTitle              TMP + LocalizedLabel (menu.settings)
      VolumeLabel                TMP + LocalizedLabel (settings.volume)
      VolumeSlider               Slider (Fill Area/Fill, Handle Slide Area/Handle)
      FullscreenButton           Image + Outline + Button + Label
      BackButton                 Image + Outline + Button + Label (menu.back)
Menu                             MainMenuController + SettingsMenu
```

Связи:

- `MainMenuController.gameSceneName = SampleScene`; `settingsMenu` указывает на `SettingsMenu` того же объекта.
- `SettingsMenu.root = SettingsDim`, `volumeSlider = VolumeSlider`, `fullscreenLabel = Label` кнопки `FullscreenButton`.
- Persistent-события: `StartButton → OnStart`, `SettingsButton → OnSettings`, `ExitButton → OnExit`, `VolumeSlider → OnVolumeChanged`, `FullscreenButton → ToggleFullscreen`, `BackButton → Hide`.
- `SettingsMenu.Start` выключает `SettingsDim` и применяет `PlayerPrefs` (`FalseMemory.Volume`, `FalseMemory.Fullscreen`).
- Шрифт всех подписей — `Assets/Settings/UI/PixelFont.asset` (dynamic, Point filter), фон панелей — `Assets/Art/UI/WhitePixel.png`.
- Масштаб Canvas: Scale With Screen Size, 640×360, Match Height; это заменяет прежний расчёт `floor(Screen.height/360)` и даёт тот же коэффициент на типовых разрешениях.

Поток: загрузка `MainMenu` → `LocalizedLabel` применяет английские ключи → `SettingsMenu.Start` скрывает настройки → New Game грузит `SampleScene`, Settings показывает панель, Exit завершает приложение/Play Mode.

## 3. Сцена SampleScene (исследование)

```
Main Camera
Global Light 2D
Player                    SpriteRenderer, Rigidbody2D, Collider2D, Health, BattleUnit,
                          PlayerController, BattleInventory, InventoryWindow,
                          WorldInputGate, PlayerInteractor, EquipmentController,
                          WorldItemUseController, CameraFollow
Grid/Tilemap
Bushs, Borders
Enemy                     SpriteRenderer, Rigidbody2D, 2 Collider2D, Health, BattleUnit,
                          EnemyAI, EnemyAttack, BattleEncounter, EnemySpriteAnimation
TestLoot                  Pickup_* (WorldItemPickup + Collider2D + LocalizedLabel)
EventSystem
BattleTransition          BattleTransition
  BattleFadeCanvas        Canvas (ScreenSpaceOverlay, sortingOrder 1000) + BattleScreenFade
    Fade                  Image (alpha 0, raycastTarget=false)
Health (у Enemy)          Slider + Fill/HPText (мировой бар)
InventoryCanvas           Canvas + CanvasScaler + GraphicRaycaster
  InventoryBackdrop       Image (модальное затемнение)
  InventoryPanel          Image + Outline; BagPanel, CharacterPanel, PortraitFrame,
                          Title, Gold, Slot0…Slot29, Details, Resources, Message,
                          Use/Split/Close/Equip, Equipment* (7 слотов), EquipmentStats
  InventoryTooltip        Image + TMP Content
  InteractionPrompt       TMP
  PickupNotification      TMP
  DragIcon                Image (выключен)
```

Связи:

- `BattleTransition.fade` указывает на `BattleFadeCanvas/Fade`; `BattleScreenFade.panel` — на тот же Image.
- `BattleTransition` при встрече затемняет экран, скрывает корни мира, аддитивно грузит `BattleScene`, возвращает/выгружает её и восстанавливает мир.
- `PlayerInteractor.prompt/notification` ссылаются на `InteractionPrompt`/`PickupNotification`.
- `InventoryWindow` получает ссылки на панель, 30 слотов, подписи, кнопки, `DragIcon`, `InventoryTooltip`, `InventoryBackdrop`, портрет и характеристики.
- `EnemySpriteAnimation` читает позицию и `Health`, меняя только `SpriteRenderer` по `SpriteAnimationSet` (Skeleton64).
- Мировой `Health`-бар — отдельный объект `Health` под `Enemy`; боевой UI его не дублирует.

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
      StatusPanel          PlayerStatus, EnemyStatus, PlayerHPBar/Fill, PlayerMPBar/Fill,
                           LowHealth
      CommandPanel         CommandRows (AttackButton, DefendButton, Command3…5),
                           MenuTitle
      Controls
      BattleResults        VictoryResultText
      AbilityGlow, AbilityAmount
      VFXLayer             PixelSpellFrame + BattleParticle0…23 (все выключены)
```

Связи:

- `BattleUI` содержит ссылки на тексты, портреты, пять кнопок/подписей, `BattleResults`, `AbilityGlow/AbilityAmount`, `BattleHUD`, `BattleAudioController`.
- `BattleHUD` содержит три `Slider` (HP/MP врага), `SessionResources`, `LowHealth`, `MenuTitle`, `Controls`, `CommandRows` и `CanvasGroup` меню.
- `BattlePresentation` двигает `PlayerBattleSprite`/`SkeletonBattleSprite`, читает `SpriteAnimationSet` (Skeleton64) и `BattleUnit`, показывает `PlayerGuard`/`EnemyGuard`.
- `BattleVFXController` использует `PixelSpellFrame` и 24 `BattleParticle*` из `VFXLayer`; профили `FireVisual`/`HealVisual` — ScriptableObject.
- `VFXLayer` — последний sibling `BattleContent`, anchor (0,0), pivot (0.5,0.5), size 0. Поэтому координаты частиц совпадают с координатами портретов; не перемещайте слой в другой RectTransform.
- `BattleManager` — единственный владелец хода, урона, MP, предметов и наград; UI/анимации только отображают результат.

## 5. Полный поток

1. `MainMenu` → New Game → `SampleScene`.
2. Игрок ходит (`PlayerController`), подбирает предметы (`PlayerInteractor`/`WorldItemPickup`), открывает инвентарь (`InventoryWindow`).
3. Контакт с врагом → `BattleEncounter` → `BattleTransition.TryBegin` → fade → аддитивная `BattleScene`.
4. `BattleManager` создаёт `BattleUnit`-ссылки из сохранённых `Player`/`Enemy`, ведёт раунды; `BattleUI`/`BattleHUD`/`BattlePresentation`/`BattleVFXController`/`BattleAudioController` показывают бой.
5. Победа → `ReturnAfterVictory` + EXP → выгрузка `BattleScene`; поражение → `ReturnToMainMenu`; побег → `ReturnAfterEscape` с защитой.
6. Рестарт/меню используют `BattleScreenFade`, который переживает Single-загрузку и удаляется после проявления.

## 6. Где что менять

- Текст и подписи — StringTable + `LocalizedLabel.key` в сцене.
- Позиции/размеры/цвета UI — в сцене, без Build-кода.
- Новые VFX — новый `BattleVFXProfile`; частицы/кадр уже есть в `VFXLayer`.
- Новые позы/кадры — `SpriteAnimationSet` и PNG 64×64.
- Новые команды боя — кнопка в `CommandRows`, ссылка в `BattleUI.commandButtons/commandLabels`, обработка в `BattleManager`.

## 7. Проверки

- EditMode: 82/82, PlayMode: 42/42 (2026-10-25).
- PlayMode покрывает меню/настройки, инвентарь, вход в бой, анимации поз, VFX, победу/поражение/побег, HUD и локализацию.
- Ручная визуальная оценка меню и эффектов остаётся за владельцем.
