---
name: space-contact-development
description: Enforce the SpaceContact project workflow and C# conventions. Use for every implementation, bug fix, refactoring, code modification, or other development task in the SpaceContact repository.
---

# SpaceContact Development

Follow this workflow whenever developing or modifying code in the SpaceContact project.

## Before writing code

1. Read `docs/CSharpCodeStyle.md` completely before creating or changing any code.
2. Treat every rule in that document as mandatory for all new and modified C# code.
3. Re-read the document if its contents change during the task.

Do not begin code edits until the style document has been read.

## After completing the task

1. Run only relevant existing tests, if such tests exist and can be run without invoking a prohibited tool.
2. Do not run `dotnet`, including `dotnet build`, `dotnet test`, or `dotnet format`, unless the user's prompt explicitly requests it.
3. Do not launch Unity, the Unity Editor, Unity batch mode, a Unity build, or Unity-based tests unless the user's prompt explicitly requests it.
4. If the available tests require `dotnet` or Unity, do not run them without the explicit request. Report this limitation in the final response.
5. Do not substitute an unrequested build or application launch for tests.

Perform non-executing checks such as reviewing the diff or inspecting files when useful. State which tests were run, or state that no tests were run and why.
