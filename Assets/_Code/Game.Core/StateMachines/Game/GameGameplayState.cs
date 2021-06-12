using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core.StateMachines.Game
{
	public class GameGameplayState : BaseGameState
	{
		public GameGameplayState(GameFSM fsm, GameSingleton game) : base(fsm, game) { }

		public override async UniTask Enter()
		{
			await base.Enter();

			_ui.ShowDebug();
			_controls.Gameplay.Enable();

			_state.Entities = new List<Entity>();
			var leader = new Entity
			{
				Position = new float2(5, 5),
				IsPlayerControlled = true,
			};
			_state.Entities.Add(leader);

			for (int i = 0; i < 100; i++)
			{
				var entity = new Entity
				{
					Position = UnityEngine.Random.insideUnitCircle * 100 * 0.08f,
					IsFollowingLeader = true,
				};
				_state.Entities.Add(entity);
			}

			foreach (var entity in _state.Entities)
			{
				Utils.SpawnEntity(entity, _config.EntityPrefab.GetComponent<EntityComponent>());
			}

			_flock.FollowTarget = leader.Component.transform;
		}

		public override void Tick()
		{
			base.Tick();

			var moveInput = _controls.Gameplay.Move.ReadValue<Vector2>();
			var leader = _state.Entities.First(e => e.IsPlayerControlled);
			var followers = _state.Entities.Where(e => e.IsFollowingLeader).ToList();

			leader.Velocity = (float2)moveInput * Time.deltaTime * leader.MoveSpeed;
			leader.Component.SpriteRenderer.material = GameObject.Instantiate(leader.Component.SpriteRenderer.material);
			leader.Component.SpriteRenderer.material.SetColor("ReplacementColor2", Color.blue);

			_flock.Tick(followers);
			Rendering.UpdateEntities(_state.Entities);

			if (Keyboard.current.f1Key.wasPressedThisFrame)
			{
				_fsm.Fire(GameFSM.Triggers.Won);
			}

			if (Keyboard.current.f2Key.wasPressedThisFrame)
			{
				_fsm.Fire(GameFSM.Triggers.Won);
			}
		}

		public override async UniTask Exit()
		{
			await base.Exit();

			_controls.Gameplay.Disable();
		}
	}
}
