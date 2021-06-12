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
				Name = "Leader",
				Position = new float2(5, 5),
				PlayerControlled = true,
				Color = Color.red,
				RecruitmentRadius = 3f,
			};
			_state.Entities.Add(leader);

			for (int i = 0; i < 10; i++)
			{
				var entity = new Entity
				{
					Name = "Follower " + i,
					Position = UnityEngine.Random.insideUnitCircle * 100 * 0.08f,
					Color = Color.green,
					WillFollowerLeader = true,
				};
				_state.Entities.Add(entity);
			}

			foreach (var entity in _state.Entities)
			{
				Utils.SpawnEntity(entity, _config.EntityPrefab.GetComponent<EntityComponent>());
			}

			_followersFlock.FollowTarget = leader.Component.transform;
		}

		public override void Tick()
		{
			base.Tick();

			var moveInput = _controls.Gameplay.Move.ReadValue<Vector2>();
			var leader = _state.Entities.First(e => e.PlayerControlled);
			var followers = _state.Entities.Where(e => e.Flock == _followersFlock).ToList();

			{
				var entity = leader;

				entity.Velocity = (float2)moveInput * Time.deltaTime * entity.MoveSpeed;

				var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.RecruitmentRadius, LayerMask.GetMask("Entity"));
				foreach (var collider in colliders)
				{
					if (collider == entity.Component.Collider)
					{
						continue;
					}

					var otherComponent = collider.GetComponent<EntityComponent>();
					var otherEntity = otherComponent.Entity;

					if (otherEntity.WillFollowerLeader && otherEntity.Flock == null)
					{
						otherEntity.Flock = _followersFlock;
					}
				}
			}

			_followersFlock.Tick(followers);

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
