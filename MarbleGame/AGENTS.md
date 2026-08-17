# MarbleGame Development Guide

## Project

- Use Unity `6000.5.8f1` (Unity 6.0.5) unless explicitly approved otherwise.
- Rendering uses URP `17.5.0` and the new Input System.
- Keep `Assets/`, `Packages/`, `ProjectSettings/`, and all `.meta` files under version control.

## Architecture

- Keep gameplay code under `Assets/Scripts/`, organized by feature (`Puzzle`, `Marble`, `Level`, `UI`, etc.).
- Separate pure puzzle/state rules from MonoBehaviours and presentation so rules can be tested without a scene.
- Prefer small, single-purpose components and explicit dependencies; avoid global singletons unless there is a clear project-wide service.
- Use ScriptableObjects for reusable, designer-authored configuration or level data; use prefabs for reusable scene objects.
- Use assembly definitions when runtime/editor/test code becomes substantial. Keep editor-only code in editor assemblies.

## Coding conventions

- C#: PascalCase for types, methods, and public members; camelCase for private fields and parameters.
- Use serialized private fields with `[SerializeField]` instead of public mutable fields.
- Prefer composition over inheritance and avoid per-frame polling when events or explicit state transitions are sufficient.
- Keep gameplay deterministic where practical; do not put puzzle rules in visual-only scripts.
- Add concise comments only where intent or non-obvious Unity behavior needs explanation.

## Testing

- Add EditMode tests for puzzle rules, state transitions, win/fail conditions, reset, and undo behavior.
- Add focused PlayMode tests for scene wiring, input integration, physics interactions, and essential presentation behavior.
- Run relevant tests and confirm there are no new Console errors before committing.
- Do not treat a successful editor import as a substitute for tests or a verified build.

## Git practices

- Use focused commits with imperative messages; do not commit generated folders, builds, logs, or local editor settings.
- Preserve `.meta` files and commit them together with their assets.
- Do not hand-edit serialized Unity YAML unless there is no safer editor or scripted alternative.
- Review `git diff` and `git status` before committing. Never discard unrelated user changes.
- Establish Git LFS rules before adding large binary models, textures, audio, or video.

## Unity asset rules

- Create and modify scenes, prefabs, materials, ScriptableObjects, input actions, and render settings through Unity or purpose-built editor scripts when possible.
- Keep reusable content in appropriately named folders and prefer prefabs over duplicated scene objects.
- Use stable, descriptive names; avoid leaving starter content such as `SampleScene` or generic `New ...` assets in production paths.
- Do not delete, regenerate, or reimport assets broadly without confirming the exact scope and checking dependent references.
- After asset changes, verify references, serialization, and the affected scene(s) in the Unity Editor.
