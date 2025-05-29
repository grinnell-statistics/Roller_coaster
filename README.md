# Roller Coaster Calculus Unity Game
A Unity-based educational game designed to simulate the construction of a roller coaster using calculus concepts. Players plot mathematical functions to build roller coasters and see how well their designs perform based on slope, height, and continuity. Reference [Cornell Roller Coaster Design Assignment](https://pi.math.cornell.edu/~dmehrle/teaching/17sp/1110/handouts/1110sp17-project.pdf) for more information on what a roller coaster calculus project is usually like.

![Game scene](https://github.com/user-attachments/assets/2681c734-f7df-4ca9-b794-7ce8ec30d70e)

## Links

[Latest deployed version](https://khanhdo05.itch.io/roller-coaster-calculus)

[Offical Host Site](https://www.stat2games.sites.grinnell.edu/)

[Github Repo](https://github.com/khanhdo05/roller-coaster-unity-game)

[Fall 2024 Documentation by Shonda and Khanh](https://docs.google.com/document/d/17cIP_GmWjrH-xXtpC6U9SE5NScgmsykUsWWhpHLYSFI/edit?tab=t.0#heading=h.k0reosfw3h6h)

[Spring 2025 Documentation](https://docs.google.com/document/d/1xNkAyuReRjKl4d4yiy6XrlTbsDNIDTe2-_qNYi8RmhI/edit?tab=t.0)

## Contributors
- Professor [Shonda Kuiper](https://github.com/skuiper)
- Students:
  - [Khanh Do](https://github.com/khanhdo05)
  - [Minh Nguyen](https://github.com/minh-nguyen-mn)
  - [Dieu-Anh Trinh](https://github.com/audreydieuanh)
  - [Linh Vu](https://github.com/vchlinnn)

## Assets
- [Stylized Amusement Park / Roller Coaster](https://assetstore.unity.com/packages/3d/environments/stylized-amusement-park-roller-coaster-197863)
- [Swiss Army Spline](https://assetstore.unity.com/packages/tools/modeling/swiss-army-spline-176382)
- [Graph and Chart Lite Edition](https://assetstore.unity.com/packages/tools/gui/graph-and-chart-lite-edition-data-visualization-148497)
- [Allsky Free](https://assetstore.unity.com/packages/2d/textures-materials/sky/allsky-free-10-sky-skybox-set-146014)
- [Roller Coaster screaming sound](https://www.youtube.com/watch?v=_L4khv3hxq4)
- [Sounds (boo sound for Score scene if score == 0)](https://www.youtube.com/watch?v=CQeezCdF4mk)
- [Sounds (clap sound for Score scene if score > 0)](https://www.youtube.com/watch?v=VJ8FQSh-H4U)

## Getting Started

### Clone the repository

```sh
git clone https://github.com/khanhdo05/roller-coaster-unity-game.git
cd roller-coaster-unity-game
```

### Open in Unity

1. Install [Unity Hub](https://unity.com/download) if you haven't already.
2. Open Unity Hub and click on Open.
3. Select the cloned repository folder.
4. Ensure you are using Unity Editor version **2022.3.48f1**.

### Play the Game

1. Navigate to `Scenes/MenuScene`
2. Click `Play` to try the game

## Create a Build

1. Make sure all your open scenes are added to the build.
2. Go to `File -> Build Settings`.
3. Choose `WebGL`.
4. Click `Build`.

## Current Game Design

The game follows this structure:

- **Classes and Instances:**
  - `Function` class to represent mathematical functions.
  - `Segment` class to represent segments of the roller coaster.
  - `SegmentsManager` instance to manage and record all functions and segments.
  - `LevelConfig` objects to store level-specific parameters.

- **Flow of the Game:**
  1. **Menu Scene:** Users input their `PlayerID` and `GroupID`.
  2. **Choose Level Scene:** Users select between Level 1 or Level 2.
  3. **Graphing Scene:** Users have a playground to add and draw functions. The design is evaluated to ensure it satisfies all rules.
  4. **Segment Calculation:** The `SegmentsManager` calculates each point into a `Vector3` point.
  5. **Spline Rendering:** Using Swiss Army Spline, the roller coaster is drawn, and carts are animated.
  6. **Score Calculation:** The same `SegmentsManager` instance calculates the final score.

- **Reset Process:**
  - Upon replaying or restarting the game, the `SegmentsManager` instance will be destroyed and reset to ensure accurate recalculations.

## Test Cases

![Screenshot 2025-03-19 224519](https://github.com/user-attachments/assets/83ee5c84-ccd1-4177-91cd-1af96710238a)

![Screenshot 2025-03-24 095839](https://github.com/user-attachments/assets/704ed2cb-e91a-4f80-9e80-eb317f764528)

## How to Contribute

1. Either create a fork or branch out from `main`.
2. Make your changes.
3. Create a pull request to merge into `main`.
4. Write detailed PR description (what are the changes, why, what needs to be done next, etc.)
