# Design gaps: the GDD against *Level Up!*

Proposal from Nilly. First written 2026-10-07 against the first ten-pager; rechecked 2026-10-14
against the V2 ten-page GDD; **rechecked 2026-11-05 against the V4 ten-pager (the submitted GDD
of 2026-10-16, titled Draft V3 inside; Drive, `Design Documentation/V4 10 page.pdf`) and against
what the Alpha build on Indev actually contains** (the AI devlog rows of 2026-10-16 to 2026-11-04).
Page numbers below are pages of that PDF. Nothing here has been agreed by the team; tiers and
dates are suggestions. The Alpha demo is 2026-11-06, Beta 2026-11-20, Release 2026-12-04.

## What changed

### V4 against V2

V4 keeps V2's shape (no Warden, six towers, six enemies, two levels, burrowing, a story from the
meadow to the moon) and changes the rest:

- **One currency.** Food is gone. Honeydew builds and upgrades towers and "buys in-run boosts"
  (p. 4, p. 10). Starting Honeydew is a yellow TBD.
- **Top-down camera**, the whole level on one screen (p. 3). The 3/4 side view is gone and every
  screenshot is of the top-down test build.
- **Click squash is silverfish only.** One click kills a silverfish; "clicking does nothing to any
  other bug" (p. 5). The 4.2% chip and the 5-click cap are gone.
- **New bosses.** The Centipede ends Tutorial Hill and the Queen Mantis ends the Meadow (p. 6, p. 8).
  The Stag Beetle and the Titan are gone. Centipede health is a yellow TBD.
- **The Meadow is ten waves** (p. 4, p. 6), no longer "variable".
- **Power-ups are clicked, held in a slot and used** (p. 7). The worker carry is gone. Two are
  named, Swarm Frenzy and Royal Gift, each with a "same family" pair; drop chances and the full
  pool are a yellow TBD.
- **Towers have no health and hazards "never wear them down"** (p. 3); diatomaceous earth now
  stuns (p. 7). The "can even kill" line is gone.
- **Numbers are labelled.** Tower prices are "placeholders from the test build" (p. 3); enemy
  numbers are "test-build values" and the Rolly Polly's are placeholders (p. 8). The Rhino
  Beetle's shield is 120 on top of 160 health.
- **The HUD key has five items** and the tower list is one of them (p. 5).
- **New sections:** a controls table and an "ant technology" ladder (p. 3), a one-wave flow (p. 4),
  modes (p. 7), bonus content and a roadmap (p. 9), monetization (p. 10).
- **Unchanged:** four hazards at Alpha with Gust and Sap drip after; no audio section; Beta is
  "balance, polish and playtesting"; ESRB T and ages 16 up; no grid size; wave contents are still
  "TBD: wave spreadsheet".

Six yellow boxes remain: contact email (p. 1), wave spreadsheet and starting Honeydew (p. 4), drop
chances and boost pool (p. 7), Centipede health (p. 8), DLC pack price (p. 10).

### The document against the Alpha build

The build was made from this document between 2026-10-16 and 2026-11-04, so the two agree on the
big things: Honeydew and bounties, six towers with three levels each, six enemies with the stated
mechanics, both levels with both bosses, four hazards, burrowing, the two power-ups, top-down
sprites, the HUD as drawn on page 5.

In the game but not in the document: a Sell button that refunds three quarters; upgrade prices
and per-level damage, range and fire interval for every tower, held as data on the prefabs; a
tower card with combat stats; pause, win and lose panels with Restart and Home; a home screen with
level select; a boss health bar; the Queen Mantis arrives with three Mantis Spawn escorts and two
more per shade change, pauses a second to pray, and draws rings on the bodies she can raise; the
Centipede is seven parts, each a quarter faster than its parent, 10 bounty each, with damage
halving per split; the drop chance is 8%, Swarm Frenzy doubles fire rate for 10 s, a second drop
fades after 15 s, Royal Gift refuses a level-3 tower; melee ants hit everything in reach; the
pesticide stun lasts 4 s and towers dig up on their own after it; the dust settles 1.5 s before it
hurts; each level brings one new enemy per wave and the Queen Mantis walks in ninth in Meadow
wave 10; dev keys F1 and F5 to F9; a Windows development build. The numbers were tuned by one
person's play passes, not from a stat matrix.

