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

			var copSpawners = GameObject.FindObjectsOfType<CopSpawner>();
			for (int i = 0; i < copSpawners.Length; i++)
			{
				var spawner = copSpawners[i];
				var entity = new Entity
				{
					Name = "Cop " + i,
					Position = (Vector2)spawner.transform.position,
					Color = _config.CopColor,
					Sprite = _config.CopSprite,
					Alliance = Alliances.Foe,
					ColliderType = 1,
					ShootOnSight = true,
					AttackRadius = 3,
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
					HealthCurrent = 4,
					HealthMax = 4,
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

			if (Keyboard.current.f1Key.wasPressedThisFrame)
			{
				_fsm.Fire(GameFSM.Triggers.Won);
			}

			if (Keyboard.current.f2Key.wasPressedThisFrame)
			{
				_fsm.Fire(GameFSM.Triggers.Won);
			}

			// Player attack
			if (_controls.Gameplay.Confirm.WasPerformedThisFrame())
			{
				var leader = _state.Entities.First(e => e.PlayerControlled);
				var followers = _state.Entities.Where(e => e.Flock == _followersFlock).ToList();

				var attackers = new List<Entity>(followers);
				attackers.Add(leader);

				foreach (var entity in attackers)
				{
					if (Time.time <= entity.AttackTimestamp)
					{
						continue;
					}

					var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.AttackRadius, LayerMask.GetMask("Entity"));
					var targetsCount = 0;
					foreach (var collider in colliders)
					{
						if (collider == entity.Component.Collider)
						{
							continue;
						}

						var otherComponent = collider.GetComponentInParent<EntityComponent>();
						var otherEntity = otherComponent.Entity;

						if (otherEntity.CanBeHit && otherEntity.Alliance != entity.Alliance)
						{
							UnityEngine.Debug.Log(entity.Name + " => " + otherEntity.Name);
							var damage = 1;
							otherEntity.HealthCurrent = math.max(0, otherEntity.HealthCurrent - damage);

							if (otherEntity.HealthCurrent == 0)
							{
								otherEntity.Component.Animator.Play("Destroy");
								otherEntity.FlaggedForDestroy = true;
								otherEntity.DestroyTimestamp = Time.time + 0.5f;
								// otherEntity.CanBeHit = false;
							}

							targetsCount += 1;
						}
					}

					if (targetsCount > 0)
					{
						entity.AttackTimestamp = Time.time + entity.AttackCooldown;
						entity.Component.Animator.Play("Attack");
					}
				}
			}

			for (int entityIndex = 0; entityIndex < _state.Entities.Count; entityIndex++)
			{
				Entity entity = _state.Entities[entityIndex];

				if (entity.PlayerControlled)
				{
					// Movement
					entity.Velocity = (float2)moveInput * Time.deltaTime * 1000f;

					// Camera
					_cameraRig.transform.position = entity.Component.transform.position;

					// Recruitment
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

				if (entity.ShootOnSight)
				{
					Shoot(entity);
				}

				if (entity.Flock != null)
				{
					entity.Flock.Tick(entity);
				}

				if (entity.ProjectileMovement)
				{

				}
			}

			Rendering.UpdateEntities(_state.Entities);
		}

		private void Shoot(Entity entity)
		{
			if (Time.time <= entity.AttackTimestamp)
			{
				return;
			}

			var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.AttackRadius, LayerMask.GetMask("Entity"));
			var targetsCount = 0;
			foreach (var collider in colliders)
			{
				if (collider == entity.Component.Collider)
				{
					continue;
				}

				var otherComponent = collider.GetComponentInParent<EntityComponent>();
				var otherEntity = otherComponent.Entity;

				if (otherEntity.CanBeHit && otherEntity.Alliance != entity.Alliance)
				{
					UnityEngine.Debug.Log(entity.Name + " SHOOT => " + otherEntity.Name);
					// var damage = 1;
					// otherEntity.HealthCurrent = math.max(0, otherEntity.HealthCurrent - damage);

					// if (otherEntity.HealthCurrent == 0)
					// {
					// 	otherEntity.Component.Animator.Play("Destroy");
					// 	otherEntity.FlaggedForDestroy = true;
					// 	otherEntity.DestroyTimestamp = Time.time + 0.5f;
					// 	// otherEntity.CanBeHit = false;
					// }

					var direction = entity.Position - otherEntity.Position;

					var projectile = new Entity
					{
						Name = "Projectile",
						Position = entity.Position,
						Color = entity.Color,
						Sprite = _config.ProjectileSprite,
						Alliance = entity.Alliance,
						ColliderType = 1,
						SortingOrder = 1,
						AttackRadius = 0.2f,
						AttackOnCollision = true,
						ProjectileMovement = true,
						Velocity = new float2(0, 1) * 10,
						Direction = direction,
					};
					Rendering.SpawnEntity(projectile, _config.EntityPrefab.GetComponent<EntityComponent>());
					_state.Entities.Add(projectile);

					targetsCount += 1;

					break;
				}
			}

			if (targetsCount > 0)
			{
				entity.AttackTimestamp = Time.time + entity.AttackCooldown;
				entity.Component.Animator.Play("Attack");
			}
		}

		public override async UniTask Exit()
		{
			await base.Exit();

			_controls.Gameplay.Disable();
		}
	}
}
