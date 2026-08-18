# XVI-MECHFRAME (Continued) - GitHub Copilot Instructions

## Mod Overview and Purpose
XVI-MECHFRAME (Continued) is a comprehensive mod for RimWorld, aimed at enhancing the battlefield experience by introducing manned mechanical combat armor frames, known as mechframes. These powerful units bring unique weaponry and abilities, offering strategic depth and protection for your colony. Players are encouraged to place this mod as forward in the mod list as possible to ensure compatibility.

## Key Features and Systems
- **Mechframe Assembly**: Automated construction via assembly bays connected to an assembly nexus, reducing manual crafting time.
- **Fuel System**: Mechframes operate on electricity and can be charged or have their batteries replaced.
- **Survivability**: Equipped with regenerating shields and non-regenerating armor panels that require repairs.
- **Weaponry**: Each mechframe comes with a model-specific heavy weapon.
- **Skill Set**: Special abilities for battlefield impact, with optional settings for no friendly fire.
- **Intelligent Combat**: Assisted combat mode allows autonomous fighting, reducing micromanagement.
- **Modular Upgrades**: Enhance mechframes with additional abilities through upgrades.
- **Unique Threats**: Mechframes attract mech crushers, advanced enemies with tailored weapons and tactics.
- **Built-in Compatibility**: Native support for Combat Extended.

## Coding Patterns and Conventions
- **C# Conventions**: Follow standard C# coding practices, ensuring readability and maintainability.
- **XML Management**: Leverage RimWorld's XML structure for defining game objects, incidents, and attributes.

## XML Integration
- **Defining Objects**: Utilize XML files to define various game elements such as DamageDef, ThingDef, and IncidentDef.
- **Path Utilization**: Access relevant files for modifications and extensions accurately, adhering to RimWorld's file structure.

## Harmony Patching
- **Harmony Usage**: Employ Harmony patches to modify game behavior without altering original game code.
- **Patch Types**: Use Prefix and Postfix methods for pre- and post-execution changes in game logic.
- - Examples include patches for work tables, projectile collisions, and faction adjustments.

## Suggestions for Copilot
1. **Code Generation**: Assist with writing C# hooks such as Harmony patches, ensuring they align with existing function signatures.
2. **XML Definition**: Aid in generating comprehensive XML defs for new game components and incident scenarios.
3. **Intelligent Autocomplete**: Use completion suggestions to follow established modding conventions and RimWorld-specific methodologies.
4. **Error Management**: Automatically suggest error-checking mechanisms based on Harmony usage and XML loading practices.
5. **Documentation Assistance**: Provide comments and documentation lines in both C# and XML files, enhancing code clarity.

## Additional Information
- **Potential for Expansion**: The mod's design allows for potential third-party add-ons; contact the author for integration guidelines.
- **Error Handling**: Community support available via Discord for troubleshooting, with suggestions to isolate issues by toggling other mods.

## Contact and Credits
- Leading Development Team: Project Lead and Programmer - 旋风, Artists - 青叶, Bill Doors, and others.
- For translation or contribution inquiries, please reach out to the authors. Proper credits will be attributed as applicable.


This `.github/copilot-instructions.md` file provides comprehensive information to facilitate the development and expansion of the XVI-MECHFRAME (Continued) mod, ensuring consistent code practices while leveraging the power of GitHub Copilot and Harmony.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

