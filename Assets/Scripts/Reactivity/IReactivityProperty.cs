#nullable enable
namespace Reactivity
{
    public interface IReactivityProperty<out T> : IReactivityEvent
    {
        T? Value { get; }
    }
}