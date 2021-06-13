using Cysharp.Threading.Tasks;

namespace Game.Core.StateMachines.Game
{
	public class GameScoreState : BaseGameState
	{
		public GameScoreState(GameFSM fsm, GameSingleton game) : base(fsm, game) { }

		public override async UniTask Enter()
		{
			await base.Enter();

			Continue();

			// _ui.SetDebugText("State: Victory");
			// await _ui.ShowVictory(_state.Scores[_state.CurrentLevelIndex].ToString());

			// _ui.VictoryButton1.onClick.AddListener(Continue);
			// _ui.VictoryButton2.onClick.AddListener(Restart);

			// _ = _audioPlayer.StopMusic(5f);
		}

		public override async UniTask Exit()
		{
			await base.Exit();

			await _ui.HideVictory();

			_ui.VictoryButton1.onClick.RemoveListener(Continue);
			_ui.VictoryButton2.onClick.RemoveListener(Restart);
		}

		private void Restart()
		{
			_fsm.Fire(GameFSM.Triggers.Retry);
		}

		private void Continue()
		{
			_state.CurrentLevelIndex += 1;

			_fsm.Fire(GameFSM.Triggers.NextLevel);
		}
	}
}
