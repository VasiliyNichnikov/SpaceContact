## Где размещать атрибуты и когда они работают

Ограничения объявляются непосредственно на **полях и свойствах snapshots и вложенных DTO**. На контекстах и runtime-классах `*Data` они не дублируются. `StateRule` размещается на классе или структуре DTO.

Сам атрибут не проверяет присваивание полю и не запускает проверку из конструктора. В проекте есть два места применения:

1. `NewtonsoftSerializer` проверяет присутствие публичных обязательных JSON-полей через `StateValidationContractResolver`.
2. `GameContext.LoadSnapshot` вызывает `StateValidator.Validate` до подготовки и установки новых данных: проверяются значения, аудитория, вложенные коллекции, ссылки и правила DTO.

Для отдельного вызова проверки полного snapshot:

```csharp
StateValidator.Validate(snapshot, GameContextRole.Client, localPlayerId, revision);
```

Этот вызов проверяет данные, но не устанавливает их в контекст. Проверка меньшей revision относительно уже загруженного состояния находится в `GameContext.LoadSnapshot`, поскольку автономный валидатор не знает текущую revision контекста.

Проверки входного snapshot выполняются во всех сборках. Условие `UNITY_EDITOR || DEBUG` относится только к дополнительному сравнению восстановленной проекции в C05, а не к атрибутам валидации.

## Быстрый выбор

| Задача | Разметка |
| --- | --- |
| Запретить отсутствие значения | `[StateRequired]` |
| Требовать непустую строку или коллекцию | `[StateRequired]` и `[StateNotEmpty]` |
| Проверить неотрицательный int | `[StateRange(0, int.MaxValue)]` |
| Проверить число в диапазоне включительно | `[StateRange(0, 1)]` |
| Принимать только объявленные значения enum | `[StateDefinedEnum]` |
| Запретить повторение элементов списка | `[StateUnique]` |
| Запретить повторение конкретного поля элементов | `[StateUnique(nameof(GamePlayerSnapshot.Order))]` |
| Зарегистрировать уникальный ID сущности | `[StateIdentity(typeof(GamePlayerSnapshot))]` |
| Проверить ссылку на сущность | `[StateReference(typeof(GamePlayerSnapshot))]` |
| Требовать по записи на каждого игрока | `[StateReference(typeof(GamePlayerSnapshot), MatchAllIdentities = true)]` |
| Проверить зависимость нескольких полей | `[StateRule(typeof(ИмяКлассаПравила))]` на типе DTO |

Атрибуты комбинируются. Например, уникальность списка сама по себе не запрещает пустой список или null. Проверки одного поля должны быть независимыми: не рассчитывайте на порядок выполнения нескольких атрибутов.

## Обязательность и видимость

### `StateRequired`

[Исходный код](../Assets/Scripts/Core/Game/Validation/StateRequiredAttribute.cs).

Проверяет, что доступное получателю значение не равно null. Ноль, false, пустая строка и пустая коллекция сами по себе не являются нарушением. Для непустого значения нужен `StateNotEmpty`, для допустимого числа — `StateRange`.

Пример из модели руки:

```csharp
[JsonProperty]
[StateVisibility(StateAudience.Owner)]
[StateRequired]
public List<SpaceCardSnapshot>? Cards;
```

Nullable здесь нужен: у чужой руки Cards отсутствует законно. Обязательность определяется одновременно ролью контекста и аудиторией поля:

| Аудитория | Server context | Client context владельца | Client context другого игрока |
| --- | --- | --- | --- |
| `Everyone` | Требуется значение | Требуется значение | Требуется значение |
| `Owner` | Требуется значение | Требуется значение | Значение должно быть null |
| `ServerOnly` | Требуется значение | Значение должно быть null | Значение должно быть null |

Таблица относится к полям с `StateRequired`. У видимого поля без него null допускается. У скрытого поля наличие ненулевого значения отклоняется независимо от `StateRequired`; остальные проверки поля и обход его содержимого пропускаются.

Владелец берётся из ключа `Dictionary<ulong, ...>` или члена с `IsOwnerId = true`. Если владелец не установлен, поле `Owner` у клиента скрыто. Проверка не использует локального игрока сервера как владельца всех данных.

### Отсутствующее поле в JSON и null — разные случаи

Пример:

```csharp
[StateVisibility(StateAudience.Everyone)]
[StateRequired]
public List<SpaceCardSnapshot> SpaceDiscard = new();
```

После обычной десериализации без специальных настроек пропущенный `SpaceDiscard` мог бы выглядеть как корректный пустой список. Проектный resolver предотвращает это: из `StateRequired` выводится `Required.Always` для `Everyone` и для полей без аудитории, например Snapshot и Revision в `GameSnapshotResponseDto`. Отсутствующий ключ и явный null отклоняются при десериализации.

