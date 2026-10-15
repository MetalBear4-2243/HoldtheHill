# Design gaps: the GDD against *Level Up!*

Proposal from Nilly, written 2026-10-07, condensed 2026-10-14. Nothing here has been agreed by
the team; tiers and dates are suggestions. The full version (96 tasks in 15 streams, and a
checklist of every piece of book advice under its task) is on Nilly's machine; ask for it.

## How the GDD scores

The course book (Scott Rogers, *Level Up!*) was split into 1,740 pieces of advice and checked
against the ten-page GDD and the two paper levels: **404 covered, 674 partly covered, 347
missing, 315 do not apply** to a single-player tower defense. "Covered" means the GDD says it,
not that the build does it. The weakest chapters: music and sound (0 covered), HUD and icons,
power-ups and economy, production.

## The ten biggest gaps

1. **No numbers to tune with.** Tower damage, range, fire rate, enemy speed and hill damage are
   mostly not written down. One stat matrix and a time-to-kill check come before tuning any wave.
2. **No master beat chart.** Nothing shows wave by wave which enemy, hazard, Warden action, tower
   unlock, power-up and story beat arrives. All 14 castes are open from wave 1.
3. **No scope cut line for December 4.** No Must / Should / Could list, freeze dates or fallback
   for risky features (Hornet, Hand, seasons, theme packs).
4. **The documents disagree.** Grid size (20×11 in the GDD, 24×13 and 25×13 on the paper levels),
   when hazards start, how many enemy types Level 1 has.
5. **Punishments the book calls unfair.** The Hornet removes a tower for the whole run, the Hand
   flattens towers, the Snail is nearly immune, smoke hides health bars. The book: never punish
   twice, always show it coming, always leave an answer.
6. **Almost no audio design.** Two sentences in the GDD, yet a soundtrack is on the price list.
7. **No playtest plan or test tools.** No outside testers, bug template, test scene or exploit list.
8. **The player is not taught.** Repair, Rally, Dig, Carry and the wave track have no prompts; no
   difficulty setting or help after repeated losses.
9. **The Warden is a list of verbs.** No name, traits, silhouette rules or movement feel; the
   story disappears between the first panel and the boss.
10. **Missing paperwork.** No one-sheet, selling points, comparable games, version history,
    glossary, technical section or licence log.

## Where the book argues with the current design

These are decisions, not errors.

| Current design | The book's advice |
|---|---|
| The Hornet takes a tower for the rest of the run | Don't punish twice; let the player win it back |
| All 14 castes buildable from wave 1 | Introduce tools a few at a time, the counter before the enemy |
| Free, unlimited wave retry | A retry easier than failing becomes a reset button |
| Opens on a quiet founding panel, ends on a cliffhanger | Open on action; give the ending closure plus a hook |
| The Titan charges at three times normal speed | Big enemies slower than the player unless clearly warned |
| About twenty keys, a long hold on F | Keys under the hand that needs them; toggle instead of hold; more than one layout |

## The order that matters

One chain carries most of the design work; nothing downstream is worth tuning until the step
before it exists:

**Lock grid, camera and Warden kit → level metrics → tower stat matrix + enemy sheets → counter
chart → unlock order → master beat chart → economy sheet → difficulty settings.**

## Suggested first tasks, by milestone

| Milestone | Must-have design work |
|---|---|
| **Pitch, Oct 16** | Scope tiers and cut order; one-page identity (objective, theme, fantasy); lock grid size, camera and Warden kit; schedule with owners and freeze dates; fix the contradictions; one-sheet; pitch deck; a playable build check |
| **Alpha, Nov 6** | Level metrics; tower stat matrix and enemy sheets (same sitting); counter chart; unlock schedule and master beat chart (same sitting); hazard specs and one warning language; HUD inventory; icon and readability rules; control scheme; playtest plan; audio source and licence plan |
| **Beta, Nov 20** | Warden specs and movement feel; boss sheets and fairness rules; economy sheet; onboarding prompts; save design; menus; un-fun pass after each graybox |
| **Release, Dec 4** | Boss tuning: each boss beatable by at least three tower mixes |

If the team has to choose inside Alpha: grid and kit first, then metrics, then stats with
enemies, then counters, unlocks, and the beat chart.
