
Unity AR App - 3D Model Viewer
This project is an AR application that lets users place 3D models on detected planes and control their animations via a world-space menu.

Instead of hardcoding behavior on specific GameObjects, I used a data-driven approach so adding new models or changing configuration doesn't require touching the code.

****** Features *****

AR Placement: Detect planes and tap to spawn models.

Dynamic Model Loading: Supports multiple unique models with different animation sets.

World Space UI: Each model has its own floating menu for control.

Animation Control:

Play/Stop

Change Speed (0.5x, 1x, 2x, 5x)

Play Mode (Once, Loop, Ping-Pong)

Reverse Playback

Movement: Drag models on XZ plane (Y-axis locked).

*** Architecture & Design  ****

The core of the project is built around ScriptableObjects to keep logic decoupled from data.

1. Data-Driven Models (ScriptableObject)
I didn't want to create a new prefab variant or Animator Controller for every single model I might add in the future.

ARModelData: This asset file holds the specific data for a model (Prefab, List of Animation Clips, Max Spawn Limit).

ModelLibrary: A registry that holds the list of all available ARModelData.

** Benefit: To add a new robot/character, I just create a new Data Asset, drag in the GLB/fbx and its clips, and the UI automatically updates to include it. No code changes needed.

2. The Animation System (Runtime Overrides)
Standard Animator Controllers are rigid. I needed the same script to handle a model with 1 clip or 10 clips without knowing their names beforehand.

ARObjectController: This is the main brain on the object.

AnimatorOverrideController: At runtime, the script takes a generic "Template" controller and injects the specific clips from the ARModelData into it.

Parameter Control: Speed and Reverse logic are handled via Animator Parameters (SpeedMult) rather than direct component manipulation, preventing Unity warnings and ensuring smooth reverse playback.

3. Manager/Service Pattern
ARSpawnManager: Handles the logic of "Who is selected?" and enforces spawn limits.

MainUIManager: dynamically generates the selection buttons at the bottom of the screen based on the ModelLibrary.

(Note: For basic Input handling and plane detection, I utilized Unity's AR Starter Assets to save time on boilerplate, but the Spawning and Animation logic is custom.)