Для `Owner`/`ServerOnly` resolver разрешает отсутствие: роль и получатель известны при загрузке контекста. Поэтому скрываемые поля должны сохранять отсутствие значения, как nullable Cards и колоды без инициализатора `new()`.

Используйте зарегистрированный `NewtonsoftSerializer`. Если создаёте другой путь десериализации, подключайте [StateValidationContractResolver](../Assets/Scripts/Core/Game/Validation/StateValidationContractResolver.cs). Не добавляйте отдельные независимые правила `JsonProperty(Required = ...)` для тех же ограничений.

В программно созданном объекте нельзя отличить неприсвоенный `int` от явно присвоенного нуля. Атрибут проверяет текущее значение; факт отсутствия скалярного JSON-поля выявляет resolver. Прямой `JsonConvert.DeserializeObject` без этого resolver такой гарантии не даёт.

## Проверки значений

### `StateRange(minimum, maximum)`

[Исходный код](../Assets/Scripts/Core/Game/Validation/StateRangeAttribute.cs).

Обе границы обязательны и включены. Для диапазона от нуля до максимума типа явно укажите `int.MaxValue`, `long.MaxValue` или `double.MaxValue` в зависимости от типа поля. Значение должно быть конечным: NaN и обе бесконечности отклоняются. Null пропускается, поэтому атрибут подходит для опциональных дедлайнов:

```csharp
[StateVisibility(StateAudience.Everyone)]
[StateRange(0, double.MaxValue)]
public double? EndTimeInSeconds;
```

Применяйте к числовым полям. Проверка использует `Convert.ToDouble` с invariant culture: она не предназначена для точного сравнения больших целых и decimal, если преобразование в double теряет значащие разряды. Для точного отношения двух revision используется отдельное правило с непосредственным сравнением `long`.

### `StateDefinedEnum`

[Исходный код](../Assets/Scripts/Core/Game/Validation/StateDefinedEnumAttribute.cs).

Проверяет `Enum.IsDefined`. Null у nullable enum пропускается. Значение вроде `(GamePhaseType)999` отклоняется, а объявленное `GamePhaseType.None` допустимо: это проверка состава enum, а не бизнес-правил перехода между фазами.

```csharp
[StateVisibility(StateAudience.Everyone)]
[StateRequired]
[StateDefinedEnum]
public GamePhaseType CurrentPhase = GamePhaseType.None;
```

Для `[Flags]` комбинация флагов проходит только тогда, когда она сама объявлена отдельным значением enum. Проверки произвольной допустимой битовой маски этот атрибут не выполняет.

### `StateNotEmpty`

[Исходный код](../Assets/Scripts/Core/Game/Validation/StateNotEmptyAttribute.cs).

Для строки запрещает пустое значение и строку из пробелов. Для `System.Collections.ICollection` требует `Count > 0`. Null пропускается; для обязательной коллекции добавляйте `StateRequired`.

```csharp
[StateVisibility(StateAudience.Everyone)]
[StateRequired]
[StateNotEmpty]
public List<GamePlayerSnapshot> Players = new();
```

Атрибут не задаёт максимальный размер и не проверяет содержимое элементов. Произвольный `IEnumerable`, не реализующий `ICollection`, не является поддерживаемой непустой коллекцией для этого атрибута.

## Уникальность и ссылки

### `StateUnique(memberName = null)`

[Исходный код](../Assets/Scripts/Core/Game/Validation/StateUniqueAttribute.cs).

Без параметра проверяет уникальность элементов коллекции. С параметром проверяет уникальность значения указанного поля/свойства каждого элемента:

```csharp
[StateVisibility(StateAudience.Everyone)]
[StateRequired]
[StateNotEmpty]
[StateUnique(nameof(GamePlayerSnapshot.Order))]
public List<GamePlayerSnapshot> Players = new();
```

Используйте `nameof` непосредственного члена элемента. Это имя C#-члена, а не его псевдоним `JsonProperty`. Пути вроде `Player.Profile.Id` не поддерживаются.

Проверка использует `HashSet<object>` и обычную семантику `Equals`/`GetHashCode`. Она не выполняет глубокое сравнение объектов. Null у самой коллекции допускается, но null у элемента или выбранного ключа отклоняется. Пустая коллекция допустима. Для списков ID используйте вариант без параметра.

### `StateIdentity(entityType)`

[Исходный код](../Assets/Scripts/Core/Game/Validation/StateIdentityAttribute.cs).

Объявляет, что значение поля — ID сущности указанного типа. Уникальность проверяется во всём проверяемом графе, а не только в одном списке или у одного владельца:

```csharp
[StateVisibility(StateAudience.Everyone, IsOwnerId = true)]
[StateRequired]
[StateIdentity(typeof(GamePlayerSnapshot))]
public ulong PlayerId;
```

