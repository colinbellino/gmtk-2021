using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using static Game.Core.Utils;

namespace Game.Core.StateMachines.Game
{
	public class GameInitState : BaseGameState
	{
		public GameInitState(GameFSM fsm, GameSingleton game) : base(fsm, game) { }

		public override async UniTask Enter()
		{
			await base.Enter();

			_audioPlayer.SetMusicVolume(_config.MusicVolume);
			_audioPlayer.SetSoundVolume(_config.SoundVolume);

			_state.InitialMusicVolume = _state.CurrentMusicVolume = _config.MusicVolume;
			_state.InitialSoundVolume = _state.CurrentSoundVolume = _config.SoundVolume;

			Time.timeScale = 1f;

			// Source: https://forum.unity.com/threads/transparency-sort-mode-and-lightweight-render-pipeline.651700/?_ga=2.33709793.1917298031.1610550622-170523125.1537693866
			GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
			GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 0.0f);

			if (IsDevBuild())
			{
				// _ui.ShowDebug();

				if (_config.LockFPS > 0)
				{
					Debug.Log($"Locking FPS to {_config.LockFPS}");
					Application.targetFrameRate = _config.LockFPS;
					QualitySettings.vSyncCount = 1;
				}
				else
				{
					Application.targetFrameRate = 999;
					QualitySettings.vSyncCount = 0;
				}
			}

			_fsm.Fire(GameFSM.Triggers.Done);
		}
	}
}
