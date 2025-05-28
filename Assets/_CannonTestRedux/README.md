# Cannon Test Redux

### Complex Games – Gameplay Programmer Test | Completed by Garret Browning

## Introduction

This project was created for the Gameplay Programmer test at Complex Games. It showcases clean, modular Unity development with a focus on maintainability and extensibility. The work follows industry best practices and was guided by the provided Game Design Document (GDD) and Technical Design Document (TDD) to reflect real-world production workflows.

## Project Structure

- **Assets/_CannonTestRedux/Scripts/**: Core gameplay scripts.
- **Assets/_CannonTestRedux/Prefabs/**: Prefabs for the player, bullets, targets, UI, and HUD elements.
- **Assets/_CannonTestRedux/Scenes/**: The main game scene.
- **Assets/_CannonTestRedux/Materials/**: Materials used for visual effects.

## Design Philosophy

This codebase follows a compositional design approach, where behaviors are implemented as small, focused components (MonoBehaviours) that can be flexibly attached to GameObjects. In line with the test’s guidelines, class names were intentionally styled as **verbs or adjectives** (e.g., `Damaging`, `Scorable`, `RespawningTargets`) to reflect what the component does or how it modifies behavior—an adaptation from my usual naming conventions that reinforced clarity and purpose.

This approach ensures:

- **Loose Coupling**: Components communicate via events and interfaces, reducing dependencies.
- **Strong Cohesion**: Each class has a clear, single responsibility.
- **Extensibility**: New features can be added by creating new components rather than modifying existing ones.
- **Readability & Maintainability**: The code is well-commented, consistently formatted, and uses descriptive naming throughout.

## Key Systems & Components

### Player System

- **RotatePlayer**: Handles player and arm cannon rotation, including pitch clamping for aiming.
- **Shooter**: Manages bullet firing using object pooling for performance.
- **SwitchingCameraViewpoint**: Enables toggling between first-person and third-person views.

### Bullet System

- **Damaging**: Applies damage on collision to objects implementing `IDamageable`.
- **SelfDisabling**: Automatically disables bullets after a set duration for pooling.
- **ObjectPooling**: Generic pooling system for bullets (and targets).

### Target System

- **Health**: Implements `IDamageable` for targets, tracks health and death state.
- **Scorable**: Awards points when a target is destroyed.
- **TargetLifecycleHandler**: Manages spawning, despawning, and event signaling for targets.
- **RespawningTargets**: Manages spawning and respawning targets, keeping a minimum distance between them in the designated area.

### Game Management

- **GameManager**: Coordinates the game loop, state transitions, scoring, and wave spawning.
- **UIManager**: Manages UI states (title, pre-game, gameplay, game over), updates HUD, and displays results.

## UI & Gameplay Flow

- Four UI states: Title, Pre-Game Countdown, Gameplay, Game Over.
- Countdown displays "3", "2", "1" (no "0").
- Gameplay tracks time, score, shots fired, and accuracy.
- Game Over screen reveals results in sequence and prompts the player to restart.

## Extensibility & Best Practices

- **Events & Interfaces**: Core systems interact via C# events and interfaces, making it easy to add new features (e.g., damage types, power-ups) without modifying existing code.
- **Inspector Tooltips**: `[Tooltip]` and `[Header]` attributes enhance inspector usability for designers.
- **Object Pooling**: Optimizes performance and memory usage for bullets and targets.
- **Compositional Behaviors**: New mechanics can be introduced as independent MonoBehaviours.

## Code Quality

- All scripts include XML summary comments and inline explanations for complex logic.
- Private fields are exposed to the Inspector using `[SerializeField]`, following Unity best practices.
- Naming conventions and formatting adhere to C# standards for clarity and professionalism.

## Conclusion

This project demonstrates my approach to game development in Unity, with an emphasis on modularity, maintainability, and clarity. It’s built to adapt smoothly as new features or changes arise, aligning with industry-quality development practices.

Thank you for the opportunity and for taking the time to review my work!