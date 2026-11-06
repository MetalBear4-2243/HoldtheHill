# Hold the Hill code bank

Every script, prefab, data asset and scene under `unity/Assets/_Game` and `_Sandbox`, built 2026-11-05 from commit `d6944f7b` on `nilly/code-bank-2026-11-05`. Generated: do not edit by hand. Refresh with

```
python B:\Library\Tools\hth-code-bank\build_code_bank.py <repo> --md docs/code-bank --xlsx <out.xlsx>
```

The same data is in the `CodeBase.xlsx` workbook in Drive > Turbulent Towers > Design Documentation, with a Values tab (one row per stored value) and filters.

## How to read it

- **Fields** tables list every `[SerializeField]` and public field: its C# type, the default in code, which prefabs, assets and scenes store a value for it (*Set by*), and the tooltip or summary from the code.
- **API** tables list public properties, methods, events and consts with their `///` summary and which other scripts use them (*Used by*: a text match on the type and member name, so common names can over-count).
- **Prefab** sections show each component's stored values with references resolved to names. A variant shows only what it overrides on its base, with the base value beside it.
- *Made by* names the editor builders (`Tools > Hold the Hill > Alpha`) whose code names that prefab or asset. Rerun the builder instead of hand-editing what it writes.

## Pages

| Page | Scripts | Prefabs | Data assets |
|---|---|---|---|
| [Core](Core.md) | 7 | 0 | 0 |
| [Combat](Combat.md) | 6 | 0 | 0 |
| [Economy](Economy.md) | 4 | 0 | 0 |
| [Enemies](Enemies.md) | 32 | 10 | 5 |
| [GameFlow](GameFlow.md) | 8 | 0 | 0 |
| [Hazards](Hazards.md) | 7 | 4 | 2 |
| [Levels](Levels.md) | 2 | 0 | 0 |
| [PowerUps](PowerUps.md) | 6 | 1 | 1 |
| [Towers](Towers.md) | 21 | 9 | 6 |
| [UI](UI.md) | 18 | 3 | 0 |
| [Editor](Editor.md) | 16 | 0 | 0 |
| [Tests](Tests.md) | 35 | 0 | 0 |
| [Sandbox: 1103-Montgomery-Adam](Sandbox_1103MontgomeryAdam.md) | 0 | 0 | 6 |
| [Scenes](Scenes.md) | 12 scenes |  |  |

## Needs attention

- **Scripts with no summary comment.** TowerCardUI, TowerShopManager
- **Serialized fields with no Tooltip or summary (48).** DamageInfo.Amount, DamageInfo.Source, DamageInfo.HitPoint, DamageInfo.Type, DamageInfo.IsCritical, EnemyHurtAndDeath._hurtFrames, EnemyHurtAndDeath._hurtFrameSe…
- **Scripts in the global namespace (the README asks for HoldTheHill.*).** EnemySpawner, MapWaveDataSO, TowerCardUI, TowerShopManager
- **Serialized reference fields left empty in a prefab, asset or scene (20); fine when the code fills them at runtime.** Enemy_Centipede: CentipedeEnemy._splitPrefab, Tower_Ant: Tower._projectilePrefab, Tower_Ant: Tower._muzzle, Tower_BulletAnt: Tower._muzzle, Tower_CarpenterAnt: …
- **Hand-made prefabs and assets (no builder writes them).** Triangle, HoneydewCluster, TowerCardBase, Map_01, Tower1, Tower2, Tower3, Tower4, Tower5, Tower6
- **Scenes listed in Build Settings but disabled.** Hill01_Gameplay, Hill01_Terrain
- **Components whose saved class identifier is stale (script renamed or moved assembly; harmless, Unity rewrites it on the next save).** Tower1: Assembly-CSharp::TowerData (script is in HoldTheHill.Runtime now); Tower2: Assembly-CSharp::TowerData (script is in HoldTheHill.Runtime now); Tower3: As…
- **Documentation pages that are out of date.** EnemyScripts_Documentation.md (old Assets/Scripts paths), SilverfishEnemy_Documentation.md (describes OnMouseDown, replaced by SilverfishClickReader)
- **MonoBehaviours with no page under Documentation/ (62 of 66).** this code bank covers their fields and API; a prose page per system is still the README's rule

## Scripts at a glance

