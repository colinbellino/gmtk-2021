using Cysharp.Threading.Tasks;

namespace Game.Core.StateMachines.Game
{
	public class GameCreditsState : BaseGameState
	{
		public GameCreditsState(GameFSM fsm, GameSingleton game) : base(fsm, game) { }

		public override async UniTask Enter()
		{
			await base.Enter();

			await _ui.ShowCredits();

			_ui.CreditsButton1.onClick.AddListener(Restart);
			_ui.CreditsButton2.onClick.AddListener(Quit);
		}

		public override async UniTask Exit()
		{
			await base.Exit();

			_ui.CreditsButton1.onClick.RemoveListener(Restart);
			_ui.CreditsButton2.onClick.RemoveListener(Quit);

			await _ui.HideCredits();
		}

		private void Restart()
		{
			_state.CurrentLevelIndex = 0;

			_fsm.Fire(GameFSM.Triggers.Retry);
		}

		private void Quit()
		{
			_fsm.Fire(GameFSM.Triggers.Quit);
		}
	}
}
