# UI

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [HomeScreen](#homescreen) | MonoBehaviour | Nilly | The home screen in the Home scene: a title page with Play and Quit, and the level select page Play opens. The level buttons on that page carry their own LevelBu… |
| [ControlsOverlay](#controlsoverlay) | MonoBehaviour | Nilly | A developer help box on the HUD listing the keys and clicks, from UiStrings. Opens with the tutorial level only (ShowsControlsHelp), since on later maps it cove… |
| [HoneydewLabel](#honeydewlabel) | MonoBehaviour (HUD listener) | Nilly | Shows the run's Honeydew. Updates only when the amount changes. Also sits on the balance box of Adam's shop list, which shows the number alone. |
| [HudPanel](#hudpanel) | MonoBehaviour (HUD listener) | Nilly | Shows the HUD only while a level runs, so the home screen is not drawn over by a stale Honeydew count and wave label. Sits on the canvas, above the HUD object i… |
| [LevelRunListener](#levelrunlistener) | MonoBehaviour | Nilly | Base for HUD and menu pieces in the persistent UI scene. They cannot hold a serialized reference into a level scene, so this follows Started and Ended and hands… |
| [NextWaveButton](#nextwavebutton) | MonoBehaviour (HUD listener) | Nilly | The Next Wave button. Sends the current run's next wave, the same as the Space key, and is greyed out whenever that would do nothing: while a wave is still comi… |
| [PauseMenu](#pausemenu) | MonoBehaviour (HUD listener) | Nilly | The pause panel: shown while the run's clock is paused and the run is still going. Its buttons work at time scale zero because uGUI buttons ignore Time.timeScal… |
| [PowerUpSlotPanel](#powerupslotpanel) | MonoBehaviour (HUD listener) | Nilly | The HUD's one power-up slot: a button with the held power-up's icon and name. Clicking it uses the power-up through the run. While a Royal Gift waits for a towe… |
| [QueenHealthBar](#queenhealthbar) | MonoBehaviour (HUD listener) | Nilly | A bar that follows the Queen's health through her health-changed event. |
| [QueenMantisBossBar](#queenmantisbossbar) | MonoBehaviour | Nilly | The Queen Mantis's health bar, shown only while she walks. Marks at the thirds show where she changes shade; the fill takes her shade's colour and the label nam… |
| [ResultPanel](#resultpanel) | MonoBehaviour (HUD listener) | Nilly | The one result panel for both outcomes: a title that says won or lost, Restart and Home. |
| [ShopPlacerBinding](#shopplacerbinding) | MonoBehaviour (HUD listener) | Nilly | Keeps Adam's shop cards pointed at the running level's TowerPlacer. A card finds the placer once, in its Start, so after Restart or the next level it would stil… |
| [TimedNotice](#timednotice) | Class | Nilly | A short message that stands in for a HUD label for a few seconds, such as "Already max level". Plain C# so EditMode tests can cover it. The caller passes the ti… |
| [TowerCardPanel](#towercardpanel) | MonoBehaviour (HUD listener) | Nilly | The card for the selected standing tower: its name and level, an upgrade button with the price, the burrow order, and the standing order (First / Last / Closest… |
| [UiStrings](#uistrings) | Static class | Nilly | Every string the HUD and menus show, in one place, so they can be checked and later localized. |
| [WaveLabel](#wavelabel) | MonoBehaviour (HUD listener) | Nilly | Shows "Wave 3 / 10". Reads 0 before the first wave is sent. |
| [TowerCardUI](#towercardui) | MonoBehaviour | Adam |  |
| [TowerShopManager](#towershopmanager) | MonoBehaviour | Adam |  |

### HomeScreen

`unity/Assets/_Game/Features/UI/Scripts/Home/HomeScreen.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Home/HomeScreen.cs)) · MonoBehaviour · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

The home screen in the Home scene: a title page with Play and Quit, and the level select page Play opens. The level buttons on that page carry their own LevelBu…

Derives from / implements: MonoBehaviour

Used on: Home

Named in: AlphaHomeAndLevelsBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_titlePage` | `GameObject` |  |  | Home = Canvas/Home Screen/Title Page | The page with the title, Play and Quit. Shown when the scene opens. |
| `_levelSelectPage` | `GameObject` |  |  | Home = Canvas/Home Screen/Level Select Page | The page with one button per level and Back. |
| `_play` | `Button` |  |  | Home = Canvas/Home Screen/Title Page/Play (Button) |  |
| `_quit` | `Button` |  |  | Home = Canvas/Home Screen/Title Page/Quit (Button) |  |
| `_back` | `Button` |  |  | Home = Canvas/Home Screen/Level Select Page/Back (Button) |  |

Private members not listed: 5.

### ControlsOverlay

`unity/Assets/_Game/Features/UI/Scripts/Hud/ControlsOverlay.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/ControlsOverlay.cs)) · MonoBehaviour · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

A developer help box on the HUD listing the keys and clicks, from UiStrings. Opens with the tutorial level only (ShowsControlsHelp), since on later maps it cove…

Derives from / implements: MonoBehaviour

Used on: UI

Named in: AlphaAlignmentBuilder, AlphaSellAndHelpBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_panel` | `GameObject` |  |  | UI = Canvas/HUD/Controls Help | The box to show or hide. |
| `_text` | `TMP_Text` |  |  | UI = Canvas/HUD/Controls Help/Text (TextMeshProUGUI) | The text inside the box; filled from UiStrings on enable. |
| `_actions` | `InputActionAsset` |  |  | UI = HoldTheHill | The game's actions asset. Uses its Gameplay map: ToggleHelp. |

Private members not listed: 7.

### HoneydewLabel

`unity/Assets/_Game/Features/UI/Scripts/Hud/HoneydewLabel.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/HoneydewLabel.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

Shows the run's Honeydew. Updates only when the amount changes. Also sits on the balance box of Adam's shop list, which shows the number alone.

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaRunFrameBuilder, AlphaShopBindingBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_amountOnly` | `bool` |  |  | UI = 0, 1 | Show only the number, for a box that already says what it holds. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 4.

### HudPanel

`unity/Assets/_Game/Features/UI/Scripts/Hud/HudPanel.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/HudPanel.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

Shows the HUD only while a level runs, so the home screen is not drawn over by a stale Honeydew count and wave label. Sits on the canvas, above the HUD object i…

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaPowerUpsBuilder, AlphaRunFrameBuilder, QueenMantisBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_hud` | `GameObject` |  |  | UI = Canvas/HUD | The HUD root (Honeydew, wave, Queen bar, Next Wave). Inactive between levels. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnEnable` | Method (protected override) | `void` | `()` |  |  |
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

### LevelRunListener

`unity/Assets/_Game/Features/UI/Scripts/Hud/LevelRunListener.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/LevelRunListener.cs)) · MonoBehaviour · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

Base for HUD and menu pieces in the persistent UI scene. They cannot hold a serialized reference into a level scene, so this follows Started and Ended and hands…

Derives from / implements: MonoBehaviour

Named in: HoneydewLabel, HudPanel, NextWaveButton, PauseMenu, PowerUpSlotPanel, QueenHealthBar, ResultPanel, ShopPlacerBinding, TowerCardPanel, WaveLabel

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Run` | Property (protected) | `LevelRun` | `=> _run` | The run this piece shows, or null between levels. |  |
| `OnEnable` | Method (protected virtual) | `void` | `()` |  | HudPanel, PauseMenu, PowerUpSlotPanel, ResultPanel, TowerCardPanel |
| `OnDisable` | Method (protected virtual) | `void` | `()` |  | PauseMenu, PowerUpSlotPanel, ResultPanel, TowerCardPanel |
| `Bind` | Method (protected abstract) | `void` | `(LevelRun run)` | Subscribe to run and show its current state. | HoneydewLabel, HudPanel, NextWaveButton, PauseMenu, PowerUpSlotPanel, QueenHealthBar, ResultPanel, ShopPlacerBinding, TowerCardPanel, WaveLabel |
| `Unbind` | Method (protected abstract) | `void` | `(LevelRun run)` | Undo Bind. The run's scene objects may already be destroyed. | HoneydewLabel, HudPanel, NextWaveButton, PauseMenu, PowerUpSlotPanel, QueenHealthBar, ResultPanel, ShopPlacerBinding, TowerCardPanel, WaveLabel |

Private members not listed: 3.

### NextWaveButton

`unity/Assets/_Game/Features/UI/Scripts/Hud/NextWaveButton.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/NextWaveButton.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

The Next Wave button. Sends the current run's next wave, the same as the Space key, and is greyed out whenever that would do nothing: while a wave is still comi…

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaRunFrameBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 4.

### PauseMenu

`unity/Assets/_Game/Features/UI/Scripts/Hud/PauseMenu.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/PauseMenu.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

The pause panel: shown while the run's clock is paused and the run is still going. Its buttons work at time scale zero because uGUI buttons ignore Time.timeScal…

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaRunFrameBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_panel` | `GameObject` |  |  | UI = Canvas/Pause Menu/Panel | The panel to show while paused. Inactive when the game starts. |
| `_resume` | `Button` |  |  | UI = Canvas/Pause Menu/Panel/Resume (Button) |  |
| `_restart` | `Button` |  |  | UI = Canvas/Pause Menu/Panel/Restart (Button) |  |
| `_home` | `Button` |  |  | UI = Canvas/Pause Menu/Panel/Home (Button) |  |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnEnable` | Method (protected override) | `void` | `()` |  |  |
| `OnDisable` | Method (protected override) | `void` | `()` |  |  |
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 6.

### PowerUpSlotPanel

`unity/Assets/_Game/Features/UI/Scripts/Hud/PowerUpSlotPanel.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/PowerUpSlotPanel.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

The HUD's one power-up slot: a button with the held power-up's icon and name. Clicking it uses the power-up through the run. While a Royal Gift waits for a towe…

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaPowerUpsBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_button` | `Button` |  |  | UI = Canvas/HUD/Power-up Slot (Button) |  |
| `_icon` | `Image` |  |  | UI = Canvas/HUD/Power-up Slot/Icon (Image) |  |
| `_label` | `TMP_Text` |  |  | UI = Canvas/HUD/Power-up Slot/Text (TextMeshProUGUI) |  |
| `_noticeSeconds` | `float` | `2f` |  | UI = 2 | How long the 'already max level' and 'slot full' notices stay, in unscaled seconds. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnEnable` | Method (protected override) | `void` | `()` |  |  |
| `OnDisable` | Method (protected override) | `void` | `()` |  |  |
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 9.

### QueenHealthBar

`unity/Assets/_Game/Features/UI/Scripts/Hud/QueenHealthBar.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/QueenHealthBar.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

A bar that follows the Queen's health through her health-changed event.

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaAlignmentBuilder, AlphaRunFrameBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_fill` | `Image` |  |  | UI = Canvas/HUD/Queen Bar/Fill (Image) | A filled Image; its fill amount is health divided by max health. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 2.

### QueenMantisBossBar

`unity/Assets/_Game/Features/UI/Scripts/Hud/QueenMantisBossBar.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/QueenMantisBossBar.cs)) · MonoBehaviour · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

The Queen Mantis's health bar, shown only while she walks. Marks at the thirds show where she changes shade; the fill takes her shade's colour and the label nam…

Derives from / implements: MonoBehaviour

Used on: UI

Named in: QueenMantisBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_panel` | `GameObject` |  |  | UI = Canvas/HUD/Queen Mantis Bar/Panel | Everything the bar draws, on a child, hidden while no Queen Mantis walks; this component stays on to listen. |
| `_fill` | `Image` |  |  | UI = Canvas/HUD/Queen Mantis Bar/Panel/Bar/Fill (Image) | A filled Image; its fill amount is her health over her maximum. |
| `_label` | `TMP_Text` |  |  | UI = Canvas/HUD/Queen Mantis Bar/Panel/Label (TextMeshProUGUI) | Her name and what her shade does. |
| `_whiteFill` | `Color` | `new Color(0.93f, 0.93f, 0.85f)` |  | UI = (0.93, 0.93, 0.85, 1) | Fill colour in her white shade. |
| `_blackFill` | `Color` | `new Color(0.55f, 0.35f, 0.75f)` |  | UI = (0.55, 0.35, 0.75, 1) | Fill colour in her black shade; light enough to read on the dark bar. |

Private members not listed: 10.

### ResultPanel

`unity/Assets/_Game/Features/UI/Scripts/Hud/ResultPanel.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/ResultPanel.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

The one result panel for both outcomes: a title that says won or lost, Restart and Home.

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaRunFrameBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_panel` | `GameObject` |  |  | UI = Canvas/Result/Panel | The panel to show when the run ends. Inactive when the game starts. |
| `_title` | `TMP_Text` |  |  | UI = Canvas/Result/Panel/Title (TextMeshProUGUI) | Reads the victory or defeat line. |
| `_restart` | `Button` |  |  | UI = Canvas/Result/Panel/Restart (Button) |  |
| `_home` | `Button` |  |  | UI = Canvas/Result/Panel/Home (Button) |  |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnEnable` | Method (protected override) | `void` | `()` |  |  |
| `OnDisable` | Method (protected override) | `void` | `()` |  |  |
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 3.

### ShopPlacerBinding

`unity/Assets/_Game/Features/UI/Scripts/Hud/ShopPlacerBinding.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/ShopPlacerBinding.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

Keeps Adam's shop cards pointed at the running level's TowerPlacer. A card finds the placer once, in its Start, so after Restart or the next level it would stil…

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaShopBindingBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 1.

### TimedNotice

`unity/Assets/_Game/Features/UI/Scripts/Hud/TimedNotice.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/TimedNotice.cs)) · Class · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

A short message that stands in for a HUD label for a few seconds, such as "Already max level". Plain C# so EditMode tests can cover it. The caller passes the ti…

Named in: PowerUpSlotPanel, TimedNoticeTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Show` | Method | `void` | `(string text, float now, float seconds)` | Shows text from now for seconds, replacing any notice already up. | PowerUpSlotPanel, TimedNoticeTests |
| `TextAt` | Method | `string` | `(float now)` | The message at now, or null once it has run out or was cleared. | PowerUpSlotPanel, TimedNoticeTests |
| `Clear` | Method | `void` | `()` | Drops the message, so it does not carry into the next run. | PowerUpSlotPanel, TimedNoticeTests |

Private members not listed: 2.

### TowerCardPanel

`unity/Assets/_Game/Features/UI/Scripts/Hud/TowerCardPanel.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/TowerCardPanel.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

The card for the selected standing tower: its name and level, an upgrade button with the price, the burrow order, and the standing order (First / Last / Closest…

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaAlignmentBuilder, AlphaHazardsBuilder, AlphaSellAndHelpBuilder, AlphaTowersBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_panel` | `GameObject` |  |  | UI = Canvas/HUD/Tower Card | The card root. Inactive while no tower is selected. |
| `_name` | `TMP_Text` |  |  | UI = Canvas/HUD/Tower Card/Name (TextMeshProUGUI) |  |
| `_level` | `TMP_Text` |  |  | UI = Canvas/HUD/Tower Card/Level (TextMeshProUGUI) |  |
| `_upgrade` | `Button` |  |  | UI = Canvas/HUD/Tower Card/Upgrade Button (Button) |  |
| `_upgradeLabel` | `TMP_Text` |  |  | UI = Canvas/HUD/Tower Card/Upgrade Button/Text (TextMeshProUGUI) |  |
| `_burrow` | `Button` |  |  | UI = Canvas/HUD/Tower Card/Burrow Button (Button) | Burrows the tower or brings it back up. Its label says which. |
| `_burrowLabel` | `TMP_Text` |  |  | UI = Canvas/HUD/Tower Card/Burrow Button/Text (TextMeshProUGUI) |  |
| `_sell` | `Button` |  |  | UI = Canvas/HUD/Tower Card/Sell Button (Button) | Sells the tower for part of what was spent on it. Its label shows the refund. |
| `_sellLabel` | `TMP_Text` |  |  | UI = Canvas/HUD/Tower Card/Sell Button/Text (TextMeshProUGUI) |  |
| `_orderButtons` | `Button[]` | `new Button[3]` |  | UI = 3 item(s) | One button per standing order, in enum order: First, Last, Closest. |
| `_orderTitle` | `GameObject` |  |  | UI = Canvas/HUD/Tower Card/Order Title | The 'Target' heading over the order buttons; hidden with them for a tower that doesn't aim. |
| `_topLeftOffset` | `Vector2` | `new Vector2(24f, -88f)` |  | UI = (24, -88) | Where the card sits, from the screen's top-left corner, while the selected tower is in the bottom half. Otherwise it keeps its bottom-left place from the scene. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnEnable` | Method (protected override) | `void` | `()` |  |  |
| `OnDisable` | Method (protected override) | `void` | `()` |  |  |
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 17.

### UiStrings

`unity/Assets/_Game/Features/UI/Scripts/Hud/UiStrings.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/UiStrings.cs)) · Static class · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

Every string the HUD and menus show, in one place, so they can be checked and later localized.

Named in: AlphaAlignmentBuilder, AlphaHazardsBuilder, AlphaHomeAndLevelsBuilder, AlphaPowerUpsBuilder, AlphaRunFrameBuilder, AlphaSellAndHelpBuilder, AlphaTowersBuilder, ControlsOverlay, HoneydewLabel, PowerUpSlotPanel, QueenMantisBossBar, QueenMantisBuilder, ResultPanel, TowerCardPanel, WaveLabel

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `HoneydewFormat` | Const | `string` |  |  | AlphaRunFrameBuilder, HoneydewLabel |
| `WaveFormat` | Const | `string` |  |  | AlphaRunFrameBuilder, WaveLabel |
| `QueenHealth` | Const | `string` |  |  | AlphaRunFrameBuilder |
| `NextWave` | Const | `string` |  |  | AlphaRunFrameBuilder |
| `Paused` | Const | `string` |  |  | AlphaRunFrameBuilder |
| `Resume` | Const | `string` |  |  | AlphaRunFrameBuilder |
| `RestartLevel` | Const | `string` |  |  | AlphaRunFrameBuilder |
| `Home` | Const | `string` |  |  | AlphaRunFrameBuilder |
| `Victory` | Const | `string` |  |  | AlphaRunFrameBuilder, ResultPanel |
| `Defeat` | Const | `string` |  |  | ResultPanel |
| `TowerLevelFormat` | Const | `string` |  |  | AlphaTowersBuilder, TowerCardPanel |
| `UpgradeFormat` | Const | `string` |  |  | AlphaTowersBuilder, TowerCardPanel |
| `MaxLevel` | Const | `string` |  |  | TowerCardPanel |
| `StandingOrder` | Const | `string` |  |  | AlphaTowersBuilder |
| `OrderFirst` | Const | `string` |  |  | AlphaTowersBuilder, TowerCardPanel |
| `OrderLast` | Const | `string` |  |  | AlphaTowersBuilder, TowerCardPanel |
| `OrderClosest` | Const | `string` |  |  | AlphaTowersBuilder, TowerCardPanel |
| `CurrentOrderMark` | Const | `string` |  |  | TowerCardPanel |
| `Burrow` | Const | `string` |  |  | AlphaHazardsBuilder, TowerCardPanel |
| `Unburrow` | Const | `string` |  |  | TowerCardPanel |
| `UnknownTower` | Const | `string` |  |  | AlphaTowersBuilder, TowerCardPanel |
| `SellFormat` | Const | `string` |  |  | AlphaSellAndHelpBuilder, TowerCardPanel |
| `ControlsTitle` | Const | `string` |  |  | AlphaSellAndHelpBuilder |
| `string[] ControlsLines =` | Property (static readonly) |  | `{ ... }` |  |  |
| `ControlsText` | Field (static readonly) | `string` |  | The whole help box: the title over one line per control. After ControlsLines, which it reads. | AlphaAlignmentBuilder, ControlsOverlay |
| `PowerUpEmpty` | Const | `string` |  |  | AlphaPowerUpsBuilder, PowerUpSlotPanel |
| `SwarmFrenzy` | Const | `string` |  |  | PowerUpSlotPanel |
| `RoyalGift` | Const | `string` |  |  | PowerUpSlotPanel |
| `PickTowerForGift` | Const | `string` |  |  | PowerUpSlotPanel |
| `GiftAtMaxLevel` | Const | `string` |  |  | PowerUpSlotPanel |
| `SlotFull` | Const | `string` |  |  | PowerUpSlotPanel |
| `FrenzyActiveFormat` | Const | `string` |  |  | PowerUpSlotPanel |
| `MantisBarFormat` | Const | `string` |  |  | QueenMantisBossBar, QueenMantisBuilder |
| `QueenMantis` | Const | `string` |  |  | QueenMantisBossBar, QueenMantisBuilder |
| `MantisWhite` | Const | `string` |  |  | QueenMantisBossBar, QueenMantisBuilder |
| `MantisBlack` | Const | `string` |  |  | QueenMantisBossBar |
| `MantisPraying` | Const | `string` |  |  | QueenMantisBossBar |
| `GameTitle` | Const | `string` |  |  | AlphaHomeAndLevelsBuilder |
| `Play` | Const | `string` |  |  | AlphaHomeAndLevelsBuilder |
| `Quit` | Const | `string` |  |  | AlphaHomeAndLevelsBuilder |
| `ChooseLevel` | Const | `string` |  |  | AlphaHomeAndLevelsBuilder |
| `Back` | Const | `string` |  |  | AlphaHomeAndLevelsBuilder |

### WaveLabel

`unity/Assets/_Game/Features/UI/Scripts/Hud/WaveLabel.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/Scripts/Hud/WaveLabel.cs)) · MonoBehaviour (HUD listener) · `HoldTheHill.Features.UI` · HoldTheHill.Runtime · owner: Nilly

Shows "Wave 3 / 10". Reads 0 before the first wave is sent.

Derives from / implements: LevelRunListener

Used on: UI

Named in: AlphaRunFrameBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Bind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |
| `Unbind` | Method (protected override) | `void` | `(LevelRun run)` |  |  |

Private members not listed: 4.

### TowerCardUI

`unity/Assets/_Game/Features/UI/TowerCardScript/TowerCardUI.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/TowerCardScript/TowerCardUI.cs)) · MonoBehaviour · `global namespace` · HoldTheHill.Runtime · owner: Adam

Derives from / implements: MonoBehaviour

Used on: TowerCardBase

Named in: ShopPlacerBinding, TowerShopManager

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `towerData` | `TowerData` |  |  | TowerCardBase = (none) |  |
| `towerPlacer` | `TowerPlacer` |  |  | TowerCardBase = (none) |  |
| `iconDisplay` | `Image` |  |  | TowerCardBase = TowerCardBase/TowerIconSprite (Image) |  |
| `nameDisplay` | `TextMeshProUGUI` |  |  | TowerCardBase = TowerCardBase/towerName (TextMeshProUGUI) |  |
| `costDisplay` | `TextMeshProUGUI` |  |  | TowerCardBase = TowerCardBase/realcost (TextMeshProUGUI) |  |
| `selectedTint` | `Color` | `new Color(1f, 0.82f, 0.3f, 1f)` | Selection | TowerCardBase = (1, 0.82, 0.3, 1) | The card's background tint while its tower is picked (the ghost is out for it). |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SetupCard` | Method | `void` | `(TowerData data)` |  | TowerShopManager |
| `IsPicked` | Method | `bool` | `()` | True while the placer holds this card's tower, ready to build. |  |
| `OnCardClicked` | Method | `void` | `()` |  |  |

Private members not listed: 6.

### TowerShopManager

`unity/Assets/_Game/Features/UI/TowerListScripts/TowerShopManager.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/TowerListScripts/TowerShopManager.cs)) · MonoBehaviour · `global namespace` · HoldTheHill.Runtime · owner: Adam

Derives from / implements: MonoBehaviour

Used on: TowerList

Named in: AlphaAlignmentBuilder, AlphaShopBindingBuilder, TowerListDataTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `availableTowers` | `List<TowerData>` |  | Configuration | TowerList = 6 item(s) |  |
| `towerCardPrefab` | `GameObject` |  | Configuration | TowerList = TowerCardBase |  |
| `shopContainer` | `Transform` |  | Configuration | TowerList = TowerList/Scroll/View/ShopContainer (RectTransform) |  |
| `shopScrollRect` | `ScrollRect` |  | Configuration | TowerList = TowerList/Scroll (ScrollRect) |  |

Private members not listed: 2.

## Prefabs

### HoneydewCluster

`unity/Assets/_Game/Features/UI/TowerCardAssets/HoneydewCluster.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/TowerCardAssets/HoneydewCluster.prefab)) · Prefab

Made by: Hand-made (no builder names it)

Used in: TowerCardBase, TowerList

**(scene)** · GameObject

| Field | Value | Points to |
|---|---|---|
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |

**HoneydewCluster** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (0, 0) |  |
| `m_SizeDelta` | (100, 100) |  |

**HoneydewCluster/HoneydewSprite2** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (49.4, -10) |  |
| `m_SizeDelta` | (19, 19) |  |

**HoneydewCluster/HoneydewSprite2** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | HoneydewBall.png › HoneydewBall_0 | Sprite: unity/Assets/_Game/Features/UI/TowerCardAssets/HoneydewBall.png |

**HoneydewCluster/honeydewSprite3** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (52.7, -2.5) |  |
| `m_SizeDelta` | (17, 17) |  |

**HoneydewCluster/honeydewSprite3** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | HoneydewBall.png › HoneydewBall_0 | Sprite: unity/Assets/_Game/Features/UI/TowerCardAssets/HoneydewBall.png |

**HoneydewCluster/honeydewSprite** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (41.4, -5.7) |  |
| `m_SizeDelta` | (19, 20) |  |

**HoneydewCluster/honeydewSprite** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | HoneydewBall.png › HoneydewBall_0 | Sprite: unity/Assets/_Game/Features/UI/TowerCardAssets/HoneydewBall.png |

### TowerCardBase

`unity/Assets/_Game/Features/UI/TowerCardAssets/TowerCardBase.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/TowerCardAssets/TowerCardBase.prefab)) · Prefab

Made by: Hand-made (no builder names it)

Used in: TowerList, UITest

**(scene)** · GameObject

| Field | Value | Points to |
|---|---|---|
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |

**TowerCardBase/towerName** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (55.192627, 54.00403) |  |
| `m_SizeDelta` | (-116.0607, -127.0618) |  |

**TowerCardBase/towerName** · TextMeshProUGUI

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_text` | Tower name |  |
| `m_fontSize` | 15 |  |

**TowerCardBase/realcost** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (144, -15.592) |  |
| `m_SizeDelta` | (200, 50) |  |

**TowerCardBase/realcost** · TextMeshProUGUI

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_text` | 350 |  |
| `m_fontSize` | 17 |  |

**TowerCardBase/TowerIconPad** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (-46.442017, 0) |  |
| `m_SizeDelta` | (-116.7362, -22.5654) |  |

**TowerCardBase/TowerIconPad** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 0.03529412) |  |
| `m_Sprite` | TowerIconPad.png › TowerIconPad_0 | Sprite: unity/Assets/_Game/Features/UI/TowerCardAssets/TowerIconPad.png |

**TowerCardBase/Button** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (0.00073862, 0.000025749) |  |
| `m_SizeDelta` | (179.26, 81.183) |  |

**TowerCardBase/Button** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (0.1, 0.1, 0.1, 0.44705883) |  |
| `m_Sprite` | (Unity built-in resource) | Built-in |

**TowerCardBase/Button** · Button

| Field | Value | Points to |
|---|---|---|
| `m_Interactable` | 1 |  |
| `m_OnClick` | 1 listener(s) |  |
| `m_OnClick[0]` | TowerCardBase (TowerCardUI).OnCardClicked() | Component: unity/Assets/_Game/Features/UI/TowerCardAssets/TowerCardBase.prefab |

**TowerCardBase/TowerIconSprite** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (-46.442017, 2.1671753) |  |
| `m_SizeDelta` | (-126.1761, -36.9405) |  |

**TowerCardBase/TowerIconSprite** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | defaultAnt.png › Gemini_Generated_Image_px3nj0px3nj0px3n-Photoroom_0 | Sprite: unity/Assets/_Game/Features/UI/TowerCardAssets/defaultAnt.png |

**TowerCardBase** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (120.88428, -63.20508) |  |
| `m_SizeDelta` | (-2380.7346, -1358.8167) |  |

**TowerCardBase** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | TowerUICardBackground.png › TowerUICardBackground_0 | Sprite: unity/Assets/_Game/Features/UI/TowerCardAssets/TowerUICardBackground.png |

**TowerCardBase** · TowerCardUI

| Field | Value | Points to |
|---|---|---|
| `towerData` | (none) |  |
| `towerPlacer` | (none) |  |
| `iconDisplay` | TowerCardBase/TowerIconSprite (Image) | Component: unity/Assets/_Game/Features/UI/TowerCardAssets/TowerCardBase.prefab |
| `nameDisplay` | TowerCardBase/towerName (TextMeshProUGUI) | Component: unity/Assets/_Game/Features/UI/TowerCardAssets/TowerCardBase.prefab |
| `costDisplay` | TowerCardBase/realcost (TextMeshProUGUI) | Component: unity/Assets/_Game/Features/UI/TowerCardAssets/TowerCardBase.prefab |
| `selectedTint` | (1, 0.82, 0.3, 1) |  |

**TowerCardBase/Button/Text (TMP)** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (0, 0) |  |
| `m_SizeDelta` | (0, 0) |  |

**TowerCardBase/Button/Text (TMP)** · TextMeshProUGUI

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_text` | (empty) |  |
| `m_fontSize` | 24 |  |

**TowerCardBase/HoneydewCluster** · GameObject (override)

| Field | Override | Base value |
|---|---|---|
| `m_Name` | HoneydewCluster |  |

### TowerList

`unity/Assets/_Game/Features/UI/TowerListPrefab/TowerList.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/UI/TowerListPrefab/TowerList.prefab)) · Prefab

Made by: AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets, looping))

Used in: New Scene, UI

**(scene)** · GameObject

| Field | Value | Points to |
|---|---|---|
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |
| `m_Layer` | 5 |  |

**TowerList** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (637.7309, 60.075012) |  |
| `m_SizeDelta` | (-1884.8304, -214.7722) |  |

**TowerList** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (0.21960786, 0.21960786, 0.21960786, 0) |  |
| `m_Sprite` | (Unity built-in resource) | Built-in |

**TowerList** · TowerShopManager

| Field | Value | Points to |
|---|---|---|
| `availableTowers` | 6 item(s) |  |
| `availableTowers[0]` | Ant | Data asset: unity/Assets/_Game/Features/Towers/Data/Ant.asset |
| `availableTowers[1]` | BulletAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/BulletAnt.asset |
| `availableTowers[2]` | CrazyAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CrazyAnt.asset |
| `availableTowers[3]` | FireAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/FireAnt.asset |
| `availableTowers[4]` | SapAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/SapAnt.asset |
| `availableTowers[5]` | CarpenterAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CarpenterAnt.asset |
| `towerCardPrefab` | TowerCardBase | Prefab: unity/Assets/_Game/Features/UI/TowerCardAssets/TowerCardBase.prefab |
| `shopContainer` | TowerList/Scroll/View/ShopContainer (RectTransform) | Component: unity/Assets/_Game/Features/UI/TowerListPrefab/TowerList.prefab |
| `shopScrollRect` | TowerList/Scroll (ScrollRect) | Component: unity/Assets/_Game/Features/UI/TowerListPrefab/TowerList.prefab |

**TowerList/TowerListBackground** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (8.553162, 5.547516) |  |
| `m_SizeDelta` | (1212.4377, 796.2912) |  |

**TowerList/TowerListBackground** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | TowerListImageGeminiGenerated.png › Gemini_Generated_Image_ir8de0ir8de0ir8d-Photoroom_0 | Sprite: unity/Assets/_Game/Features/UI/TowerListAssets/TowerListImageGeminiGenerated.png |

**TowerList/Scroll/Scrollbar/Sliding Area/Handle** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (0, 0) |  |
| `m_SizeDelta` | (20, 20) |  |

**TowerList/Scroll/Scrollbar/Sliding Area/Handle** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | (Unity built-in resource) | Built-in |

**TowerList/Scroll/View/ShopContainer** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (22.000061, 835.57434) |  |
| `m_SizeDelta` | (356.2078, 0) |  |

**TowerList/Scroll/View/ShopContainer** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 0.003921569) |  |
| `m_Sprite` | (Unity built-in resource) | Built-in |

**TowerList/Scroll/Scrollbar/Sliding Area** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (0, 0) |  |
| `m_SizeDelta` | (-20, -20) |  |

**TowerList/BalanceBox** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (2.9, 433.25) |  |
| `m_SizeDelta` | (354.8965, 113.3276) |  |

**TowerList/BalanceBox** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_Sprite` | BalancePad-Photoroom.png › BalancePad-Photoroom_0 | Sprite: unity/Assets/_Game/Features/UI/TowerListAssets/BalancePad-Photoroom.png |

**TowerList/Scroll** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (346.5537, -114.07501) |  |
| `m_SizeDelta` | (373.4315, 900.2822) |  |

**TowerList/Scroll** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 0) |  |
| `m_Sprite` | (Unity built-in resource) | Built-in |

**TowerList/Scroll** · ScrollRect

| Field | Value | Points to |
|---|---|---|
| `m_OnValueChanged` | 0 listener(s) |  |

**TowerList/Scroll/View** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (-35.621, 0) |  |
| `m_SizeDelta` | (399.2554, 835.5744) |  |

**TowerList/Scroll/View** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (0.7686275, 0.7686275, 0.7686275, 0.003921569) |  |
| `m_Sprite` | (none) |  |

**TowerList/BalanceAmount** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (109.26904, 433.2549) |  |
| `m_SizeDelta` | (200, 50) |  |

**TowerList/BalanceAmount** · TextMeshProUGUI

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 1) |  |
| `m_text` | 150 |  |
| `m_fontSize` | 43 |  |

**TowerList/Scroll/Scrollbar** · RectTransform

| Field | Value | Points to |
|---|---|---|
| `m_AnchoredPosition` | (0, 358) |  |
| `m_SizeDelta` | (20, 418.3885) |  |

**TowerList/Scroll/Scrollbar** · Image

| Field | Value | Points to |
|---|---|---|
| `m_Color` | (1, 1, 1, 0.10980392) |  |
| `m_Sprite` | (Unity built-in resource) | Built-in |

**TowerList/Scroll/Scrollbar** · Scrollbar

| Field | Value | Points to |
|---|---|---|
| `m_Interactable` | 1 |  |
| `m_OnValueChanged` | 0 listener(s) |  |

**TowerList/HoneydewCluster** · GameObject (override)

| Field | Override | Base value |
|---|---|---|
| `m_Name` | HoneydewCluster |  |

**TowerList/HoneydewCluster** · RectTransform (override)

| Field | Override | Base value |
|---|---|---|
| `m_LocalScale.x` | 2 |  |
| `m_LocalScale.y` | 2 |  |
