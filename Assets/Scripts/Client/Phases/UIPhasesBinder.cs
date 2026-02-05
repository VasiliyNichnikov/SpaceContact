using System;
using Client.Factory;
using Client.UI.Dialogs.Lobby;
using Core;
using Core.Phases;
using VContainer.Unity;

namespace Client.Phases
{
    public class UIPhasesBinder : IStartable, IDisposable, IPhaseVisitor
    {
        private readonly GameStateMachine _stateMachine;
        private readonly DialogsFactory _dialogsFactory;
        
        public UIPhasesBinder(GameStateMachine stateMachine, DialogsFactory dialogsFactory)
        {
            _stateMachine = stateMachine;
            _dialogsFactory = dialogsFactory;
            _stateMachine.OnPhaseChanged += PhaseChanged;
        }
        
        public void Start()
        {
            // нужен, чтобы класс попал в контейнер и не пропал, тк на него никто не ссылается
        }

        public void Dispose()
        {
            _stateMachine.OnPhaseChanged -= PhaseChanged;
        }

        private void PhaseChanged(IGamePhase phase)
        {
            phase.Accept(this);
        }

        void IPhaseVisitor.Visit(LobbyPhase phase)
        {
            _dialogsFactory.CreateDialog<LobbyDialog>();
        }

        void IPhaseVisitor.Visit(RegroupPhase phase)
        {
            throw new NotImplementedException();
        }
    }
}