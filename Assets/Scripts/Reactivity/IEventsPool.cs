#nullable enable
namespace Reactivity
{
    internal interface IEventsPool
    {
        EventWrapper Get();
    }
}