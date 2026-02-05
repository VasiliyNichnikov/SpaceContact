using Core.Player;
using VContainer;
using VContainer.Unity;

namespace App.Player
{
    public class PlayerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Чтобы PlayerManager создавался на одного игрока - Scoped
            builder.Register<PlayerManager>(Lifetime.Scoped).AsImplementedInterfaces();
        }
    }
}