`entityType` служит ключом группы идентификаторов. Само наличие объекта этого типа в snapshot ничего не регистрирует: нужен атрибут на его ID. Одинаковые значения у разных типов сущностей допустимы; одинаковый ShipId у разных игроков — нет.

Атрибут не проверяет положительность ID и допускает null без регистрации. Например, для ID планеты дополнительно используются `StateRequired` и `StateRange(0, int.MaxValue)`. Значение 0 разрешено, в том числе для host player ID.

### `StateReference(entityType)`

[Исходный код](../Assets/Scripts/Core/Game/Validation/StateReferenceAttribute.cs).

Проверяет существование ID, ранее объявленных через `StateIdentity` того же типа. Фактическое разрешение ссылок отложено до завершения обхода, поэтому порядок разделов snapshot не важен.

| Значение поля | Что проверяется |
| --- | --- |
| Скаляр | Одна ссылка |
| Строка | Одна ссылка; строка не разбивается на символы |
| Коллекция ID | Каждый элемент |
| Словарь | Каждый ключ; значения словаря обходятся общим валидатором отдельно |

Пример опциональной ссылки:

```csharp
[StateVisibility(StateAudience.Everyone)]
[StateReference(typeof(GamePlayerSnapshot))]
public ulong? DefenderPlayerId;
```

Null у поля допустим, null среди ссылок коллекции — нет. Значения ID сравниваются как объекты без преобразования числовых типов: `int` и `ulong` с одинаковым числом не являются одной ссылкой. Тип ID должен совпадать с типом поля `StateIdentity`.

`StateReference` не запрещает дубликаты в списке и не проверяет владение сущностью. Для отсутствия повторов добавляйте `StateUnique`; для дополнительных отношений используйте отдельное правило.

### `MatchAllIdentities = true`

Требует, чтобы множество ссылок совпадало со всеми зарегистрированными ID выбранного типа:

```csharp
[StateVisibility(StateAudience.Everyone)]
[StateRequired]
[StateReference(typeof(GamePlayerSnapshot), MatchAllIdentities = true)]
public Dictionary<ulong, PlayerHandSnapshot> HandsByPlayerId = new();
```

Так у каждого участника должна быть запись руки, а неизвестных владельцев быть не должно. Аналогично размечены словари планет и кораблей по игрокам.

Параметр действует для коллекций и словарей; для скалярного значения дополнительная проверка множества не выполняется. Сравнение множеств не проверяет количество повторов: для списка при необходимости нужен `StateUnique`.

## Сложные ограничения: `StateRule(ruleType)`

[Атрибут](../Assets/Scripts/Core/Game/Validation/StateRuleAttribute.cs), [общий интерфейс](../Assets/Scripts/Core/Game/Validation/IStateValidationRule.cs), [типизированная база](../Assets/Scripts/Core/Game/Validation/StateValidationRule.cs).

Используйте для связей нескольких полей или структурных правил, которые неудобно выражать независимыми атрибутами. На DTO можно указать несколько `StateRule`.

Пример существующего правила, файл `PhaseRevisionRule.cs`:

```csharp
using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class PhaseRevisionRule : StateValidationRule<PhaseSnapshot>
    {
        protected override void Validate(PhaseSnapshot value, StateValidationContext context)
        {
            context.At(".Revision").Require(value.Revision <= context.Revision,
                "Phase revision exceeds snapshot revision.");
        }
    }
}
```

Подключение на DTO:

```csharp
[StateRule(typeof(PhaseRevisionRule))]
public sealed class PhaseSnapshot
{
    // Поля snapshot и их атрибуты.
}
```

Экземпляры правил создаются через `Activator.CreateInstance` и кешируются по типу DTO. Класс должен реализовывать `IStateValidationRule`, быть неабстрактным и иметь публичный конструктор без параметров. Базовый `StateValidationRule<T>` предоставляет типизированный метод; T должен соответствовать DTO, на котором указан атрибут. DI для этих правил не используется.

Правило не должно хранить данные текущего вызова в своих полях, изменять snapshot или контекст игры. Данные проверки находятся в аргументах и локальных переменных. Конструктор не выполняет валидацию. Правила запускаются после общего обхода, когда ID уже собраны; не рассчитывайте, что другое отложенное правило обязательно выполнено раньше.

Существующие специализированные правила:

| Класс | Назначение |
| --- | --- |
| `SpaceCardShapeRule` | Ровно одна форма Space card. |
| `DestinyCardShapeRule` | Ровно одна форма Destiny card. |
| `PlayerHandShapeRule` | Объявленный Count согласован с известным составом руки. |
| `PhaseRevisionRule` | Revision фазы не превышает revision snapshot. |
| `LocalPlayerSnapshotRule` | Получатель клиентского snapshot входит в состав участников. |
| `ShipPlacementRule` | Корабль не размещён на нескольких планетах. |
| `EncounterDependenciesRule` | Согласованы зависимости aggressor/defender/цели/союзов; союзы не пересекаются. |

