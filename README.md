# Hold the Hill

A 2D Unity game by **Turbulent Towers Studio**.

## Setup (do this once)

1. **Install Unity 6000.6.0f1 exactly** through Unity Hub. Opening the project in any other version rewrites dozens of files and causes conflicts for everyone.
2. **Install Git LFS.** Art, audio, and other big files are stored with LFS. Without it you get tiny placeholder files instead of real assets.
   - Mac: `brew install git-lfs`, then `git lfs install`
   - Windows: included with [Git for Windows](https://git-scm.com/download/win); run `git lfs install`
   - GitHub Desktop already includes it.
3. **Clone the repo**, then open the `Hold the Hill` folder in Unity Hub (not the repo root).
4. **Set up Unity scene merging** (recommended). Run this inside the repo, using your Unity path:
   - Mac:
     ```
     git config merge.unityyamlmerge.name "Unity SmartMerge"
     git config merge.unityyamlmerge.driver '"/Applications/Unity/Hub/Editor/6000.6.0f1/Unity.app/Contents/Helpers/UnityYAMLMerge" merge -p %O %B %A %A'
     git config merge.unityyamlmerge.recursive binary
     ```
   - Windows:
     ```
     git config merge.unityyamlmerge.name "Unity SmartMerge"
     git config merge.unityyamlmerge.driver '"C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Tools/UnityYAMLMerge.exe" merge -p %O %B %A %A'
     git config merge.unityyamlmerge.recursive binary
     ```
   Without this, scene conflicts fall back to normal text merging. Nothing breaks, it's just messier.

## Where things go

```
Hold the Hill/Assets/
  _Game/            the real game - shared by everyone
    Scenes/         shared scenes (edit only after telling the group)
    Scripts/
    Prefabs/
    Art/
    Audio/
  _Sandbox/         personal test areas - only edit your own folder
    1103-Montgomery-Adam/
    MetalBear4-2243/
    nilly-ctrl/
  Settings/         render pipeline and input settings (leave alone)
```

**Build features as prefabs in your sandbox.** Test them in your own sandbox scene, then drop the finished prefab into a shared scene. Three people editing the same scene is the #1 cause of lost work in Unity.

## How we work

1. **Pull `main` before starting anything.**
2. **Make a branch for each task**, named `yourname/what-it-does`, e.g. `adam/enemy-spawner`.
3. **Keep branches small and short** - merge within a day or two.
4. **Open a pull request into `main`.** Don't push straight to `main`.
5. **Say in Discord before editing a shared scene**, and push it as soon as you're done.

## Art files are locked while you edit them

Aseprite, Photoshop, Krita, Affinity, and Blender files can't be merged. If two people edit the same one, someone's work is lost. So these files are **read-only until you lock them**:

```
git lfs lock "Hold the Hill/Assets/_Game/Art/player.aseprite"     # claim it
git lfs locks                                                      # see who has what
git lfs unlock "Hold the Hill/Assets/_Game/Art/player.aseprite"   # release it after you push
```

Only lock what you're actively editing, and unlock as soon as your change is pushed.