| Script | Feature | Kind | Owner | What it does |
|---|---|---|---|---|
| [FaceTravel](Core.md#facetravel) | Core | MonoBehaviour | Nilly | Turns a child (the drawn body) to face the way this object moved since the last frame. Reads only the object's own position, after the movement code has run, so… |
| [SpriteFlipbook](Core.md#spriteflipbook) | Core | MonoBehaviour | Nilly | Plays a looping row of sprites on one SpriteRenderer, at a fixed time per frame. No Animator: the frames and their timing come from the sheet (see AlphaSpritesB… |
| [SpriteRow](Core.md#spriterow) | Core | Static class | Nilly | Timing for a row of sprites played once (a hurt flash, a death), as pure functions so EditMode tests cover it. |
| [LevelCatalogue](Core.md#levelcatalogue) | Core | Static class | Nilly | The levels the home screen offers, in order. The Alpha has two. |
| [LevelScenes](Core.md#levelscenes) | Core | Static class | Nilly | The scene-name rule for levels: a level with id L00_TutorialHill is the scene pair L00_TutorialHill_Terrain + L00_TutorialHill_Gameplay. Plain C#, so the rule i… |
| [SceneLoader](Core.md#sceneloader) | Core | MonoBehaviour | Nilly | Lives in the Bootstrap scene. Loads the persistent scenes (UI), then either the home screen or a level's scene pair additively, so pressing Play in ANY scene (B… |
| [SpriteUtil](Core.md#spriteutil) | Core | Static class | Nilly | Shared placeholder art so features run before real sprites exist. |
| [AlphaAlignmentBuilder](Editor.md#alphaalignmentbuilder) | Editor | Static class | Nilly | What items 7-20 of the 10-29 alignment report need in the scenes and settings. In the UI scene: the tower card's order heading wired, so it hides with the order… |
| [AlphaBossesBuilder](Editor.md#alphabossesbuilder) | Editor | Static class | Nilly | Alpha step 6, the bosses. Makes the Centipede prefab (Jason's CentipedeEnemy with a CentipedeSpeedUp beside it) and, through QueenMantisBuilder, the Queen Manti… |
| [AlphaCoreLoopBuilder](Editor.md#alphacoreloopbuilder) | Editor | Static class | Nilly | Sets up everything the Alpha core loop needs and nothing else: placeholder sprites, the first two tower prefabs and their shop data, a basic enemy prefab, a wav… |
| [AlphaEnemiesBuilder](Editor.md#alphaenemiesbuilder) | Editor | Static class | Nilly | Alpha step 5, the enemies. Makes a prefab for each GDD enemy beside the basic one (Rhino Beetle, Wasp, Scarab, Rolly Polly, Mantis Spawn, Silverfish), fills the… |
| [AlphaHazardsBuilder](Editor.md#alphahazardsbuilder) | Editor | Static class | Nilly | Alpha step 7, burrow and the hazards. Makes the four hazard prefabs and the wasp's stinger, gives the Wasp prefab its stinger dropper, writes one hazard schedul… |
| [AlphaHomeAndLevelsBuilder](Editor.md#alphahomeandlevelsbuilder) | Editor | Static class | Nilly | Alpha step 3, home and levels. The Home scene: a title page (Play, Quit) and a level select page with one button per LevelCatalogue entry. The levels L00_Tutori… |
| [AlphaPlayerBuild](Editor.md#alphaplayerbuild) | Editor | Static class | Nilly | Builds a Windows Development player of whatever scenes are enabled in Build Settings (UNITY_RULES rule 48: a player is built and run before each milestone). |
| [AlphaPlaytestToolsBuilder](Editor.md#alphaplaytesttoolsbuilder) | Editor | Static class | Nilly | What the 10-31 play pass asked for in the scenes: DevKeys in the UI scene, with the centipede its F5 key sends, and a LevelCameraFit on each level's camera so t… |
| [AlphaPowerUpsBuilder](Editor.md#alphapowerupsbuilder) | Editor | Static class | Nilly | Alpha step 8, power-ups. Makes the settings asset and the drop prefab, puts a Power Ups object in each level's gameplay scene (wired to the run, the placer and … |
| [AlphaRunFrameBuilder](Editor.md#alpharunframebuilder) | Editor | Static class | Nilly | Alpha step 2, the run frame. In Hill01_Gameplay: the Queen, and a Level Run object that reads the run keys and decides win or lose. In the UI scene: one canvas … |
| [AlphaSellAndHelpBuilder](Editor.md#alphasellandhelpbuilder) | Editor | Static class | Nilly | After the first playtest: a Sell button on the tower card, and a developer help box on the HUD that lists the controls (F1 hides it). Both go into the UI scene. |
| [AlphaShopBindingBuilder](Editor.md#alphashopbindingbuilder) | Editor | Static class | Nilly | Fits Adam's TowerList (his #45) into the run, in the UI scene only: a ShopPlacerBinding as an added component on the prefab instance, so his cards follow Restar… |
| [AlphaSpritesBuilder](Editor.md#alphaspritesbuilder) | Editor | Static class | Nilly | Alpha sprites (2026-11-03): Nilly's top-down pixel sheets on the placeholder enemies and towers, each playing one looping row through a SpriteFlipbook (no Anima… |
| [AlphaTowersBuilder](Editor.md#alphatowersbuilder) | Editor | Static class | Nilly | Alpha step 4, the rest of the towers. Makes the four remaining tower prefabs and their shop data, gives every tower its upgrade levels, puts all six on offer in… |
| [HoldTheHillMenu](Editor.md#holdthehillmenu) | Editor | Static class | Nilly | Adds a Tools &gt; Hold the Hill menu to the Unity editor. |
| [QueenMantisBuilder](Editor.md#queenmantisbuilder) | Editor | Static class | Nilly | The Queen Mantis rebuild (2026-11-02): her art's import settings, a ring sprite, her QueenMantisData, the Enemy_QueenMantis prefab (QueenMantisBoss with the hea… |
| [CombatUtil](Combat.md#combatutil) | Combat | Static class | Jason | Shared queries, splash calculations, and life-check utilities for towers, weapons, and damageables. Uses static non-allocating buffers to avoid garbage collecti… |
| [DamageInfo](Combat.md#damageinfo) | Combat | Struct | Jason | One instance of damage travelling from a source to a target. Struct so hits cost no allocation during heavy waves. |
| [IDamageable](Combat.md#idamageable) | Combat | Interface | Jason | Contract for anything a tower, projectile or hazard can hurt or affect. |
| [IHealthReadable](Combat.md#ihealthreadable) | Combat | Interface | Jason | An IDamageable that exposes its current and maximum health values. Used by towers for Strongest and Weakest priority targeting modes. |
| [IOwned](Combat.md#iowned) | Combat | Interface | Jason | Contract for objects that act on behalf of another entity (e.g., projectiles, hazards left by towers). Damage dealt is credited to Owner rather than the loose o… |
| [StatusEffectData](Combat.md#statuseffectdata) | Combat | Struct | Jason | A damage-over-time and/or slow that a hit or area hazard can leave behind. Serializable so designers can configure it on projectile and weapon prefabs. |
| [Bounty](Economy.md#bounty) | Economy | MonoBehaviour | Nilly | Honeydew paid to the player when this enemy dies. Sits on the enemy prefab next to BasicEnemy and listens to its death; the enemy class itself is untouched. Bos… |
| [Honeydew](Economy.md#honeydew) | Economy | Class | Nilly | The one currency of a run. Plain C#, so the rules can be tested without a scene; HoneydewWallet puts one in a scene. |
| [HoneydewWallet](Economy.md#honeydewwallet) | Economy | MonoBehaviour | Nilly | Holds the run's Honeydew in the gameplay scene. One per level. Towers spend from it, bounties and wave rewards pay into it, the HUD listens to it. |
| [WaveReward](Economy.md#wavereward) | Economy | MonoBehaviour | Nilly | Pays a flat Honeydew reward each time the spawner finishes a wave. Lives next to the HoneydewWallet in the gameplay scene. |
| [EnemyDeathEffect](Enemies.md#enemydeatheffect) | Enemies | MonoBehaviour | Nilly | What is left where an enemy died: a copy of its body playing the Death row once, then fading away. Made at run time by EnemyHurtAndDeath, in the enemy's own sce… |
| [EnemyHurtAndDeath](Enemies.md#enemyhurtanddeath) | Enemies | MonoBehaviour | Nilly | Plays the enemy sheet's Hurt row over the walk each time a hit takes health, and leaves an EnemyDeathEffect playing the Death row where it dies (the enemy itsel… |
| [BasicEnemy](Enemies.md#basicenemy) | Enemies | MonoBehaviour | Nilly + Jason | Baseline component providing core stats, health management, status effect handling, and waypoint traversal along a coordinate path. Implements IDamageable and I… |
| [EnemyPath](Enemies.md#enemypath) | Enemies | MonoBehaviour | Jason | The route enemies walk. Each child object is a waypoint, visited in Hierarchy order. Shows the route in the Scene view and lays down a road when the game starts… |
| [SilverfishClickReader](Enemies.md#silverfishclickreader) | Enemies | MonoBehaviour | Nilly | Kills a SilverfishEnemy with one click, read from the game's actions asset. Jason's #43 took out his OnMouseDown in favour of this reader, so it is the only kil… |
| [EnemySpawner](Enemies.md#enemyspawner) | Enemies | MonoBehaviour | Nilly + Jason | Controls enemy spawning for a 2D Tower Defense game. Handles map-specific wave configurations, sequential enemy spawning, and pauses at the end of each wave unt… |
| [MapWaveDataSO](Enemies.md#mapwavedataso) | Enemies | ScriptableObject | Jason | ScriptableObject asset to configure and store map wave configurations in the Project. Create via: Assets &gt; Create &gt; Tower Defense &gt; Map Wave Config |
| [CentipedeEnemy](Enemies.md#centipedeenemy) | Enemies | MonoBehaviour (enemy) | Nilly + Jason | Specialized subclass of BasicEnemy representing a Centipede enemy. When defeated, it splits into 2 smaller enemies with half of the parent's maximum health and … |
| [CentipedeSpeedUp](Enemies.md#centipedespeedup) | Enemies | MonoBehaviour | Nilly | The Centipede boss's first mechanic, beside Jason's CentipedeEnemy, which does the second (splitting in two on death). Listens to his OnSplit and makes every ch… |
| [IFlyingEnemy](Enemies.md#iflyingenemy) | Enemies | Interface | Nilly | An enemy that may be in the air. Melee and lobbed towers skip an enemy whose IsFlying is true (the wasp rule); straight shots, mines and the slow aura still rea… |
| [RhinoBeetleEnemy](Enemies.md#rhinobeetleenemy) | Enemies | MonoBehaviour (enemy) | Nilly | The Rhino Beetle: a BasicEnemy behind an EnemyShield. Damage hits the shield first; what gets through reaches its health. The shield grows back after a few seco… |
| [RollyPollyEnemy](Enemies.md#rollypollyenemy) | Enemies | MonoBehaviour (enemy) | Nilly | The Rolly Polly: a BasicEnemy that curls up when hit (see RollyPollyCurl). Curled, it takes only a share of each hit and rolls faster for a few tiles of path, t… |
| [SilverfishEnemy](Enemies.md#silverfishenemy) | Enemies | MonoBehaviour (enemy) | Jason | A subclass of BasicEnemy that represents a Silverfish. The Silverfish enemy can be instantly defeated when clicked by the player. |
| [WaspEnemy](Enemies.md#waspenemy) | Enemies | MonoBehaviour (enemy) | Nilly | The Wasp: a BasicEnemy that flies, so melee and lobbed towers cannot reach it (see IFlyingEnemy). Its stingers (hazard 2 in the GDD) come with the hazards step;… |
| [QueenMantisBodyMarkers](Enemies.md#queenmantisbodymarkers) | Enemies | MonoBehaviour (boss part) | Nilly | Shows which bodies she can raise, so the player can keep the fallen away from her: a ring on each remembered body within her raise reach, fading as it gets olde… |
| [QueenMantisBoss](Enemies.md#queenmantisboss) | Enemies | MonoBehaviour (enemy) | Nilly | The Queen Mantis, Meadow's boss: a BasicEnemy that changes shade each time she loses a third of her health, white, black, white (QueenMantisShadeCycle, GDD deci… |
| [QueenMantisData](Enemies.md#queenmantisdata) | Enemies | ScriptableObject | Nilly | The numbers and art behind the Queen Mantis's mechanics: the pray pause, the white heal, the black raise, her summons and her looks. Data, not code (UNITY_RULES… |
| [QueenMantisHealAura](Enemies.md#queenmantishealaura) | Enemies | MonoBehaviour (boss part) | Nilly | The white shade: every HealInterval seconds she heals the hurt enemies within HealRadius (never herself, QueenMantisHealRule) with Jason's Heal, and a QueenMant… |
| [QueenMantisLook](Enemies.md#queenmantislook) | Enemies | MonoBehaviour (boss part) | Nilly | How she looks: the White or Black top-down sprite for her shade, the Shift frames played through each pray pause (forwards into white, backwards into black), an… |
| [QueenMantisPart](Enemies.md#queenmantispart) | Enemies | MonoBehaviour | Nilly | Base for each piece of the Queen Mantis (heal aura, raiser, summoner, look, body markers): a small component beside QueenMantisBoss that reads her QueenMantisDa… |
| [QueenMantisPulse](Enemies.md#queenmantispulse) | Enemies | MonoBehaviour | Nilly | The ring that shows a Queen Mantis heal: it grows from her out to the heal radius and fades. Sits on a child of the Queen Mantis with its own SpriteRenderer (a … |
| [QueenMantisRaiser](Enemies.md#queenmantisraiser) | Enemies | MonoBehaviour (boss part) | Nilly | The black shade. She remembers where enemies fall (BossDeathLog), in either shade. In black, every RaiseInterval seconds she picks up to HusksPerRaise of the fr… |
| [QueenMantisSummoner](Enemies.md#queenmantissummoner) | Enemies | MonoBehaviour (boss part) | Nilly | Her escort: SummonOnEntry Mantis Spawn when she walks in and SummonOnShadeChange more at each shade change, on a ring around her (QueenMantisSummonRule). With E… |
| [EnemyRegistry](Enemies.md#enemyregistry) | Enemies | Static class | Nilly | Every living enemy that carries a RegisteredEnemy, so towers can look for targets without a scene search or physics queries. Enemies made outside the spawner (c… |
| [RegisteredEnemy](Enemies.md#registeredenemy) | Enemies | MonoBehaviour | Nilly | Puts this enemy in the EnemyRegistry while it is alive, so towers can target it, and tells the registry when it is killed. Goes on every enemy prefab next to Ba… |
| [BossDeathLog](Enemies.md#bossdeathlog) | Enemies | Class | Nilly | The Queen Mantis's memory of recent deaths, as plain C# so EditMode tests can cover it. Only the last Capacity deaths are kept. Her own death and the deaths of … |
| [EnemyShield](Enemies.md#enemyshield) | Enemies | Class | Nilly | The Rhino Beetle's shield, as plain C# so EditMode tests can cover it. Damage hits the shield first and only what is left reaches the enemy's health. Once the s… |
| [QueenMantisHealRule](Enemies.md#queenmantishealrule) | Enemies | Static class | Nilly | Who her white heal reaches, as plain C# so EditMode tests can cover it. |
| [QueenMantisShade](Enemies.md#queenmantisshade) | Enemies | Enum | Nilly | The Queen Mantis's two shades (GDD decision 98). |
| [QueenMantisShadeCycle](Enemies.md#queenmantisshadecycle) | Enemies | Class | Nilly | Her shade over one fight, as plain C# so EditMode tests can cover it. It only moves forward: healing never takes her back a third. It moves one third per TryAdv… |
| [QueenMantisSummonRule](Enemies.md#queenmantissummonrule) | Enemies | Static class | Nilly | Where her summoned Mantis Spawn appear, as plain C# so EditMode tests can cover it. |
| [RollyPollyCurl](Enemies.md#rollypollycurl) | Enemies | Class | Nilly | The Rolly Polly's curl, as plain C# so EditMode tests can cover it. A hit on an uncurled Rolly Polly lands in full and makes it curl; while curled it takes only… |
| [DevKeys](GameFlow.md#devkeys) | GameFlow | MonoBehaviour | Nilly | Test keys for the Editor and development builds; a release build turns them off. F5 sends a centipede down the last stretch of road to the Queen, F6 adds Honeyd… |
| [LevelRun](GameFlow.md#levelrun) | GameFlow | MonoBehaviour | Nilly | Where a level's run comes together: the Queen, the spawner and the wallet, the pause and speed RunClock, and the win or lose decision. Lives in the level's game… |
| [NextWaveRule](GameFlow.md#nextwaverule) | GameFlow | Static class | Nilly | When the player may send the next wave, as a pure function so EditMode tests cover it: only while the run is on and not paused, and only when the spawner is wai… |
| [Queen](GameFlow.md#queen) | GameFlow | MonoBehaviour | Nilly + Jason | Represents the Queen object (Ant Queen), which acts as the core health pool for the player / hill. Reduces health by enemy damage when a BasicEnemy reaches its … |
| [RunClock](GameFlow.md#runclock) | GameFlow | Class | Nilly | Pause and speed of a run, as plain C#. TimeScale is what the scene writes to Time.timeScale: zero while paused, otherwise the normal or fast speed. Pausing keep… |
| [RunControls](GameFlow.md#runcontrols) | GameFlow | MonoBehaviour | Nilly | Reads the run keys from the game's actions asset and hands them to the LevelRun beside it: NextWave sends a wave, ToggleSpeed switches 1x / 2x, Pause opens or c… |
| [RunOutcome](GameFlow.md#runoutcome) | GameFlow | Enum | Nilly | How a level run stands: still going, won or lost. |
| [RunOutcomeRule](GameFlow.md#runoutcomerule) | GameFlow | Static class | Nilly | The win and lose rule of a level, as a pure function so EditMode tests cover it. Lost when the Queen is beaten; won when every wave has been sent and no enemy i… |
| [BirdShadow](Hazards.md#birdshadow) | Hazards | MonoBehaviour | Nilly | A bird's shadow drifting across the map. It is placed on the spot it will strike, flies in from one side, and when it passes over that spot it takes whatever is… |
| [DiatomaceousEarth](Hazards.md#diatomaceousearth) | Hazards | MonoBehaviour | Nilly | A patch of diatomaceous earth on the map. It first settles: the dust fades in and does nothing yet, which is the warning (time to burrow, or to see which towers… |
| [HazardScheduler](Hazards.md#hazardscheduler) | Hazards | MonoBehaviour | Nilly | Spawns the level's hazards on their waves, from its LevelHazardSchedule. Lives in the gameplay scene beside the spawner and listens to its wave-started event. |
| [LevelHazardSchedule](Hazards.md#levelhazardschedule) | Hazards | ScriptableObject | Nilly | When the hazards of one level happen: which wave, which hazard, where, how long after the wave starts. One asset per level, read by the level's HazardScheduler.… |
| [PesticideHuman](Hazards.md#pesticidehuman) | Hazards | MonoBehaviour | Nilly | The human with the pesticide. A cloud thickens over the map for a warning spell, then every tower that is not burrowed is stunned for a while. Burrow first and … |
| [WaspStinger](Hazards.md#waspstinger) | Hazards | MonoBehaviour | Nilly | A stinger a wasp dropped. Lies where it fell, then bursts: every tower within reach misses for a while. The sprite swells over the fuse so the player sees it co… |
| [WaspStingerDropper](Hazards.md#waspstingerdropper) | Hazards | MonoBehaviour | Nilly | Makes a wasp drop a WaspStinger every few seconds while it is alive and in the air. Sits on the Wasp prefab beside WaspEnemy; the wasp class is untouched. |
| [LevelButton](Levels.md#levelbutton) | Levels | MonoBehaviour | Nilly | A level select button: clicking it asks the SceneLoader to open its level. |
| [LevelCameraFit](Levels.md#levelcamerafit) | Levels | MonoBehaviour | Nilly | Frames the level's map on any screen shape, with bands left free above and below for the HUD, so the top and bottom rows are not under the Honeydew line, the Qu… |
| [LevelPowerUps](PowerUps.md#levelpowerups) | PowerUps | MonoBehaviour | Nilly | The run's power-ups, in the level's gameplay scene. Rolls a drop when an enemy dies (through EnemyDied, so Jason's enemy class is untouched), lets a click on th… |
| [PowerUpKind](PowerUps.md#powerupkind) | PowerUps | Enum | Nilly | The two power-ups of the Alpha. Nothing else drops. |
| [PowerUpPickup](PowerUps.md#poweruppickup) | PowerUps | MonoBehaviour | Nilly | A power-up lying on the map where an enemy died. Bobs so it is seen, fades away after its lifetime, and says whether a click lands on it. LevelPowerUps makes an… |
| [PowerUpRule](PowerUps.md#poweruprule) | PowerUps | Static class | Nilly | The power-up rules as plain C#, so EditMode tests can cover them: whether a kill drops one and which, and whether a Royal Gift can do anything for a tower. Leve… |
| [PowerUpSettings](PowerUps.md#powerupsettings) | PowerUps | ScriptableObject | Nilly | The numbers behind power-ups: how often they drop, how long a drop waits on the map, and what each one does. One asset per game; data, not code (UNITY_RULES 32)… |
| [PowerUpSlot](PowerUps.md#powerupslot) | PowerUps | Class | Nilly | The one HUD slot: holds at most one collected power-up until it is used. Plain C#, so the rule can be tested; LevelPowerUps owns one. A drop cannot be collected… |
| [ITowerAbility](Towers.md#itowerability) | Towers | Interface | Nilly | A component that does a tower's work in place of its own attack, such as laying mines or slowing enemies. The Tower beside it keeps its numbers (range, damage, … |
| [Mine](Towers.md#mine) | Towers | MonoBehaviour | Nilly | A mine laid by the Carpenter Ant. Waits on the path; the first living enemy to come within its trigger radius sets it off, and everything within the blast radiu… |
| [MineLayer](Towers.md#minelayer) | Towers | MonoBehaviour | Nilly | The Carpenter Ant: keeps a few Mines lying on the stretch of path inside the tower's range, laying a new one on the tower's cooldown whenever one has gone off. … |
| [Projectile](Towers.md#projectile) | Towers | MonoBehaviour | Nilly | One shot from a ranged tower. A straight shot flies at its target and deals the tower's damage on arrival; if the target dies or reaches the Queen first it carr… |
| [RangeRing](Towers.md#rangering) | Towers | MonoBehaviour | Nilly | A thin circle showing how far a tower reaches. The placer shows one around the ghost before a tower is bought, and around the selected tower, so the player sees… |
| [SlowAura](Towers.md#slowaura) | Towers | MonoBehaviour | Nilly | The Sap Ant: slows every enemy inside the tower's range. Uses Jason's status effects on BasicEnemy, re-applied each tick for a little longer than the tick, so a… |
| [TargetPicker](Towers.md#targetpicker) | Towers | Static class | Nilly | The targeting rules, as plain C# so EditMode tests can cover them: how far along the path an enemy is, and which of several candidates a TargetingPriority picks… |
| [TargetingPriority](Towers.md#targetingpriority) | Towers | Enum | Nilly | Which enemy a tower attacks when several are in range. The player sets it per tower from the tower card (the GDD's standing orders). |
| [Tower](Towers.md#tower) | Towers | MonoBehaviour | Nilly | A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, otherwis… |
| [TowerAttackAnimation](Towers.md#towerattackanimation) | Towers | MonoBehaviour | Nilly | Plays the tower's attack row from its sprite sheet over the idle loop. A tower that attacks on its own winds up just before it is ready, so the hit frame shows … |
| [TowerAttackTiming](Towers.md#towerattacktiming) | Towers | Static class | Nilly | The timing of a tower's attack animation, as pure functions so EditMode tests cover it. The art's attack row is wind-up frames, then the hit frame, then follow-… |
| [TowerClickRule](Towers.md#towerclickrule) | Towers | Static class | Nilly | What a left-click on the map does. Plain C# so EditMode tests can cover it; TowerPlacer applies the answer. A standing tower under the cursor is always selected… |
| [TowerCondition](Towers.md#towercondition) | Towers | Class | Nilly | What is being done to a tower, and what that allows: burrowed (the player's order), stunned (a hazard, for a while) or missing (a hazard, for a while). Plain C#… |
| [TowerData](Towers.md#towerdata) | Towers | ScriptableObject | Adam | What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data). |
| [TowerPlacer](Towers.md#towerplacer) | Towers | MonoBehaviour | Nilly | Places and selects towers. A tower is picked from the shop (Select) or with the number keys; a ghost then follows the mouse, snapped to the grid. Left-click bui… |
| [TowerRegistry](Towers.md#towerregistry) | Towers | Static class | Nilly | Every standing Tower, so hazards can find the towers under them without a scene search. The mirror of EnemyRegistry; towers add themselves. |
| [TowerSellRule](Towers.md#towersellrule) | Towers | Static class | Nilly | What the player gets back for selling a tower: a fraction of every Honeydew spent on it (the build price and each upgrade), rounded down. Plain C# so EditMode t… |
| [TowerStunRing](Towers.md#towerstunring) | Towers | MonoBehaviour | Nilly | A ring around a stunned tower that empties like a clock as the stun wears off, so the player can see when the tower fights again. Hidden while the tower is not … |
| [TowerTempo](Towers.md#towertempo) | Towers | Static class | Nilly | How fast every tower fires right now, as one factor: 1 is normal, 2 is twice as fast. Tower divides its cooldown by it in one place; the Swarm Frenzy power-up s… |
| [TowerUpgrade](Towers.md#towerupgrade) | Towers | Struct | Nilly | One upgrade step of a tower: what it costs and the numbers the tower has afterwards. A tower prefab lists two of these for levels 2 and 3. Data, not code (UNITY… |
| [TowerUpgradeRule](Towers.md#towerupgraderule) | Towers | Static class | Nilly | Whether a tower may be upgraded right now. Plain C# so EditMode tests can cover it; UpgradeSelectedTower applies the answer. |
| [HomeScreen](UI.md#homescreen) | UI | MonoBehaviour | Nilly | The home screen in the Home scene: a title page with Play and Quit, and the level select page Play opens. The level buttons on that page carry their own LevelBu… |
| [ControlsOverlay](UI.md#controlsoverlay) | UI | MonoBehaviour | Nilly | A developer help box on the HUD listing the keys and clicks, from UiStrings. Opens with the tutorial level only (ShowsControlsHelp), since on later maps it cove… |
| [HoneydewLabel](UI.md#honeydewlabel) | UI | MonoBehaviour (HUD listener) | Nilly | Shows the run's Honeydew. Updates only when the amount changes. Also sits on the balance box of Adam's shop list, which shows the number alone. |
| [HudPanel](UI.md#hudpanel) | UI | MonoBehaviour (HUD listener) | Nilly | Shows the HUD only while a level runs, so the home screen is not drawn over by a stale Honeydew count and wave label. Sits on the canvas, above the HUD object i… |
| [LevelRunListener](UI.md#levelrunlistener) | UI | MonoBehaviour | Nilly | Base for HUD and menu pieces in the persistent UI scene. They cannot hold a serialized reference into a level scene, so this follows Started and Ended and hands… |
| [NextWaveButton](UI.md#nextwavebutton) | UI | MonoBehaviour (HUD listener) | Nilly | The Next Wave button. Sends the current run's next wave, the same as the Space key, and is greyed out whenever that would do nothing: while a wave is still comi… |
| [PauseMenu](UI.md#pausemenu) | UI | MonoBehaviour (HUD listener) | Nilly | The pause panel: shown while the run's clock is paused and the run is still going. Its buttons work at time scale zero because uGUI buttons ignore Time.timeScal… |
| [PowerUpSlotPanel](UI.md#powerupslotpanel) | UI | MonoBehaviour (HUD listener) | Nilly | The HUD's one power-up slot: a button with the held power-up's icon and name. Clicking it uses the power-up through the run. While a Royal Gift waits for a towe… |
| [QueenHealthBar](UI.md#queenhealthbar) | UI | MonoBehaviour (HUD listener) | Nilly | A bar that follows the Queen's health through her health-changed event. |
| [QueenMantisBossBar](UI.md#queenmantisbossbar) | UI | MonoBehaviour | Nilly | The Queen Mantis's health bar, shown only while she walks. Marks at the thirds show where she changes shade; the fill takes her shade's colour and the label nam… |
| [ResultPanel](UI.md#resultpanel) | UI | MonoBehaviour (HUD listener) | Nilly | The one result panel for both outcomes: a title that says won or lost, Restart and Home. |
| [ShopPlacerBinding](UI.md#shopplacerbinding) | UI | MonoBehaviour (HUD listener) | Nilly | Keeps Adam's shop cards pointed at the running level's TowerPlacer. A card finds the placer once, in its Start, so after Restart or the next level it would stil… |
| [TimedNotice](UI.md#timednotice) | UI | Class | Nilly | A short message that stands in for a HUD label for a few seconds, such as "Already max level". Plain C# so EditMode tests can cover it. The caller passes the ti… |
| [TowerCardPanel](UI.md#towercardpanel) | UI | MonoBehaviour (HUD listener) | Nilly | The card for the selected standing tower: its name and level, an upgrade button with the price, the burrow order, and the standing order (First / Last / Closest… |
| [UiStrings](UI.md#uistrings) | UI | Static class | Nilly | Every string the HUD and menus show, in one place, so they can be checked and later localized. |
| [WaveLabel](UI.md#wavelabel) | UI | MonoBehaviour (HUD listener) | Nilly | Shows "Wave 3 / 10". Reads 0 before the first wave is sent. |
| [TowerCardUI](UI.md#towercardui) | UI | MonoBehaviour | Adam |  |
| [TowerShopManager](UI.md#towershopmanager) | UI | MonoBehaviour | Adam |  |
| [BasicEnemyTests](Tests.md#basicenemytests) | Tests | Test fixture | Nilly + Jason |  |
| [BossDeathLogTests](Tests.md#bossdeathlogtests) | Tests | Test fixture | Nilly |  |
| [CentipedeEnemyTests](Tests.md#centipedeenemytests) | Tests | Test fixture | Nilly + Jason |  |
| [DiatomaceousEarthTests](Tests.md#diatomaceousearthtests) | Tests | Test fixture | Nilly |  |
| [EnemyHurtAndDeathDataTests](Tests.md#enemyhurtanddeathdatatests) | Tests | Test fixture | Nilly | Checks the enemy prefabs, not code: each one plays its own sheet's Hurt and Death rows on its Body, holding its walk (Build Sprites sets this). The pill bug's s… |
| [EnemyShieldTests](Tests.md#enemyshieldtests) | Tests | Test fixture | Nilly |  |
| [EnemySpawnerTests](Tests.md#enemyspawnertests) | Tests | Test fixture | Jason |  |
| [HoneydewTests](Tests.md#honeydewtests) | Tests | Test fixture | Nilly |  |
| [LevelCameraFitTests](Tests.md#levelcamerafittests) | Tests | Test fixture | Nilly |  |
| [LevelCatalogueTests](Tests.md#levelcataloguetests) | Tests | Test fixture | Nilly |  |
| [LevelScenesTests](Tests.md#levelscenestests) | Tests | Test fixture | Nilly |  |
| [NextWaveRuleTests](Tests.md#nextwaveruletests) | Tests | Test fixture | Nilly |  |
| [PowerUpRuleTests](Tests.md#powerupruletests) | Tests | Test fixture | Nilly |  |
| [PowerUpSlotTests](Tests.md#powerupslottests) | Tests | Test fixture | Nilly |  |
| [QueenMantisHealRuleTests](Tests.md#queenmantishealruletests) | Tests | Test fixture | Nilly |  |
| [QueenMantisShadeTests](Tests.md#queenmantisshadetests) | Tests | Test fixture | Nilly |  |
| [QueenMantisSummonRuleTests](Tests.md#queenmantissummonruletests) | Tests | Test fixture | Nilly |  |
| [QueenTests](Tests.md#queentests) | Tests | Test fixture | Nilly + Jason |  |
| [RangeRingTests](Tests.md#rangeringtests) | Tests | Test fixture | Nilly |  |
| [RollyPollyCurlTests](Tests.md#rollypollycurltests) | Tests | Test fixture | Nilly |  |
| [RunClockTests](Tests.md#runclocktests) | Tests | Test fixture | Nilly |  |
| [RunOutcomeRuleTests](Tests.md#runoutcomeruletests) | Tests | Test fixture | Nilly |  |
| [SpriteRowTests](Tests.md#spriterowtests) | Tests | Test fixture | Nilly |  |
| [TargetPickerTests](Tests.md#targetpickertests) | Tests | Test fixture | Nilly |  |
| [TimedNoticeTests](Tests.md#timednoticetests) | Tests | Test fixture | Nilly |  |
| [TowerAttackDataTests](Tests.md#towerattackdatatests) | Tests | Class | Nilly | Checks the tower prefabs, not code: each one carries its sheet's attack row, wired to its own renderer and idle loop, with the blow on frame 2 (Build Sprites se… |
| [TowerAttackTimingTests](Tests.md#towerattacktimingtests) | Tests | Test fixture | Nilly |  |
| [TowerClickRuleTests](Tests.md#towerclickruletests) | Tests | Test fixture | Nilly |  |
| [TowerConditionTests](Tests.md#towerconditiontests) | Tests | Test fixture | Nilly |  |
| [TowerIconDataTests](Tests.md#towericondatatests) | Tests | Class | Nilly | Checks the tower data, not code: each tower's shop card (Adam's TowerList) shows the same picture as the tower standing on the map, not the placeholder square. |
| [TowerListDataTests](Tests.md#towerlistdatatests) | Tests | Test fixture | Nilly | Checks Adam's TowerList prefab, not code: its shop cards are the game's six towers, in the same order as the number keys, and none of the sandbox test towers th… |
| [TowerRangeDataTests](Tests.md#towerrangedatatests) | Tests | Test fixture | Nilly | Checks the tower prefabs' numbers, not code: a melee tower placed on the outer diagonal of a road corner must still reach the road. The road's centre line runs … |
| [TowerSellRuleTests](Tests.md#towersellruletests) | Tests | Test fixture | Nilly |  |
| [TowerTempoTests](Tests.md#towertempotests) | Tests | Test fixture | Nilly |  |
| [TowerUpgradeRuleTests](Tests.md#towerupgraderuletests) | Tests | Test fixture | Nilly |  |

## Prefabs at a glance

| Prefab | Feature | Scripts | Variant of | Made by |
|---|---|---|---|---|
| [Enemy_Basic](Enemies.md#enemy_basic) | Enemies | SpriteFlipbook, BasicEnemy, RegisteredEnemy, Bounty, FaceTravel, EnemyHurtAndDeath |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silve… |
| [Enemy_Centipede](Enemies.md#enemy_centipede) | Enemies | CentipedeEnemy, RegisteredEnemy, Bounty, CentipedeSpeedUp, FaceTravel, EnemyHurtAndDeath, SpriteFlipbook |  | AlphaBossesBuilder (Tools/Hold the Hill/Alpha/Build Bosses (Centipede, Queen Mantis, last waves)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites … |
| [Enemy_MantisSpawn](Enemies.md#enemy_mantisspawn) | Enemies |  | Enemy_Basic | AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites… |
| [Enemy_QueenMantis](Enemies.md#enemy_queenmantis) | Enemies | QueenMantisPulse, QueenMantisBoss, RegisteredEnemy, Bounty, QueenMantisHealAura, QueenMantisRaiser, QueenMantisBodyMarkers, QueenMantisSummoner, QueenMantisLook |  | AlphaBossesBuilder (Tools/Hold the Hill/Alpha/Build Bosses (Centipede, Queen Mantis, last waves)); QueenMantisBuilder (Tools/Hold the Hill/Alpha/Build Queen Man… |
| [Enemy_RhinoBeetle](Enemies.md#enemy_rhinobeetle) | Enemies | RhinoBeetleEnemy, RegisteredEnemy, Bounty, FaceTravel, EnemyHurtAndDeath, SpriteFlipbook |  | AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites… |
| [Enemy_RollyPolly](Enemies.md#enemy_rollypolly) | Enemies | SpriteFlipbook, RollyPollyEnemy, RegisteredEnemy, Bounty, FaceTravel |  | AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites… |
| [Enemy_Scarab](Enemies.md#enemy_scarab) | Enemies |  | Enemy_Basic | AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites… |
| [Enemy_Silverfish](Enemies.md#enemy_silverfish) | Enemies | SpriteFlipbook, SilverfishEnemy, RegisteredEnemy, Bounty, FaceTravel, EnemyHurtAndDeath |  | AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites… |
| [Enemy_Wasp](Enemies.md#enemy_wasp) | Enemies | SpriteFlipbook, WaspEnemy, RegisteredEnemy, Bounty, WaspStingerDropper, FaceTravel, EnemyHurtAndDeath |  | AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards… |
| [Triangle](Enemies.md#triangle) | Enemies | BasicEnemy |  | Hand-made (no builder names it) |
| [Hazard_BirdShadow](Hazards.md#hazard_birdshadow) | Hazards | BirdShadow |  | AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules)) |
| [Hazard_DiatomaceousEarth](Hazards.md#hazard_diatomaceousearth) | Hazards | DiatomaceousEarth |  | AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules)) |
| [Hazard_PesticideHuman](Hazards.md#hazard_pesticidehuman) | Hazards | PesticideHuman |  | AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules)) |
| [Hazard_WaspStinger](Hazards.md#hazard_waspstinger) | Hazards | WaspStinger |  | AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules)) |
| [PowerUp_Pickup](PowerUps.md#powerup_pickup) | PowerUps | PowerUpPickup |  | AlphaPowerUpsBuilder (Tools/Hold the Hill/Alpha/Build Power-ups (drops, slot, Swarm Frenzy, Royal Gift)) |
| [Mine_Carpenter](Towers.md#mine_carpenter) | Towers | Mine |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [Projectile_Bullet](Towers.md#projectile_bullet) | Towers | Projectile |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [Projectile_Lob](Towers.md#projectile_lob) | Towers | Projectile |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [Tower_Ant](Towers.md#tower_ant) | Towers | Tower, SpriteFlipbook, TowerAttackAnimation |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,… |
| [Tower_BulletAnt](Towers.md#tower_bulletant) | Towers | Tower, SpriteFlipbook, TowerAttackAnimation |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,… |
| [Tower_CarpenterAnt](Towers.md#tower_carpenterant) | Towers | Tower, MineLayer, SpriteFlipbook, TowerAttackAnimation |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,… |
| [Tower_CrazyAnt](Towers.md#tower_crazyant) | Towers | Tower, SpriteFlipbook, TowerAttackAnimation |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,… |
| [Tower_FireAnt](Towers.md#tower_fireant) | Towers | Tower, SpriteFlipbook, TowerAttackAnimation |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,… |
| [Tower_SapAnt](Towers.md#tower_sapant) | Towers | Tower, SlowAura, SpriteFlipbook, TowerAttackAnimation |  | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,… |
| [HoneydewCluster](UI.md#honeydewcluster) | UI |  |  | Hand-made (no builder names it) |
| [TowerCardBase](UI.md#towercardbase) | UI | TowerCardUI |  | Hand-made (no builder names it) |
| [TowerList](UI.md#towerlist) | UI | TowerShopManager |  | AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets, looping)) |

## Data assets at a glance

| Asset | Type | Feature | Made by |
|---|---|---|---|
| [Hill01_Waves](Enemies.md#hill01_waves) | MapWaveDataSO | Enemies | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [L00_TutorialHill_Waves](Enemies.md#l00_tutorialhill_waves) | MapWaveDataSO | Enemies | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [L01_Meadow_Waves](Enemies.md#l01_meadow_waves) | MapWaveDataSO | Enemies | QueenMantisBuilder (Tools/Hold the Hill/Alpha/Build Queen Mantis (art, data, prefab, wave, boss bar)) |
| [QueenMantisData](Enemies.md#queenmantisdata) | QueenMantisData | Enemies | QueenMantisBuilder (Tools/Hold the Hill/Alpha/Build Queen Mantis (art, data, prefab, wave, boss bar)) |
| [Map_01](Enemies.md#map_01) | MapWaveDataSO | Enemies | Hand-made (no builder names it) |
| [L00_TutorialHill_Hazards](Hazards.md#l00_tutorialhill_hazards) | LevelHazardSchedule | Hazards | AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules)) |
| [L01_Meadow_Hazards](Hazards.md#l01_meadow_hazards) | LevelHazardSchedule | Hazards | AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules)) |
| [PowerUpSettings](PowerUps.md#powerupsettings) | PowerUpSettings | PowerUps | AlphaPowerUpsBuilder (Tools/Hold the Hill/Alpha/Build Power-ups (drops, slot, Swarm Frenzy, Royal Gift)) |
| [Ant](Towers.md#ant) | TowerData | Towers | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [BulletAnt](Towers.md#bulletant) | TowerData | Towers | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [CarpenterAnt](Towers.md#carpenterant) | TowerData | Towers | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [CrazyAnt](Towers.md#crazyant) | TowerData | Towers | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [FireAnt](Towers.md#fireant) | TowerData | Towers | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [SapAnt](Towers.md#sapant) | TowerData | Towers | AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)) |
| [Tower1](Sandbox_1103MontgomeryAdam.md#tower1) | TowerData | Sandbox: 1103-Montgomery-Adam | Hand-made (no builder names it) |
| [Tower2](Sandbox_1103MontgomeryAdam.md#tower2) | TowerData | Sandbox: 1103-Montgomery-Adam | Hand-made (no builder names it) |
| [Tower3](Sandbox_1103MontgomeryAdam.md#tower3) | TowerData | Sandbox: 1103-Montgomery-Adam | Hand-made (no builder names it) |
| [Tower4](Sandbox_1103MontgomeryAdam.md#tower4) | TowerData | Sandbox: 1103-Montgomery-Adam | Hand-made (no builder names it) |
| [Tower5](Sandbox_1103MontgomeryAdam.md#tower5) | TowerData | Sandbox: 1103-Montgomery-Adam | Hand-made (no builder names it) |
| [Tower6](Sandbox_1103MontgomeryAdam.md#tower6) | TowerData | Sandbox: 1103-Montgomery-Adam | Hand-made (no builder names it) |

## Assemblies

| Assembly | References | Scripts |
|---|---|---|
| HoldTheHill.Editor | HoldTheHill.Runtime, Unity.InputSystem, UnityEngine.UI, Unity.TextMeshPro, Unity.2D.Sprite.Editor | 16 |
| HoldTheHill.Runtime | Unity.InputSystem, Unity.RenderPipelines.Core.Runtime, Unity.RenderPipelines.Universal.Runtime, Unity.RenderPipelines.Universal.2D.Runtime, Unity.2D.Animation.R… | 111 |
| HoldTheHill.Tests.EditMode | HoldTheHill.Runtime, UnityEngine.TestRunner, UnityEditor.TestRunner | 35 |
| HoldTheHill.Tests.PlayMode | HoldTheHill.Runtime, UnityEngine.TestRunner, UnityEditor.TestRunner | 0 |

## Editor menu commands

| Menu | Script | What it does |
|---|---|---|
| Tools/Hold the Hill/Alpha/Add Dev Keys and Camera Fit | AlphaPlaytestToolsBuilder | What the 10-31 play pass asked for in the scenes: DevKeys in the UI scene, with the centipede its F5 key sends, and a LevelCameraFit on each level's camera so t… |
| Tools/Hold the Hill/Alpha/Apply Alignment Fixes (10-29 items 7-20) | AlphaAlignmentBuilder | What items 7-20 of the 10-29 alignment report need in the scenes and settings. In the UI scene: the tower card's order heading wired, so it hides with the order… |
| Tools/Hold the Hill/Alpha/Bind Shop List to the Level | AlphaShopBindingBuilder | Fits Adam's TowerList (his #45) into the run, in the UI scene only: a ShopPlacerBinding as an added component on the prefab instance, so his cards follow Restar… |
| Tools/Hold the Hill/Alpha/Build Bosses (Centipede, Queen Mantis, last waves) | AlphaBossesBuilder | Alpha step 6, the bosses. Makes the Centipede prefab (Jason's CentipedeEnemy with a CentipedeSpeedUp beside it) and, through QueenMantisBuilder, the Queen Manti… |
| Tools/Hold the Hill/Alpha/Build Core Loop (Hill01) | AlphaCoreLoopBuilder | Sets up everything the Alpha core loop needs and nothing else: placeholder sprites, the first two tower prefabs and their shop data, a basic enemy prefab, a wav… |
| Tools/Hold the Hill/Alpha/Build Development Player (Windows) | AlphaPlayerBuild | Builds a Windows Development player of whatever scenes are enabled in Build Settings (UNITY_RULES rule 48: a player is built and run before each milestone). |
| Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click) | AlphaEnemiesBuilder | Alpha step 5, the enemies. Makes a prefab for each GDD enemy beside the basic one (Rhino Beetle, Wasp, Scarab, Rolly Polly, Mantis Spawn, Silverfish), fills the… |
| Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules) | AlphaHazardsBuilder | Alpha step 7, burrow and the hazards. Makes the four hazard prefabs and the wasp's stinger, gives the Wasp prefab its stinger dropper, writes one hazard schedul… |
| Tools/Hold the Hill/Alpha/Build Home Screen | AlphaHomeAndLevelsBuilder | Alpha step 3, home and levels. The Home scene: a title page (Play, Quit) and a level select page with one button per LevelCatalogue entry. The levels L00_Tutori… |
| Tools/Hold the Hill/Alpha/Build Home and Levels (all) | AlphaHomeAndLevelsBuilder | Alpha step 3, home and levels. The Home scene: a title page (Play, Quit) and a level select page with one button per LevelCatalogue entry. The levels L00_Tutori… |
| Tools/Hold the Hill/Alpha/Build Level L00_TutorialHill | AlphaHomeAndLevelsBuilder | Alpha step 3, home and levels. The Home scene: a title page (Play, Quit) and a level select page with one button per LevelCatalogue entry. The levels L00_Tutori… |
| Tools/Hold the Hill/Alpha/Build Level L01_Meadow | AlphaHomeAndLevelsBuilder | Alpha step 3, home and levels. The Home scene: a title page (Play, Quit) and a level select page with one button per LevelCatalogue entry. The levels L00_Tutori… |
| Tools/Hold the Hill/Alpha/Build Power-ups (drops, slot, Swarm Frenzy, Royal Gift) | AlphaPowerUpsBuilder | Alpha step 8, power-ups. Makes the settings asset and the drop prefab, puts a Power Ups object in each level's gameplay scene (wired to the run, the placer and … |
| Tools/Hold the Hill/Alpha/Build Queen Mantis (art, data, prefab, wave, boss bar) | QueenMantisBuilder | The Queen Mantis rebuild (2026-11-02): her art's import settings, a ring sprite, her QueenMantisData, the Enemy_QueenMantis prefab (QueenMantisBoss with the hea… |
| Tools/Hold the Hill/Alpha/Build Run Frame (HUD, keys, win and lose) | AlphaRunFrameBuilder | Alpha step 2, the run frame. In Hill01_Gameplay: the Queen, and a Level Run object that reads the run keys and decides win or lose. In the UI scene: one canvas … |
| Tools/Hold the Hill/Alpha/Build Sell Button and Controls Help | AlphaSellAndHelpBuilder | After the first playtest: a Sell button on the tower card, and a developer help box on the HUD that lists the controls (F1 hides it). Both go into the UI scene. |
| Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets, looping) | AlphaSpritesBuilder | Alpha sprites (2026-11-03): Nilly's top-down pixel sheets on the placeholder enemies and towers, each playing one looping row through a SpriteFlipbook (no Anima… |
| Tools/Hold the Hill/Alpha/Build Towers (all six, upgrades, card) | AlphaTowersBuilder | Alpha step 4, the rest of the towers. Makes the four remaining tower prefabs and their shop data, gives every tower its upgrade levels, puts all six on offer in… |
| Tools/Hold the Hill/Create Example Path | HoldTheHillMenu | Adds a Tools &gt; Hold the Hill menu to the Unity editor. |

## Tests

| Fixture | Tests | Test methods |
|---|---|---|
| BasicEnemyTests | 14 | Health_IsDouble_CanTakeDoubleDamage, Damage_IsInt_CanGetAndSet, PathCoordinates_CanBeAssignedAndAccessed, PathCoordinates_Vector2Overload_AssignsCorrectly, Take… |
| BossDeathLogTests | 11 | AnOrdinaryDeath_Counts, TheQueensOwnDeath_DoesNotCount, AHuskSheRaised_DoesNotCount_SoItNeverComesBackTwice, OnlyTheNewestDeaths_AreKept_UpToTheCapacity, TakeNe… |
| CentipedeEnemyTests | 22 | SplitsRemaining_DefaultsToMinusOne, CanSplit_WithHealthGreaterThanOne_AndMinusOneSplits_ReturnsTrue, CanSplit_WhenHealthIsOneOrLess_ReturnsFalse, CanSplit_WhenH… |
| DiatomaceousEarthTests | 3 | SettleShare_FadesInFromAHaze, SettleShare_IsSolidOnceSettled, SettleShare_NoSettleTime_IsSolidAtOnce |
| EnemyHurtAndDeathDataTests | 2 | EachEnemy_FlinchesAndDiesWithItsOwnArt, TheRollyPolly_HasNoRowsToPlay_AndIsLeftAsItWas |
| EnemyShieldTests | 7 | StartsFull_AndSoaksAHitSmallerThanItself, OnlyTheExcessGetsThrough_WhenAHitEmptiesTheShield, ZeroOrNegativeDamage_ChangesNothing, DoesNotRegrow_BeforeTheDelayHa… |
| EnemySpawnerTests | 10 | LivingEnemyCount_InitializesToZero, SpawnEnemy_IncrementsLivingEnemyCount, EnemyDeath_ViaStaticEvent_DecrementsLivingEnemyCount, EnemyDestinationReached_ViaStat… |
| HoneydewTests | 4 | StartsAtTheStartingAmount_NeverBelowZero, Earn_Adds_AndIgnoresZeroOrLess, Spending_FailsAndChangesNothing_WhenShort, Changed_IsRaisedWithTheNewAmount_OnlyWhenIt… |
| LevelCameraFitTests | 5 | At16By9_TheHeightWithItsHudBands_SetsTheSize, At4By3_TheWidthSetsTheSize, WithTheListBand_TheWholeMapSitsLeftOfIt_AtEveryAspect, WithNoRightBand_TheCameraStaysC… |
| LevelCatalogueTests | 5 | HasTheTwoAlphaLevels_InOrder, EveryLevel_HasAUniqueIdAndATitle, EveryLevelId_RoundTripsThroughTheSceneNameRule, TryFind_FindsListedIds_AndRejectsOthers, TheCont… |
| LevelScenesTests | 9 | SceneNames_AreTheIdPlusASuffix, TryGetLevelId_ReadsTheIdBackFromEitherScene, TryGetLevelId_StillFitsHill01, TryGetLevelId_RejectsEveryOtherScene, TryGetLevelId_… |
| NextWaveRuleTests | 4 | CanSend_BeforeTheFirstWave_AndBetweenWaves, CannotSend_WhileAWaveIsComingOut_OrAfterTheLast, CannotSend_WhilePaused, CannotSend_OnceTheRunIsOver |
| PowerUpRuleTests | 5 | RollDrop_DropsOnlyBelowTheChance, RollDrop_SplitsTheDroppingRange_EvenlyBetweenTheKinds, RollDrop_NeverDrops_WithNoChance_OrABadRoll, RollDrop_SeededRandom_Drop… |
| PowerUpSlotTests | 3 | StartsEmpty_AndStoresOne, AFullSlot_RefusesASecondDrop_AndKeepsTheFirst, Take_EmptiesTheSlot_AndHandsBackWhatItHeld |
| QueenMantisHealRuleTests | 6 | AHurtEnemyWithinReach_IsHealed, SheNeverHealsHerself, TheDead_AreNotHealed, AnEnemyAtFullHealth_IsLeftAlone, AnEnemyOutOfReach_IsNotHealed, TheEdgeOfHerReach_Co… |
| QueenMantisShadeTests | 9 | ThirdsLost_FollowsTheHealthLeft, ExactThirds_CountAsLost, OutOfRangeFractions_ReadAsFullOrEmpty, TheShades_GoWhiteBlackWhite_AsInDecision108, TheCycle_StartsWhi… |
| QueenMantisSummonRuleTests | 4 | TheFirstEscort_StandsStraightAboveHer, EveryEscort_IsOnTheRing, Escorts_AreSpreadEvenly_NoTwoInOnePlace, ACountBelowOne_StillGivesAPlace |
| QueenTests | 7 | Health_IsDouble_AndInitializedToMaxHealth, TakeDamage_ReducesCurrentHealth_AndFiresHealthChanged, HandleEnemyReachedDestination_SubtractsEnemyDamageFromQueenHea… |
| RangeRingTests | 2 | CirclePoints_AllLieOnTheRange_AroundTheTower, CirclePoints_GoAllTheWayRound |
| RollyPollyCurlTests | 6 | TheFirstHit_LandsInFull_AndCurlsIt, HitsWhileCurled_LandAtTheReducedShare_AndDoNotUseACurl, UncurlsAfterRollingTheDistance_AndSaysSoOnce, CurlsAtMostThreeTimes_… |
| RunClockTests | 5 | StartsRunning_AtNormalSpeed, ToggleSpeed_SwitchesBetweenNormalAndFast, Pausing_StopsTime_AndResumingKeepsTheChosenSpeed, Changed_IsRaised_OnlyWhenSomethingChang… |
| RunOutcomeRuleTests | 4 | Playing_WhileWavesRemain, Playing_WhileTheLastWaveStillHasEnemies, Won_WhenEveryWaveIsSentAndNoEnemyIsLeft, Lost_WhenTheQueenFalls_EvenWithTheMapCleared |
| SpriteRowTests | 3 | FrameAt_PlaysTheRowOnce, Length_IsEveryFrameOnce, ADeath_StaysSolidThroughTheRow_ThenFades |
| TargetPickerTests | 7 | First_PicksTheEnemyFurthestAlongThePath, Last_PicksTheEnemyLeastFarAlongThePath, Closest_PicksTheEnemyNearestTheTower, NoCandidates_PicksNothing_AndTiesGoToTheE… |
| TimedNoticeTests | 4 | ANotice_ShowsForItsTime_ThenRunsOut, Clear_DropsTheNotice_SoTheNextRunStartsWithout, ANewNotice_ReplacesTheOneUp_AndRestartsTheTime, NothingShown_GivesNoText |
| TowerAttackDataTests | 1 | EachTower_PlaysItsOwnAttackRow |
| TowerAttackTimingTests | 7 | FrameSeconds_UsesTheSheetsTime_WhenTheTowerIsSlowerThanTheRow, FrameSeconds_SqueezesTheRow_IntoTheTimeBetweenAttacks, FrameSeconds_NeverDropsBelowTheFloor, Wind… |
| TowerClickRuleTests | 4 | ClickOnStandingTower_SelectsIt_WithOrWithoutATowerInHand, ClickOnEmptyCell_WithNothingInHand_ClearsTheSelection, ClickWithTowerInHand_BuildsOnlyOnAFreeAffordabl… |
| TowerConditionTests | 7 | AFreshTower_ActsAndItsHitsLand, Burrowed_DoesNotAct_ButIsNotStunned, Stunned_DoesNotAct_UntilTheTimeHasPassed, Missing_StillActs_ButItsHitsDoNothing, AShorterSp… |
| TowerIconDataTests | 1 | TheShopIcon_IsTheTowersOwnArt |
| TowerListDataTests | 1 | TheShopListsTheSixTowers_InNumberKeyOrder |
| TowerRangeDataTests | 1 | MeleeTowers_ReachTheRoadFromACornersOuterDiagonal_AtEveryLevel |
| TowerSellRuleTests | 2 | Refund_IsTheFractionOfEverythingSpent_RoundedDown, Refund_NeverGoesNegative_OrAboveWhatWasSpent |
| TowerTempoTests | 5 | NextCooldown_KeepsThePartOfTheFrameTheShotWasLateBy, NextCooldown_GivesNoCreditForSittingIdle, NextCooldown_ExactlyOnTime_IsTheFullInterval, NextCooldown_Follow… |
| TowerUpgradeRuleTests | 4 | Ready_WhenBelowMaxLevelAndTheNextLevelIsAffordable, CannotAfford_WhenShortByEvenOne, AtMaxLevel_BeatsThePriceCheck, AtMaxLevel_ForATowerWithNoUpgradesAtAll |