Эти правила не заменяют проверки допустимости GameAction: разрешение действия, фаза хода и игровые механики проверяются отдельно.

## Новый вид атрибута

Для повторяемого ограничения одного поля создайте отдельный класс-наследник [StateValidationAttribute](../Assets/Scripts/Core/Game/Validation/StateValidationAttribute.cs) и реализуйте:

```csharp
public override void Validate(object? value, StateValidationContext context)
```

Общий обход автоматически найдёт новый атрибут; добавлять его имя в `StateValidator` или центральный switch не требуется.

Соглашения для реализации:

- Проверка null согласуется с обязательностью: обычный атрибут пропускает null, а `StateRequired` отвечает за его запрет.
- Конструктор принимает параметры ограничения и сохраняет их; проверка значений выполняется в `Validate`.
- Несоответствие данных сообщается через `context.Require`. Не включайте содержимое закрытых данных в сообщение.
- Не изменяйте объект, коллекцию или поля экземпляра атрибута во время проверки: метаданные кешируются.
- Добавьте положительный и отрицательный сценарии в проверки, а затем опишите новое ограничение в этом документе.

### Возможности `StateValidationContext`

| Член | Назначение |
| --- | --- |
| `Path` | Путь до текущего объекта или поля. |
| `Role`, `LocalPlayerId`, `OwnerPlayerId` | Роль контекста, локальный игрок и владелец текущей ветви. |
| `Revision` | Revision проверяемого snapshot. |
| `At(".Field")` / `At("[0]")` | Новый контекст с уточнённым путём ошибки. |
| `Require(condition, message)` | При false выбрасывает `StateValidationException`. |
| `RegisterIdentity(type, id)` | Регистрирует уникальный ID. |
| `RequireReference(type, id)` | Планирует проверку существования ID после обхода. |
| `RequireAllIdentities(type, ids)` | Планирует проверку совпадения множеств ID. |

Пример пути: `$.Cards.HandsByPlayerId[0].Count`. Валидатор останавливается на первой ошибке; он не собирает полный список ошибок. Ошибки JSON-присутствия приходят от Newtonsoft.Json при десериализации. Ошибки настройки самой схемы, например неверный класс правила, могут быть обычными исключениями конфигурации, а не `StateValidationException`.

## Порядок добавления поля

1. Добавьте поле в snapshot/DTO и укажите `StateVisibility`.
2. Определите, допустим ли null у видимого поля. Если нет — добавьте `StateRequired`.
3. Добавьте ограничения значения и коллекции. Для ссылок укажите тип сущности; проверьте наличие `StateIdentity` у её ID.
4. Если есть связь нескольких полей, подключите отдельное правило через `StateRule`.
5. Проверьте загрузку полного server snapshot и клиентских представлений владельца/другого игрока, включая пустые и скрытые значения.
6. Для обязательного сетевого поля проверьте удаление ключа из JSON, а не только присваивание null объекту.

`StateVisibility` требуется для каждого сериализуемого члена графа, который обходит валидатор. `[JsonIgnore]`-члены исключены. Особый случай — поля сетевой оболочки `GameSnapshotResponseDto`: `StateRequired` используется resolver без аудитории, сама оболочка не передаётся в общий обход.

Текущий контракт схемы поддерживает скаляры, nullable, вложенные объекты, `List<T>` и `Dictionary<ulong, T>`. Ключ словаря одновременно задаёт владельца значения. Для новых видов коллекций сначала требуется поддержка в общей схеме/проекции. Элементы списков и значения словарей текущей модели не могут быть null. Циклы запрещены.

## Проверки и сборки

В Unity доступны команды **Tools → Space Contact**:

- **Validate Game Rules** — в том числе схема видимости и создание подключённых правил валидации; эта проверка выполняется также перед сборкой. Она не заменяет проверку конкретных значений и всех сочетаний атрибутов.
- **Check State Projection** — представления данных для разных получателей.
- **Check Snapshot Validation** — некорректные значения, ссылки, JSON-поля, роли, циклы и сохранность контекста при ошибке.

Для IL2CPP/WebGL необходимо сохранить используемые через reflection типы и конструкторы. В проекте это задаёт [Visibility/link.xml](../Assets/Scripts/Core/Game/Visibility/link.xml). Новый класс правил в другом namespace/assembly нужно учесть в настройках сохранения. Проверка в редакторе не подтверждает выполнение в WebGL: при приёмке требуется загрузка snapshots в собранном приложении. Текущая реализация не была проверена агентом запуском Unity или WebGL.
