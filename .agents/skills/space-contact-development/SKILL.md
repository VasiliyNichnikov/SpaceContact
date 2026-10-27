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
4. For GameAction validators and failure enums, follow section 1.1: nest the class named `Validator` and any failure enum inside the corresponding GameAction, rather than creating separate files.

Do not begin code edits until the style document has been read.

## Проверки состояния и ответственность классов

- Общие проверки жизненного цикла и готовности (`_disposed`, `_isInitialized` и аналогичные) выполняй в классе-координаторе, который управляет вызовами остальных классов.
- Если координатор уже гарантирует предусловие, не повторяй ту же проверку в каждом подчинённом классе и последовательном внутреннем вызове. Не добавляй ради этого дублирующие флаги состояния.
- Обособленный класс с самостоятельным жизненным циклом проверяет свои предусловия на собственных точках входа. Отдельный callback или продолжение после `await` также может требовать проверки, если гарантия координатора к этому моменту уже не действует.
- Добавляй проверку только там, где она защищает конкретную границу ответственности; не распространяй её автоматически по всей цепочке вызовов.

## After completing the task

1. Run only relevant existing tests, if such tests exist and can be run without invoking a prohibited tool.
2. Do not run `dotnet`, including `dotnet build`, `dotnet test`, or `dotnet format`, unless the user's prompt explicitly requests it.
3. Do not launch Unity, the Unity Editor, Unity batch mode, a Unity build, or Unity-based tests unless the user's prompt explicitly requests it.
4. If the available tests require `dotnet` or Unity, do not run them without the explicit request. Report this limitation in the final response.
5. Do not substitute an unrequested build or application launch for tests.

Perform non-executing checks such as reviewing the diff or inspecting files when useful. State which tests were run, or state that no tests were run and why.
