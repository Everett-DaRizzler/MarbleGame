# Development log

## 2026-08-16 — M1: Physics foundation

- Replaced scripted forward motion with Rigidbody-driven travel.
- Left/right arrows provide lateral steering only; slopes, gravity, momentum, friction, and collisions own forward/back motion.
- Rebuilt the camera to follow the marble's current travel direction.

## 2026-08-16 — M2: Glass sphere and store

- Added the large transparent glass sphere, machine framing, and an atmospheric generic grocery/big-box store environment.

## 2026-08-16 — M3: First playable level

- Built one compact six-section machine route with deceptive center-line traps, a banked momentum section, timed pressure gate, false exit, and finish trigger.
- Added fast automatic failure reset and an explicit restart key.

## 2026-08-16 — M4: Traversal and tuning

- Tuned track elevations, rails, supports, trigger placement, camera collision handling, readable signage, and restart feedback.
- Added generated rolling/impact audio and machine ambience.

## 2026-08-16 — M5: First-playable polish

- Added presentation materials, fog/lighting, telemetry UI, finish/failure overlays, and Windows build validation.
- Final editor build completed with zero compiler errors and zero warnings.

## 2026-08-16 — M6: Camera-facing presentation pass

- Reduced the generated sphere framing so the intake camera keeps the marble and puzzle path readable.
- Rebuilt the scene and Windows player; the final build completed with zero compiler errors and zero warnings.

## 2026-08-17 — M7: Traversal reliability

- Opened the first branch segment at the fork so overlapping rails cannot wedge a centered marble.
- Enlarged the glass sphere and safety radius to contain the full route, extended the final deck to the finish trigger, and removed collision from the decorative exit ring.
- A temporary guided physics probe reached the exit region while using the same gravity and lateral-force model as gameplay; the probe was removed after validation.
- The existing four smoke-test methods were executed directly and all four passed; Unity Test Framework result-file generation remains unavailable in batch mode.

## 2026-08-17 — M8: Runtime budget spot-check

- A temporary player-side frame probe measured the generated scene on the local RTX 3080 at 1.25 ms average frame time / 799.5 FPS over 5 seconds, with a 387.55 ms worst frame during startup and 626,688 bytes of reported GC delta.
- The probe was removed and the clean Windows player was rebuilt successfully.

## 2026-08-17 — M9: Runtime completion validation

- A temporary player-side traversal harness exercised the real moving gate, trigger, reset, and finish callback chain; it reached `GameDirector` completion with zero attempts.
- The harness was removed and the final clean Windows build completed with zero compiler errors and zero warnings.

## Status

The first-playable implementation is in place. Gravity-only release and arrow steering were observed in the standalone smoke pass; a guided physics validation reached the exit trigger with the final gate animated; all four existing smoke-test methods passed; camera readability, scene generation, runtime launch, and the final Windows build were also checked. Runtime FinishZone callback behavior, audio/performance budgets, and Unity Test Framework result-file generation still require a real PlayMode/manual pass because the standalone player's accessibility tree is unavailable and Unity batch test execution exits without producing a result file.
