# Escape Game: Île de la Cité

A location-based escape game for Android and iOS, made with Unity.

You walk around Île de la Cité in Paris. Your real GPS position appears on a map. You solve three puzzles. After each puzzle, you go back to the map and move on to the next riddle.

<!-- TODO: add one or two screenshots, e.g. <img src="screenshots/map.png" width="300"> -->

## The game

The main scene shows a map of Île de la Cité and a marker for your position. Three buttons open the puzzles.

| Puzzle | Scene | How it works |
| --- | --- | --- |
| **Taquin** | `TaquinScene` | A sliding-tile puzzle on a 4×4 grid. The tiles are shuffled. Slide them back into the right order. |
| **Zip** | `ZipScene` | A 6×6 grid with the numbers 1 to 10. Draw one path through the numbers in order. Fill every cell and end on 10. Click the 1 to restart. Click the last cell to undo. |
| **Connections** | `ConnectionsScene` | A grid of 16 words. Find the 4 groups of 4 related words. Select 4 words, then press Submit. |

When you win a puzzle, the game shows a message ("Bien joué !") and returns to the map after 2 seconds. The game text is in French.

## How the GPS works

`GpsToUnity` reads the phone's GPS and moves the marker on the map.

- It converts latitude and longitude into meters around a reference point (the default is the centre of Paris), then into Unity units.
- It ignores readings with an accuracy worse than 15 m.
- It smooths the movement so the marker does not jump.
- The GPS only works on a phone (Android or iOS). On a computer, the game shows "GPS non dispo sur PC".

You can set the reference point, the scale and the map rotation in the Inspector, on the `GPSManager` object of `SampleScene`.

## Open the project

- **Unity version:** 6000.3.3f1 (Unity 6.3). Install it with Unity Hub.
- In Unity Hub, click **Add → Add project from disk**.
- Select the root folder of this repository (the one that contains `Assets`, `Packages` and `ProjectSettings`).
- Open `Assets/Scenes/SampleScene.unity` and press Play.

## Build for Android

- Open **File → Build Profiles**.
- Select **Android** and click **Switch Platform**.
- Check that the four scenes are in the scene list. They are already registered: `SampleScene`, `ConnectionsScene`, `TaquinScene` and `ZipScene`.
- Connect your phone and click **Build And Run**.
- Allow location access on the phone when asked.

## Project structure

```text
Assets/
  Scenes/       The 4 game scenes
  Scripting/    Game scripts (GPS, puzzles, scene navigation)
  Maps/         Map images
  Prefabs/      UI prefabs used by the puzzles
  taquin_tiles/ The cut-out image tiles for the Taquin puzzle
Packages/       Unity package list
ProjectSettings/
archive/        Unfinished work: GPS proximity detection (not imported by Unity)
webapp-reference/  An earlier web version (React and TypeScript), kept for comparison
```

## Work in progress

- **Proximity detection.** The idea is to trigger a question when the player is within 30 m of a place. An early version is in `archive/`.
- **Web version.** `webapp-reference/` is not part of the Unity build. It has its own `package.json`.

## Authors

- Camille Bernard
- Maissa Frikha
- Faustine Delorme

<!-- TODO: add each person's role, and a license if you want one. -->