In the document but not in the game: a standing order on every tower (p. 3, p. 5; the Sap and
Carpenter Ants have none); macOS (p. 1; only Windows is built); enemies re-routing where the path
forks under diatomaceous earth (p. 7; not recorded as built); Honeydew for finished waves and boss
rewards (p. 4, p. 10; the devlog records kill bounties, pay per finished wave is not recorded, so
check it); "in-run boosts" bought with Honeydew (p. 4, p. 10; nothing in the build sells a boost);
the four "same family" power-ups (p. 7); the planned modes, cyber ants and DLC. Neither has audio
or a settings screen.

The original pass split the course book (Scott Rogers, *Level Up!*) into 1,740 pieces of advice
and scored the first draft: 404 covered, 674 partly, 347 missing, 315 not applicable. **That
count has not been re-run for V2 or V4.** This page rechecks the ten biggest gaps and adds what V4
and the build introduce; a full re-score is a bigger job.

## The ten gaps, rechecked

| # | Gap in the first draft | In V4 | Still to do |
|---|---|---|---|
| 1 | No numbers to tune with | **Partly closed, and the build is ahead of the document.** Health, speed and bounty for all six enemies, the Rhino's 120 shield, six tower prices (p. 3, p. 8), all labelled test-build placeholders. Starting Honeydew and Centipede health are TBD. Tower damage, range, rate and upgrade prices exist only as prefab data in Unity; enemy damage to the Queen and Queen Mantis health are stated nowhere | One stat matrix on the design side, written from the prefab data: six towers by three levels, six enemies plus two bosses, damage to the Queen, starting Honeydew. Then the time-to-kill check the book wants before any wave is tuned |
| 2 | No master beat chart | **Open.** Wave contents are still "TBD: wave spreadsheet" (p. 4). The build has a wave asset and a hazard schedule asset per level, one new enemy per wave, boss appended to the last wave; nothing on the design side | A wave-by-wave chart for L00 and L01 written from the built assets: enemy introduced, hazard, tower unlocked, power-up, lesson, intensity. Say which towers are open on wave 1 and whether anything unlocks later (the build opens all six) |
| 3 | No scope cut line | **Closed for Alpha.** The roadmap (p. 9) says Alpha is Tutorial Hill and the Meadow, six towers, six enemies, two bosses, and that is what was built. Beta is "balance, polish and playtesting" and Release is "the full game", neither with a list | A Beta list (fixes, not features) and a Release list; freeze dates; an owner per system; and a plain statement of what Release contains, since the story levels are after-launch updates (see disagreements) |
| 4 | The documents disagree | **Six of the nine V2 items settled; three remain; seven new** (next section) | One pass to settle them, in the document, before Beta |
| 5 | Punishments the book calls unfair | **Mostly closed.** Burrowing answers the bird shadow and the pesticide (p. 7); towers cannot be destroyed (p. 3). The document gives no warning for diatomaceous earth, stingers or the pesticide, and no longer says the bird's timer is visible. The build warns in places the document does not: dust settles 1.5 s before it hurts, the pesticide shows a cloud then a 4 s ring, the bird drifts in | Write the warning for each hazard as built (picture, timing) into the document and decide the two with none (stinger burst, bird timer on screen). Say what a snatched tower costs: the build frees the cell and refunds nothing, the document does not say |
| 6 | Almost no audio design | **Open, and absent in both.** V4 has no audio section; the build has no audio at all, 29 days before Release | Decide where sound and music come from, the licence, and a list of about ten sounds (a warning per hazard, boss entry, build, sell, win, lose), or write down the decision to ship silent |
| 7 | No playtest plan or test tools | **Open.** Beta still says "balance, polish and playtesting" (p. 9). The build has dev keys F1 and F5 to F9 and has been played by one person in the Editor; no outsider has played it | Who tests when (team after Alpha, outsiders before Beta), a bug template, and a test scene or key that fires any hazard and spawns any enemy |
| 8 | The player is not taught | **Partly closed.** Tutorial Hill is "five waves that teach the basics, ending with the Centipede" (p. 6). What each wave teaches is not stated. The build brings one new enemy per wave and shows a developer help box on Tutorial Hill only | A lesson per wave (place, standing orders, burrow, squash, collect, upgrade, sell), the prompt for each, and a name card the first time each enemy and each hazard appears |
| 9 | The Warden is a list of verbs | **No longer applies.** "There is no hero ant" (p. 3). The Queen Mantis has a line of personality (p. 8); the player's Queen has none. The towers now have silhouettes: the built sprite sheets | Whether the game has any text voice at all; if so, whose. One line per tower naming its ant for the name cards |
| 10 | Missing paperwork | **Partly closed.** Comparable games (p. 4), a roadmap (p. 9), monetization (p. 10) and the yellow-box convention, which is the start of an open-items list. The achievements list V2 had is not in V4. The version label is wrong (see disagreements) | Version history, glossary, a short technical section (Windows only or macOS too, resolution, input, saves, settings), and a licence log for fonts, sprite sheets, sounds and AI-made art |

