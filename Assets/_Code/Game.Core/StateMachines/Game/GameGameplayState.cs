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
				Color = _config.LeaderColor,
				RecruitmentRadius = 2f,
				Sprite = _config.LeaderSprite,
				CanBeHit = true,
				Alliance = Alliances.Ally,
				ColliderType = 1,
				SortingOrder = 1,
			};
			_state.Entities.Add(leader);

			var followerSpawners = GameObject.FindObjectsOfType<FollowerSpawner>();
			for (int i = 0; i < followerSpawners.Length; i++)
			{
				var spawner = followerSpawners[i];
				var entity = new Entity
				{
					Name = "Follower " + i,
					Position = (Vector2)spawner.transform.position,
					Color = _config.LeaderColor,
					WillFollowerLeader = true,
					Sprite = _config.FollowerSprite,
					Alliance = Alliances.Ally,
					ColliderType = 1,
				};
				_state.Entities.Add(entity);

				GameObject.Destroy(spawner.gameObject);
			}

			var crateSpawners = GameObject.FindObjectsOfType<CrateSpawner>();
			for (int i = 0; i < crateSpawners.Length; i++)
			{
				var spawner = crateSpawners[i];
				var entity = new Entity
				{
					Name = "Crate " + i,
					Position = (Vector2)spawner.transform.position,
					Sprite = _config.CrateSprite,
					Static = true,
					CanBeHit = true,
					HealthCurrent = 5,
					HealthMax = 5,
					SortingOrder = 1,
				};
				_state.Entities.Add(entity);

				GameObject.Destroy(spawner.gameObject);
			}

			foreach (var entity in _state.Entities)
			{
				Rendering.SpawnEntity(entity, _config.EntityPrefab.GetComponent<EntityComponent>());
			}

			_followersFlock.FollowTarget = leader.Component.transform;
		}

		public override void Tick()
		{
			base.Tick();

			var moveInput = _controls.Gameplay.Move.ReadValue<Vector2>();
			var leader = _state.Entities.First(e => e.PlayerControlled);
			var followers = _state.Entities.Where(e => e.Flock == _followersFlock).ToList();

			_cameraRig.transform.position = leader.Component.transform.position;

			{
				var entity = leader;

				entity.Velocity = (float2)moveInput * Time.deltaTime * 1000f;

				if (_controls.Gameplay.Confirm.WasPerformedThisFrame() && Time.time > entity.AttackTimestamp)
				{
					entity.AttackTimestamp = Time.time + entity.AttackCooldown;
					entity.Component.Animator.Play("Attack");

					var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.HitRadius, LayerMask.GetMask("Entity"));
					foreach (var collider in colliders)
					{
						if (collider == entity.Component.Collider)
						{
							continue;
						}

						var otherComponent = collider.GetComponentInParent<EntityComponent>();
						var otherEntity = otherComponent.Entity;

						if (otherEntity.CanBeHit && otherEntity.Alliance != Alliances.Ally)
						{
							UnityEngine.Debug.Log("Hit => " + otherEntity.Name);
							var damage = 1;
							otherEntity.HealthCurrent = math.max(0, otherEntity.HealthCurrent - damage);


							if (otherEntity.HealthCurrent == 0)
							{
								otherEntity.Component.Animator.Play("Destroy");
								otherEntity.FlaggedForDestroy = true;
								otherEntity.DestroyTimestamp = Time.time + 0.5f;
							}
						}
					}
				}

				{
					var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.RecruitmentRadius, LayerMask.GetMask("Entity"));
					foreach (var collider in colliders)
					{
						if (collider == entity.Component.Collider)
						{
							continue;
						}

						var otherComponent = collider.GetComponentInParent<EntityComponent>();
						var otherEntity = otherComponent.Entity;

						if (otherEntity.WillFollowerLeader && otherEntity.Flock == null)
						{
							otherEntity.Flock = _followersFlock;
							otherEntity.CanBeHit = true;
							otherEntity.Component.Collider.gameObject.SetActive(false);
						}
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
