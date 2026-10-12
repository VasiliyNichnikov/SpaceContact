#if UNITY_EDITOR
namespace Editor
{
    public interface IGameRulesValidation
    {
        void Validate(GameRulesValidationContext context);
    }
}
#endif
