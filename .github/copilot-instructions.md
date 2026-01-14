# XVI-MECHFRAME (Continued): GitHub Copilot Instructions

## Mod Overview and Purpose

XVI-MECHFRAME is a continuation and update of the original mod by andery233xjs, introducing a new kind of manned mechanical combat armor to RimWorld that significantly alters battlefield dynamics. These mechframes provide enhanced protection and offensive capabilities, boasting unique weapons and a diverse skill set aimed at providing superior defenses for your colony.

## Key Features and Systems

- **Assembly System**: Mechframes are automatically constructed and maintained via advanced assembly bays and nexuses, allowing colonists to focus on other tasks.
- **Electric Fuel System**: Mechframes operate on electricity and can be recharged at assembly bays or by swapping batteries, simulating a realistic power management strategy.
- **Survivability and Defense**: Mechframes are equipped with a regenerative shield and durable armor panels, both requiring strategic management to maintain.
- **Weaponry and Combat**: Each mechframe model comes with unique heavy weaponry, offering varied offensive strategies.
- **Special Skill Sets**: Different models have distinct skills and abilities that can be utilized on the battlefield, enhancing tactical options.
- **AI-Driven Combat**: Optional AI assists in combat, allowing mechframes to automatically engage enemies, reducing the need for micromanagement.
- **Modular Upgrades**: Provides options for customizations and enhancements, allowing for further extension via upgrades to address specific operational needs, like battery efficiency or enhanced armor.

## Coding Patterns and Conventions

- Follow C# best practices, including naming conventions and code organization.
- Utilize interfaces like `IChargingEquipment` and `IExposable` for consistent handling of equipment and data exposure.
- Incorporate design patterns such as the Factory Pattern for creating objects and the Singleton Pattern for shared instances.
- Avoid magic numbers; use constants and enums for readability and maintainability.

## XML Integration

- XML files are used to define mechframe properties, weapon configurations, and skill sets.
- Ensure XML is well-formed and utilizes proper schema-specified tags to define item properties and mech behavior.
- Use Def-based classes, such as `MechanicalArmorDef` and `ModuleDef`, to integrate XML definitions seamlessly with C# code logic.

## Harmony Patching

- Use Harmony to patch methods without altering the base game code, ensuring compatibility and flexibility.
- Commonly patched classes include `DamageWorker` and `IncidentWorker`, enhancing options for interactions and event handling.
- Apply prefix, postfix, and transpiler patches where necessary to modify game logic, such as `Pawn_AbilityTracker_GetGizmos`.

## Suggestions for Copilot

- When suggesting code, focus on enhancing or extending existing systems rather than rewriting core logic.
- Provide context-specific IDEs, such as helpful comments or explanations for complex patches and Harmony methods.
- Suggest test cases, particularly in areas where AI behaviors and mechframe combat simulations might introduce edge cases.
- Provide auto-completion suggestions based on prevalent coding patterns in other RimWorld mod projects.
- Emphasize modular coding practices to support potential third-party expansions, keeping future compatibility in mind.

## Conclusion

This document outlines guidelines to leverage GitHub Copilot effectively for the XVI-MECHFRAME mod project, ensuring quality, maintainability, and extensibility in the codebase. Following these instructions will aid in efficient code generation and error minimization.