## Where the GDD disagrees with itself

Settled since V2, dropped from the list: towers have no health and diatomaceous earth stuns
(p. 3, p. 7); "two bosses" now means the Centipede and the Queen Mantis and nothing else (p. 6,
p. 8, p. 9); the camera is top-down in the document and the build (p. 3); the HUD key has the
tower list as item 4 (p. 5); the four Alpha hazards and the Queen Mantis as the Meadow's boss are
in the document and in the build. The cut list was not re-read on 2026-11-05; if it still says
the hazards and the Queen Mantis were cut, it should be brought in line, not the other way round.

Still there from V2:

1. **Grid size is not stated.** Speeds are "tiles per second" and the Rolly Polly rolls "3 tiles"
   (p. 8), but no page says how big a tile is or how many tiles a level is.
2. **Wasps are both an enemy and a hazard.** Hazard 2 is "wasps drop stingers as they fly over"
   (p. 7) and the Wasp's unique mechanic is "flies" (p. 8). Say whether the hazard is the enemy's
   ability or a separate event. The build makes it the enemy's ability: a dropper on the Wasp
   prefab.
3. **Rating and audience.** ESRB T, ages 16 and up (p. 1), against a "comic" mood (p. 6) and
   *Plants vs. Zombies* as a comparable (p. 4). The book asks that the rating follow the content;
   say what earns the T or aim for E10+.

New in V4:

4. **The version label.** The title page says "DRAFT V3, October 15, 2026"; every footer says
   October 16; the file is named V4; the roadmap calls the Oct 16 pitch "this document" (p. 9).
   Pick one label and date and start a version history.
5. **What ships on December 4.** Release is "the full game, free on PC" (p. 9), but the story
   levels (kitchen, launch pad, rocket, moon) are "free updates" after launch (p. 9), and the
   middle act, The Reach (p. 2), has no level in any list. As written, Release is Tutorial Hill
   and the Meadow. Say so, or say what else is in it.
6. **Two after-launch lists.** "Beyond launch" names kitchen, launch pad, rocket and moon; the
   roadmap names "military levels, NASA kitchen, launch pad, rocket, moon" (both p. 9).
7. **Honeydew "buys in-run boosts"** (p. 4, p. 10), but the only boosts in the document are free
   power-up drops (p. 7) and nothing has a Honeydew price. Say what a bought boost is and what it
   costs, or cut the phrase.
