# Tests

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [BasicEnemyTests](#basicenemytests) | Test fixture | Nilly + Jason |  |
| [BossDeathLogTests](#bossdeathlogtests) | Test fixture | Nilly |  |
| [CentipedeEnemyTests](#centipedeenemytests) | Test fixture | Nilly + Jason |  |
| [DiatomaceousEarthTests](#diatomaceousearthtests) | Test fixture | Nilly |  |
| [EnemyHurtAndDeathDataTests](#enemyhurtanddeathdatatests) | Test fixture | Nilly | Checks the enemy prefabs, not code: each one plays its own sheet's Hurt and Death rows on its Body, holding its walk (Build Sprites sets this). The pill bug's s… |
| [EnemyShieldTests](#enemyshieldtests) | Test fixture | Nilly |  |
| [EnemySpawnerTests](#enemyspawnertests) | Test fixture | Jason |  |
| [HoneydewTests](#honeydewtests) | Test fixture | Nilly |  |
| [LevelCameraFitTests](#levelcamerafittests) | Test fixture | Nilly |  |
| [LevelCatalogueTests](#levelcataloguetests) | Test fixture | Nilly |  |
| [LevelScenesTests](#levelscenestests) | Test fixture | Nilly |  |
| [NextWaveRuleTests](#nextwaveruletests) | Test fixture | Nilly |  |
| [PowerUpRuleTests](#powerupruletests) | Test fixture | Nilly |  |
| [PowerUpSlotTests](#powerupslottests) | Test fixture | Nilly |  |
| [QueenMantisHealRuleTests](#queenmantishealruletests) | Test fixture | Nilly |  |
| [QueenMantisShadeTests](#queenmantisshadetests) | Test fixture | Nilly |  |
| [QueenMantisSummonRuleTests](#queenmantissummonruletests) | Test fixture | Nilly |  |
| [QueenTests](#queentests) | Test fixture | Nilly + Jason |  |
| [RangeRingTests](#rangeringtests) | Test fixture | Nilly |  |
| [RollyPollyCurlTests](#rollypollycurltests) | Test fixture | Nilly |  |
| [RunClockTests](#runclocktests) | Test fixture | Nilly |  |
| [RunOutcomeRuleTests](#runoutcomeruletests) | Test fixture | Nilly |  |
| [SpriteRowTests](#spriterowtests) | Test fixture | Nilly |  |
| [TargetPickerTests](#targetpickertests) | Test fixture | Nilly |  |
| [TimedNoticeTests](#timednoticetests) | Test fixture | Nilly |  |
| [TowerAttackDataTests](#towerattackdatatests) | Class | Nilly | Checks the tower prefabs, not code: each one carries its sheet's attack row, wired to its own renderer and idle loop, with the blow on frame 2 (Build Sprites se… |
| [TowerAttackTimingTests](#towerattacktimingtests) | Test fixture | Nilly |  |
| [TowerClickRuleTests](#towerclickruletests) | Test fixture | Nilly |  |
| [TowerConditionTests](#towerconditiontests) | Test fixture | Nilly |  |
| [TowerIconDataTests](#towericondatatests) | Class | Nilly | Checks the tower data, not code: each tower's shop card (Adam's TowerList) shows the same picture as the tower standing on the map, not the placeholder square. |
| [TowerListDataTests](#towerlistdatatests) | Test fixture | Nilly | Checks Adam's TowerList prefab, not code: its shop cards are the game's six towers, in the same order as the number keys, and none of the sandbox test towers th… |
| [TowerRangeDataTests](#towerrangedatatests) | Test fixture | Nilly | Checks the tower prefabs' numbers, not code: a melee tower placed on the outer diagonal of a road corner must still reach the road. The road's centre line runs … |
| [TowerSellRuleTests](#towersellruletests) | Test fixture | Nilly |  |
| [TowerTempoTests](#towertempotests) | Test fixture | Nilly |  |
| [TowerUpgradeRuleTests](#towerupgraderuletests) | Test fixture | Nilly |  |

### BasicEnemyTests

`unity/Assets/_Game/Tests/EditMode/BasicEnemyTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/BasicEnemyTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly + Jason

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SetUp` | Method | `void` | `()` |  |  |
| `TearDown` | Method | `void` | `()` |  |  |
| `Health_IsDouble_CanTakeDoubleDamage` | Method | `void` | `()` |  |  |
| `Damage_IsInt_CanGetAndSet` | Method | `void` | `()` |  |  |
| `PathCoordinates_CanBeAssignedAndAccessed` | Method | `void` | `()` |  |  |
| `PathCoordinates_Vector2Overload_AssignsCorrectly` | Method | `void` | `()` |  |  |
| `TakeDamage_ExceedingHealth_TriggersDiedEventAndSetsIsDead` | Method | `void` | `()` |  |  |
| `Implements_IDamageable_And_IHealthReadable` | Method | `void` | `()` |  |  |
| `TakeDamage_DamageInfo_ReducesHealth_AndFiresOnDamaged` | Method | `void` | `()` |  |  |
| `ApplyStatusEffect_Slow_ModifiesSpeedMultiplier` | Method | `void` | `()` |  |  |
| `ApplyStatusEffect_StrongestSlowWins` | Method | `void` | `()` |  |  |
| `Knockback_PushesEnemyBackwardAlongPath` | Method | `void` | `()` |  |  |
| `AnEnemyThatReachedTheQueen_CanNoLongerBeHurtHealedOrKilled` | Method | `void` | `()` |  |  |
| `ReachDestination_RaisesItsEventOnce` | Method | `void` | `()` |  |  |
| `OnDamaged_IsRaisedOnce_ForAHitByNumber_AndForAHitWithDetails` | Method | `void` | `()` |  |  |
| `ADamageOverTime_RefreshedFasterThanItTicks_StillTicks` | Method | `void` | `()` |  |  |

Private members not listed: 2.

### BossDeathLogTests

`unity/Assets/_Game/Tests/EditMode/BossDeathLogTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/BossDeathLogTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Queen` | Const (private) | `ulong` |  |  |  |
| `AnOrdinaryDeath_Counts` | Method | `void` | `()` |  |  |
| `TheQueensOwnDeath_DoesNotCount` | Method | `void` | `()` |  |  |
| `AHuskSheRaised_DoesNotCount_SoItNeverComesBackTwice` | Method | `void` | `()` |  |  |
| `OnlyTheNewestDeaths_AreKept_UpToTheCapacity` | Method | `void` | `()` |  |  |
| `TakeNear_RaisesOnlyDeathsWithinReach_NewestFirst_AndCapsTheCount` | Method | `void` | `()` |  |  |
| `TakenDeaths_AreForgotten` | Method | `void` | `()` |  |  |
| `DeathsOlderThanTheMemory_AreDropped_NotRaised` | Method | `void` | `()` |  |  |
| `ADeathExactlyAtTheMemoryEdge_StillCounts` | Method | `void` | `()` |  |  |
| `RememberedDeaths_ReadOldestFirst` | Method | `void` | `()` |  |  |
| `Freshness_FadesFromOneToZeroOverTheMemory` | Method | `void` | `()` |  |  |
| `Freshness_WithNoMemory_IsZero` | Method | `void` | `()` |  |  |

Private members not listed: 1.

### CentipedeEnemyTests

`unity/Assets/_Game/Tests/EditMode/CentipedeEnemyTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/CentipedeEnemyTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly + Jason

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SetUp` | Method | `void` | `()` |  |  |
| `TearDown` | Method | `void` | `()` |  |  |
| `SplitsRemaining_DefaultsToMinusOne` | Method | `void` | `()` |  |  |
| `CanSplit_WithHealthGreaterThanOne_AndMinusOneSplits_ReturnsTrue` | Method | `void` | `()` |  |  |
| `CanSplit_WhenHealthIsOneOrLess_ReturnsFalse` | Method | `void` | `()` |  |  |
| `CanSplit_WhenHalvedHealthIsLessThanOne_ReturnsFalse` | Method | `void` | `()` |  |  |
| `CanSplit_WhenSplitsRemainingIsZero_ReturnsFalse` | Method | `void` | `()` |  |  |
| `CanSplit_WhenSplitsRemainingPositive_ReturnsTrue` | Method | `void` | `()` |  |  |
| `Split_CreatesTwoChildrenWithHalfHealth_AndDecrementsSplitsRemaining` | Method | `void` | `()` |  |  |
| `Split_WithMinusOneSplitsRemaining_PreservesMinusOneInChildren` | Method | `void` | `()` |  |  |
| `Split_ScalesDownChildrenVisualScale` | Method | `void` | `()` |  |  |
| `Split_ChildrenInheritPathAndCurrentWaypointIndex` | Method | `void` | `()` |  |  |
| `TakeDamage_Fatal_WhenSplitsRemainingIsZero_DoesNotSplit` | Method | `void` | `()` |  |  |
| `TakeDamage_Fatal_WhenOneHp_DoesNotSplit` | Method | `void` | `()` |  |  |
| `TakeDamage_Fatal_WhenEligible_TriggersSplitEvent` | Method | `void` | `()` |  |  |
| `Split_SpawnsFirstChildAtOriginalPosition_AndSecondChildForwardOnPath` | Method | `void` | `()` |  |  |
| `CalculatePositionAlongPath_AdvancesAlongCornersAndUpdatesWaypointIndex` | Method | `void` | `()` |  |  |
| `Split_NearTheEndOfThePath_KeepsTheSecondChildOffTheLastWaypoint` | Method | `void` | `()` |  |  |
| `Split_PastTheLastWaypoint_GivesNoChildAnIndexItCanNeverWalkTo` | Method | `void` | `()` |  |  |
| `Split_WithASplitPrefabThatIsNoCentipede_ErrorsAndCopiesItselfInstead` | Method | `void` | `()` |  |  |
| `Split_HalvesDamageInChildren` | Method | `void` | `()` |  |  |
| `Split_WithDamageTen_HalvesDamageToFive` | Method | `void` | `()` |  |  |
| `Split_WithDamageOne_MaintainsMinimumDamageOfOne` | Method | `void` | `()` |  |  |
| `Split_WithDamageZero_PreservesZeroDamage` | Method | `void` | `()` |  |  |

Private members not listed: 3.

### DiatomaceousEarthTests

`unity/Assets/_Game/Tests/EditMode/DiatomaceousEarthTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/DiatomaceousEarthTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SettleShare_FadesInFromAHaze` | Method | `void` | `()` |  |  |
| `SettleShare_IsSolidOnceSettled` | Method | `void` | `()` |  |  |
| `SettleShare_NoSettleTime_IsSolidAtOnce` | Method | `void` | `()` |  |  |

### EnemyHurtAndDeathDataTests

`unity/Assets/_Game/Tests/EditMode/EnemyHurtAndDeathDataTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/EnemyHurtAndDeathDataTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

Checks the enemy prefabs, not code: each one plays its own sheet's Hurt and Death rows on its Body, holding its walk (Build Sprites sets this). The pill bug's s…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Folder` | Const (private) | `string` |  |  |  |
| `EachEnemy_FlinchesAndDiesWithItsOwnArt` | Method | `void` | `(string prefabName, string sheet)` |  |  |
| `TheRollyPolly_HasNoRowsToPlay_AndIsLeftAsItWas` | Method | `void` | `()` |  |  |

Private members not listed: 1.

### EnemyShieldTests

`unity/Assets/_Game/Tests/EditMode/EnemyShieldTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/EnemyShieldTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `StartsFull_AndSoaksAHitSmallerThanItself` | Method | `void` | `()` |  |  |
| `OnlyTheExcessGetsThrough_WhenAHitEmptiesTheShield` | Method | `void` | `()` |  |  |
| `ZeroOrNegativeDamage_ChangesNothing` | Method | `void` | `()` |  |  |
| `DoesNotRegrow_BeforeTheDelayHasPassed` | Method | `void` | `()` |  |  |
| `RegrowsAtTheRate_OnceLeftAloneForTheDelay` | Method | `void` | `()` |  |  |
| `AnyHit_RestartsTheWait_EvenOneFullySoaked` | Method | `void` | `()` |  |  |
| `Fraction_FollowsTheShieldLeft` | Method | `void` | `()` |  |  |

Private members not listed: 1.

### EnemySpawnerTests

`unity/Assets/_Game/Tests/EditMode/EnemySpawnerTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/EnemySpawnerTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Jason

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SetUp` | Method | `void` | `()` |  |  |
| `TearDown` | Method | `void` | `()` |  |  |
| `LivingEnemyCount_InitializesToZero` | Method | `void` | `()` |  |  |
| `SpawnEnemy_IncrementsLivingEnemyCount` | Method | `void` | `()` |  |  |
| `EnemyDeath_ViaStaticEvent_DecrementsLivingEnemyCount` | Method | `void` | `()` |  |  |
| `EnemyDestinationReached_ViaStaticEvent_DecrementsLivingEnemyCount` | Method | `void` | `()` |  |  |
| `CentipedeSplit_ViaStaticEvent_IncrementsLivingEnemyCountByChildCount` | Method | `void` | `()` |  |  |
| `CentipedeDefeatAndSplit_EndToEnd_CorrectlyUpdatesCounter` | Method | `void` | `()` |  |  |
| `GenericEnemiesSpawned_ViaStaticEvent_IncrementsLivingEnemyCount` | Method | `void` | `()` |  |  |
| `LivingEnemyCount_DoesNotDropBelowZero` | Method | `void` | `()` |  |  |
| `SetActiveMap_ResetsLivingEnemyCountToZero` | Method | `void` | `()` |  |  |
| `NotifyEnemyDefeated_DecrementsLivingEnemyCount` | Method | `void` | `()` |  |  |

Private members not listed: 4.

### HoneydewTests

`unity/Assets/_Game/Tests/EditMode/HoneydewTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/HoneydewTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `StartsAtTheStartingAmount_NeverBelowZero` | Method | `void` | `()` |  |  |
| `Earn_Adds_AndIgnoresZeroOrLess` | Method | `void` | `()` |  |  |
| `Spending_FailsAndChangesNothing_WhenShort` | Method | `void` | `()` |  |  |
| `Changed_IsRaisedWithTheNewAmount_OnlyWhenItChanges` | Method | `void` | `()` |  |  |

### LevelCameraFitTests

`unity/Assets/_Game/Tests/EditMode/LevelCameraFitTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/LevelCameraFitTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `At16By9_TheHeightWithItsHudBands_SetsTheSize` | Method | `void` | `()` |  |  |
| `At4By3_TheWidthSetsTheSize` | Method | `void` | `()` |  |  |
| `WithTheListBand_TheWholeMapSitsLeftOfIt_AtEveryAspect` | Method | `void` | `()` |  |  |
| `WithNoRightBand_TheCameraStaysCentred` | Method | `void` | `()` |  |  |
| `ABiggerBottomBand_LowersTheCamera_SoTheMapSitsHigher` | Method | `void` | `()` |  |  |

### LevelCatalogueTests

`unity/Assets/_Game/Tests/EditMode/LevelCatalogueTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/LevelCatalogueTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `HasTheTwoAlphaLevels_InOrder` | Method | `void` | `()` |  |  |
| `EveryLevel_HasAUniqueIdAndATitle` | Method | `void` | `()` |  |  |
| `EveryLevelId_RoundTripsThroughTheSceneNameRule` | Method | `void` | `()` |  |  |
| `TryFind_FindsListedIds_AndRejectsOthers` | Method | `void` | `()` |  |  |
| `TheControlsHelp_OpensOnlyOnTheTutorial` | Method | `void` | `()` |  |  |

### LevelScenesTests

`unity/Assets/_Game/Tests/EditMode/LevelScenesTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/LevelScenesTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SceneNames_AreTheIdPlusASuffix` | Method | `void` | `()` |  |  |
| `TryGetLevelId_ReadsTheIdBackFromEitherScene` | Method | `void` | `()` |  |  |
| `TryGetLevelId_StillFitsHill01` | Method | `void` | `()` |  |  |
| `TryGetLevelId_RejectsEveryOtherScene` | Method | `void` | `(string sceneName)` |  |  |
| `TryGetLevelId_IsCaseSensitive` | Method | `void` | `()` |  |  |
| `UnloadOrder_GameplayBeforeTerrain_EvenWhenGameplayLoadedFirst` | Method | `void` | `()` |  |  |
| `UnloadOrder_ANormalLoad_StillUnloadsGameplayFirst` | Method | `void` | `()` |  |  |
| `UnloadOrder_OtherScenesSitBetween_LastLoadedFirst` | Method | `void` | `()` |  |  |
| `UnloadOrder_NothingLoaded_IsEmpty` | Method | `void` | `()` |  |  |

### NextWaveRuleTests

`unity/Assets/_Game/Tests/EditMode/NextWaveRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/NextWaveRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `CanSend_BeforeTheFirstWave_AndBetweenWaves` | Method | `void` | `()` |  |  |
| `CannotSend_WhileAWaveIsComingOut_OrAfterTheLast` | Method | `void` | `()` |  |  |
| `CannotSend_WhilePaused` | Method | `void` | `()` |  |  |
| `CannotSend_OnceTheRunIsOver` | Method | `void` | `()` |  |  |

### PowerUpRuleTests

`unity/Assets/_Game/Tests/EditMode/PowerUpRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/PowerUpRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `RollDrop_DropsOnlyBelowTheChance` | Method | `void` | `()` |  |  |
| `RollDrop_SplitsTheDroppingRange_EvenlyBetweenTheKinds` | Method | `void` | `()` |  |  |
| `RollDrop_NeverDrops_WithNoChance_OrABadRoll` | Method | `void` | `()` |  |  |
| `RollDrop_SeededRandom_DropsAboutTheChance` | Method | `void` | `()` |  |  |
| `CanGift_OnlyBelowTheLastLevel` | Method | `void` | `()` |  |  |

### PowerUpSlotTests

`unity/Assets/_Game/Tests/EditMode/PowerUpSlotTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/PowerUpSlotTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `StartsEmpty_AndStoresOne` | Method | `void` | `()` |  |  |
| `AFullSlot_RefusesASecondDrop_AndKeepsTheFirst` | Method | `void` | `()` |  |  |
| `Take_EmptiesTheSlot_AndHandsBackWhatItHeld` | Method | `void` | `()` |  |  |

### QueenMantisHealRuleTests

`unity/Assets/_Game/Tests/EditMode/QueenMantisHealRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/QueenMantisHealRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Radius` | Const (private) | `float` |  |  |  |
| `AHurtEnemyWithinReach_IsHealed` | Method | `void` | `()` |  |  |
| `SheNeverHealsHerself` | Method | `void` | `()` |  |  |
| `TheDead_AreNotHealed` | Method | `void` | `()` |  |  |
| `AnEnemyAtFullHealth_IsLeftAlone` | Method | `void` | `()` |  |  |
| `AnEnemyOutOfReach_IsNotHealed` | Method | `void` | `()` |  |  |
| `TheEdgeOfHerReach_Counts` | Method | `void` | `()` |  |  |

### QueenMantisShadeTests

`unity/Assets/_Game/Tests/EditMode/QueenMantisShadeTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/QueenMantisShadeTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `ThirdsLost_FollowsTheHealthLeft` | Method | `void` | `(double fraction, int expected)` |  |  |
| `ExactThirds_CountAsLost` | Method | `void` | `()` |  |  |
| `OutOfRangeFractions_ReadAsFullOrEmpty` | Method | `void` | `()` |  |  |
| `TheShades_GoWhiteBlackWhite_AsInDecision108` | Method | `void` | `()` |  |  |
| `TheCycle_StartsWhite` | Method | `void` | `()` |  |  |
| `TheCycle_ChangesAtEachThird` | Method | `void` | `()` |  |  |
| `HealingBackUp_NeverReturnsAShade` | Method | `void` | `()` |  |  |
| `ABigHitThatSkipsAThird_StillStepsThroughBlack` | Method | `void` | `()` |  |  |
| `AfterTheLastThird_NothingMoreChanges` | Method | `void` | `()` |  |  |

### QueenMantisSummonRuleTests

`unity/Assets/_Game/Tests/EditMode/QueenMantisSummonRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/QueenMantisSummonRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Tolerance` | Const (private) | `float` |  |  |  |
| `TheFirstEscort_StandsStraightAboveHer` | Method | `void` | `()` |  |  |
| `EveryEscort_IsOnTheRing` | Method | `void` | `(int count)` |  |  |
| `Escorts_AreSpreadEvenly_NoTwoInOnePlace` | Method | `void` | `()` |  |  |
| `ACountBelowOne_StillGivesAPlace` | Method | `void` | `()` |  |  |

### QueenTests

`unity/Assets/_Game/Tests/EditMode/QueenTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/QueenTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly + Jason

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SetUp` | Method | `void` | `()` |  |  |
| `TearDown` | Method | `void` | `()` |  |  |
| `Health_IsDouble_AndInitializedToMaxHealth` | Method | `void` | `()` |  |  |
| `TakeDamage_ReducesCurrentHealth_AndFiresHealthChanged` | Method | `void` | `()` |  |  |
| `HandleEnemyReachedDestination_SubtractsEnemyDamageFromQueenHealth` | Method | `void` | `()` |  |  |
| `EnemyReachingDestination_ViaStaticEvent_ReducesQueenHealth` | Method | `void` | `()` |  |  |
| `HealthDepletedToZeroOrBelow_FiresGameOverEventExactlyOnce` | Method | `void` | `()` |  |  |
| `Heal_RestoresHealthUpToMaxLimit` | Method | `void` | `()` |  |  |
| `ResetHealth_RestoresHealthAndResetsGameOver` | Method | `void` | `()` |  |  |

Private members not listed: 4.

### RangeRingTests

`unity/Assets/_Game/Tests/EditMode/RangeRingTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/RangeRingTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `CirclePoints_AllLieOnTheRange_AroundTheTower` | Method | `void` | `()` |  |  |
| `CirclePoints_GoAllTheWayRound` | Method | `void` | `()` |  |  |

### RollyPollyCurlTests

`unity/Assets/_Game/Tests/EditMode/RollyPollyCurlTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/RollyPollyCurlTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `TheFirstHit_LandsInFull_AndCurlsIt` | Method | `void` | `()` |  |  |
| `HitsWhileCurled_LandAtTheReducedShare_AndDoNotUseACurl` | Method | `void` | `()` |  |  |
| `UncurlsAfterRollingTheDistance_AndSaysSoOnce` | Method | `void` | `()` |  |  |
| `CurlsAtMostThreeTimes_ThenEveryHitLandsInFull` | Method | `void` | `()` |  |  |
| `ZeroOrNegativeDamage_NeitherLandsNorCurls` | Method | `void` | `()` |  |  |
| `NumbersAreClamped_ToSensibleRanges` | Method | `void` | `()` |  |  |

Private members not listed: 1.

### RunClockTests

`unity/Assets/_Game/Tests/EditMode/RunClockTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/RunClockTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `StartsRunning_AtNormalSpeed` | Method | `void` | `()` |  |  |
| `ToggleSpeed_SwitchesBetweenNormalAndFast` | Method | `void` | `()` |  |  |
| `Pausing_StopsTime_AndResumingKeepsTheChosenSpeed` | Method | `void` | `()` |  |  |
| `Changed_IsRaised_OnlyWhenSomethingChanges` | Method | `void` | `()` |  |  |
| `FastScale_FallsBackToDouble_WhenNotFasterThanNormal` | Method | `void` | `()` |  |  |

### RunOutcomeRuleTests

`unity/Assets/_Game/Tests/EditMode/RunOutcomeRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/RunOutcomeRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Playing_WhileWavesRemain` | Method | `void` | `()` |  |  |
| `Playing_WhileTheLastWaveStillHasEnemies` | Method | `void` | `()` |  |  |
| `Won_WhenEveryWaveIsSentAndNoEnemyIsLeft` | Method | `void` | `()` |  |  |
| `Lost_WhenTheQueenFalls_EvenWithTheMapCleared` | Method | `void` | `()` |  |  |

### SpriteRowTests

`unity/Assets/_Game/Tests/EditMode/SpriteRowTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/SpriteRowTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `FrameAt_PlaysTheRowOnce` | Method | `void` | `()` |  |  |
| `Length_IsEveryFrameOnce` | Method | `void` | `()` |  |  |
| `ADeath_StaysSolidThroughTheRow_ThenFades` | Method | `void` | `()` |  |  |

### TargetPickerTests

`unity/Assets/_Game/Tests/EditMode/TargetPickerTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TargetPickerTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `First_PicksTheEnemyFurthestAlongThePath` | Method | `void` | `()` |  |  |
| `Last_PicksTheEnemyLeastFarAlongThePath` | Method | `void` | `()` |  |  |
| `Closest_PicksTheEnemyNearestTheTower` | Method | `void` | `()` |  |  |
| `NoCandidates_PicksNothing_AndTiesGoToTheEarlierOne` | Method | `void` | `()` |  |  |
| `CanReach_OnlyAStraightShotReachesAFlyer_EverythingReachesTheGround` | Method | `void` | `()` |  |  |
| `AttacksOnItsOwn_OnlyWithoutAnAbility_AndWithSomethingToHitWith` | Method | `void` | `()` |  |  |
| `PathProgress_IsDistanceWalked_FromWaypointIndexAndPosition` | Method | `void` | `()` |  |  |

Private members not listed: 1.

### TimedNoticeTests

`unity/Assets/_Game/Tests/EditMode/TimedNoticeTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TimedNoticeTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `ANotice_ShowsForItsTime_ThenRunsOut` | Method | `void` | `()` |  |  |
| `Clear_DropsTheNotice_SoTheNextRunStartsWithout` | Method | `void` | `()` |  |  |
| `ANewNotice_ReplacesTheOneUp_AndRestartsTheTime` | Method | `void` | `()` |  |  |
| `NothingShown_GivesNoText` | Method | `void` | `()` |  |  |

### TowerAttackDataTests

`unity/Assets/_Game/Tests/EditMode/TowerAttackDataTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerAttackDataTests.cs)) · Class · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

Checks the tower prefabs, not code: each one carries its sheet's attack row, wired to its own renderer and idle loop, with the blow on frame 2 (Build Sprites se…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EachTower_PlaysItsOwnAttackRow` | Method | `void` | `(string tower, string sheet)` |  |  |

### TowerAttackTimingTests

`unity/Assets/_Game/Tests/EditMode/TowerAttackTimingTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerAttackTimingTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `FrameSeconds_UsesTheSheetsTime_WhenTheTowerIsSlowerThanTheRow` | Method | `void` | `()` |  |  |
| `FrameSeconds_SqueezesTheRow_IntoTheTimeBetweenAttacks` | Method | `void` | `()` |  |  |
| `FrameSeconds_NeverDropsBelowTheFloor` | Method | `void` | `()` |  |  |
| `WindUp_IsTheFramesBeforeTheHit` | Method | `void` | `()` |  |  |
| `ShouldWindUp_OnlyForAReadyArmedTowerWithATarget` | Method | `void` | `()` |  |  |
| `FrameAt_StepsThroughTheRow_ThenEnds` | Method | `void` | `()` |  |  |
| `TheHitFrame_ShowsAsTheTowerBecomesReady` | Method | `void` | `()` |  |  |

### TowerClickRuleTests

`unity/Assets/_Game/Tests/EditMode/TowerClickRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerClickRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `ClickOnStandingTower_SelectsIt_WithOrWithoutATowerInHand` | Method | `void` | `()` |  |  |
| `ClickOnEmptyCell_WithNothingInHand_ClearsTheSelection` | Method | `void` | `()` |  |  |
| `ClickWithTowerInHand_BuildsOnlyOnAFreeAffordableCell` | Method | `void` | `()` |  |  |
| `TheMapCells_HoldBothRoutes_AndNothingPastTheirEdges` | Method | `void` | `()` |  |  |

### TowerConditionTests

`unity/Assets/_Game/Tests/EditMode/TowerConditionTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerConditionTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `AFreshTower_ActsAndItsHitsLand` | Method | `void` | `()` |  |  |
| `Burrowed_DoesNotAct_ButIsNotStunned` | Method | `void` | `()` |  |  |
| `Stunned_DoesNotAct_UntilTheTimeHasPassed` | Method | `void` | `()` |  |  |
| `Missing_StillActs_ButItsHitsDoNothing` | Method | `void` | `()` |  |  |
| `AShorterSpell_NeverCutsALongerOneShort` | Method | `void` | `()` |  |  |
| `StunLeftShare_RunsFromOneToZero_AndALongerStunStartsAgainFull` | Method | `void` | `()` |  |  |
| `BurrowAll_BurrowsWhileAnyTowerIsUp_AndRaisesOnlyWhenAllAreDown` | Method | `void` | `()` |  |  |

### TowerIconDataTests

`unity/Assets/_Game/Tests/EditMode/TowerIconDataTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerIconDataTests.cs)) · Class · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

Checks the tower data, not code: each tower's shop card (Adam's TowerList) shows the same picture as the tower standing on the map, not the placeholder square.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `DataFolder` | Const (private) | `string` |  |  |  |
| `PrefabFolder` | Const (private) | `string` |  |  |  |
| `TheShopIcon_IsTheTowersOwnArt` | Method | `void` | `(string tower)` |  |  |

### TowerListDataTests

`unity/Assets/_Game/Tests/EditMode/TowerListDataTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerListDataTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

Checks Adam's TowerList prefab, not code: its shop cards are the game's six towers, in the same order as the number keys, and none of the sandbox test towers th…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `TowerListPrefab` | Const (private) | `string` |  |  |  |
| `TheShopListsTheSixTowers_InNumberKeyOrder` | Method | `void` | `()` |  |  |

Private members not listed: 1.

### TowerRangeDataTests

`unity/Assets/_Game/Tests/EditMode/TowerRangeDataTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerRangeDataTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

Checks the tower prefabs' numbers, not code: a melee tower placed on the outer diagonal of a road corner must still reach the road. The road's centre line runs …

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `TowerPrefabFolder` | Const (private) | `string` |  |  |  |
| `CellSize` | Const (private) | `float` |  |  |  |
| `MeleeTowers_ReachTheRoadFromACornersOuterDiagonal_AtEveryLevel` | Method | `void` | `()` |  |  |

Private members not listed: 1.

### TowerSellRuleTests

`unity/Assets/_Game/Tests/EditMode/TowerSellRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerSellRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Refund_IsTheFractionOfEverythingSpent_RoundedDown` | Method | `void` | `()` |  |  |
| `Refund_NeverGoesNegative_OrAboveWhatWasSpent` | Method | `void` | `()` |  |  |

### TowerTempoTests

`unity/Assets/_Game/Tests/EditMode/TowerTempoTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerTempoTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Frame` | Const (private) | `float` |  |  |  |
| `NextCooldown_KeepsThePartOfTheFrameTheShotWasLateBy` | Method | `void` | `()` |  |  |
| `NextCooldown_GivesNoCreditForSittingIdle` | Method | `void` | `()` |  |  |
| `NextCooldown_ExactlyOnTime_IsTheFullInterval` | Method | `void` | `()` |  |  |
| `NextCooldown_FollowsTheSwarmFrenzyTempo_AndNeverGoesBelowNormal` | Method | `void` | `()` |  |  |
| `FiresAtItsStatedRate_OverManyFrames` | Method | `void` | `()` |  |  |

### TowerUpgradeRuleTests

`unity/Assets/_Game/Tests/EditMode/TowerUpgradeRuleTests.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Tests/EditMode/TowerUpgradeRuleTests.cs)) · Test fixture · `HoldTheHill.Tests` · HoldTheHill.Tests.EditMode · owner: Nilly

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Ready_WhenBelowMaxLevelAndTheNextLevelIsAffordable` | Method | `void` | `()` |  |  |
| `CannotAfford_WhenShortByEvenOne` | Method | `void` | `()` |  |  |
| `AtMaxLevel_BeatsThePriceCheck` | Method | `void` | `()` |  |  |
| `AtMaxLevel_ForATowerWithNoUpgradesAtAll` | Method | `void` | `()` |  |  |