8. **"Melee and artillery can't hit" the Wasp** (p. 8), but no tower is called artillery. The
   tower table (p. 3) has a molotov toss and mines. Name which of the six hit a Wasp. The build's
   answer is the Bullet Ant, the Carpenter's mines and the Sap aura; the Ant, Crazy Ant and Fire
   Ant skip it.
9. **"Every tower has a standing order"** (p. 3, p. 5), but the built Sap and Carpenter Ants have
   none, by decision on 2026-11-02. Either the document drops "every" or the build adds them.
10. **Enemies are not placed on levels.** The Mantis Spawn's unique mechanic is that the Queen
    Mantis can spawn and raise it (p. 8), which only happens in the Meadow, so on Tutorial Hill it
    has no mechanic. No page says which enemies appear on which level or in which wave; the build
    decides it in the wave assets.

## Where the book argues with V4

These are decisions, not errors.

| V4 design | The book's advice |
|---|---|
| Losing: the document says only "you lose" (p. 4); the build restarts the level | Ten fixed waves is short enough to replay, so a level restart is fine now; write it down, and reconsider if a level ever gets longer |
| The Centipede splits when it dies into smaller, faster parts (p. 8) | A boss that gets worse when hit needs a telegraph the player can learn; show how many parts remain and make the split read the same every time. The build splits into seven parts, each a quarter faster; the document should carry those numbers |
| A bird snatches a tower (p. 7) | Fine if it is clearly shown and answerable; don't also take the Honeydew (don't punish twice). The build refunds nothing and the document does not say |
| The Queen Mantis raises the last few bugs killed near her (p. 8) | Show which bodies she can raise, so "keep the fallen away from her" is a plan the player can see. The build already draws rings on them; put that in the document |
| "Clicking does nothing to any other bug" (p. 5) | A click with no response is a silent fail; give a small non-reward response (a puff, a sound once there is sound) so the player learns the rule in one try |
| Power-ups drop at random, are clicked, and sit in one slot (p. 7) | Random drops are fine if the player can plan around them: show the drop's lifetime (the build fades a second drop after 15 s), and name what the slot holds |
| The Wasp cannot be hit by melee or artillery (p. 8) | A hard counter works when the answer is on screen before the problem: the Wasp's first wave should come with the tower that hits it already open and named |

## The order that matters

One chain carries most of the design work; nothing downstream is worth tuning until the step
before it exists. The camera is now locked, and the build already holds the numbers, so most
of the chain is writing down what exists and then checking it:

**Lock grid size → level metrics → tower stat matrix + enemy sheets (from the prefab data) →
counter chart → which towers open when → wave spreadsheet (beat chart, from the wave assets) →
starting Honeydew and Honeydew budget per wave → difficulty modes.**

## Suggested next tasks, by milestone

| Milestone | Design work |
|---|---|
| **Before the Alpha demo, Nov 6** | Document only, and only if the ten-pager is shown: fix the version label and date on page 1, and the two captions the build has outgrown ("its labels still say Gold and Hill Base HP", p. 5; "sprites are top-down placeholders", p. 8). Nothing else in the document blocks the demo |
| **Beta, Nov 20** | Settle the ten disagreements above in the document; write the stat matrix and the wave chart from the built assets; fill the yellow boxes the build already answers (starting Honeydew, drop chance, Centipede health as built); write each hazard's warning as built and decide the two that have none; the snatched-tower refund rule; the lesson, prompt and name card per Tutorial Hill wave; the audio decision (source, licence, a list of about ten sounds, or ship silent); a playtest plan with people outside the team and a bug template; a plain statement of what Release contains |
| **Release, Dec 4** | Boss tuning: each boss beatable by at least three tower mixes; economy check against the wave chart; an un-fun pass after each level is played by an outsider; licence log for fonts, sprite sheets, sounds and AI-made art; version history, glossary and a short technical section (Windows only or macOS too, resolution, input, saves, settings screen); the difficulty-mode decision |
