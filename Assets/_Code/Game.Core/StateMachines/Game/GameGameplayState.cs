using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game.Core.StateMachines.Game
{
	public class GameGameplayState : BaseGameState
	{
		private Transform _levelsContainer;
		private bool _isTransitioning;
		private Tilemap _fogTilemap;

		public GameGameplayState(GameFSM fsm, GameSingleton game) : base(fsm, game) { }

		public override async UniTask Enter()
		{
			await base.Enter();

			_isTransitioning = false;
			if (_levelsContainer == null)
			{
				_levelsContainer = GameObject.Find("Levels").transform;
			}
			for (int levelIndex = 0; levelIndex < _levelsContainer.childCount; levelIndex++)
			{
				var level = _levelsContainer.GetChild(levelIndex);
				// UnityEngine.Debug.Log(level);
				level.gameObject.SetActive(levelIndex == _state.CurrentLevelIndex);
			}

			if (_config.HideObstacleLayer || Utils.IsDevBuild() == false)
			{
				foreach (var obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
				{
					obstacle.GetComponent<SpriteRenderer>().enabled = false;
				}
			}

			if (_state.Scores == null)
			{
				_state.Scores = new Score[_levelsContainer.childCount];
			}

			_state.Entities = new List<Entity>();

			var leaderSpawner = GameObject.FindObjectOfType<LeaderSpawner>();
			var leader = new Entity
			{
				Name = "Leader",
				Position = (Vector2)leaderSpawner.transform.position,
				PlayerControlled = true,
				Color = _config.LeaderColor,
				ColorHair = _config.HairColors[UnityEngine.Random.Range(0, _config.HairColors.Length)],
				ColorSkin = _config.SkinColors[UnityEngine.Random.Range(0, _config.SkinColors.Length)],
				ColorSwap = true,
				RecruitmentRadius = 2f,
				Sprite = _config.LeaderSprite,
				CanBeHit = true,
				Alliance = Alliances.Ally,
				ColliderType = 1,
				MoveSpeed = 3,
				AttackRadius = 1f,
				CastShadow = true,
				AnimatorController = _config.HumanAnimatorController,
				Human = true,
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
					ColorHair = _config.HairColors[UnityEngine.Random.Range(0, _config.HairColors.Length)],
					ColorSkin = _config.SkinColors[UnityEngine.Random.Range(0, _config.SkinColors.Length)],
					ColorSwap = true,
					WillFollowerLeader = true,
					Sprite = _config.FollowerSprite,
					Alliance = Alliances.Ally,
					ColliderType = 1,
					MoveSpeed = 10,
					AttackRadius = 1f,
					CastShadow = true,
					AnimatorController = _config.HumanAnimatorController,
					Human = true,
				};
				_state.Entities.Add(entity);
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
					ColorHair = _config.HairColors[UnityEngine.Random.Range(0, _config.HairColors.Length)],
					ColorSkin = _config.SkinColors[UnityEngine.Random.Range(0, _config.SkinColors.Length)],
					ColorSwap = true,
					Sprite = _config.CopSprite,
					Alliance = Alliances.Foe,
					AttackCooldown = 1f,
					ColliderType = 1,
					ShootOnSight = true,
					AttackRadius = 5,
					CanBeHit = true,
					CastShadow = true,
					AnimatorController = _config.HumanAnimatorController,
					Human = true,
				};
				_state.Entities.Add(entity);
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
					RigidbodyType = RigidbodyType2D.Static,
					CanBeHit = true,
					HealthCurrent = 4,
					HealthMax = 4,
					AnimatorController = _config.StaticAnimatorController,
				};
				_state.Entities.Add(entity);
			}

			var exitSpawners = GameObject.FindObjectsOfType<ExitSpawner>();
			for (int i = 0; i < exitSpawners.Length; i++)
			{
				var spawner = exitSpawners[i];
				var entity = new Entity
				{
					Name = "Exit " + i,
					Position = (Vector2)spawner.transform.position,
					// ColorSwap = true,
					// Color = _config.LeaderColor,
					RigidbodyType = RigidbodyType2D.Static,
					Sprite = _config.ExitSprite,
					TriggerVictory = true,
					AttackRadius = 1f,
					AnimatorController = _config.StaticAnimatorController,
				};
				_state.Entities.Add(entity);
			}

			foreach (var entity in _state.Entities)
			{
				Rendering.SpawnEntity(entity, _config.EntityPrefab.GetComponent<EntityComponent>());
			}

			_followersFlock.FollowTarget = leader;

			_fogTilemap = GameObject.Find("Fog")?.GetComponent<Tilemap>();

			_ = _ui.FadeOut();
			_controls.Gameplay.Enable();
		}

		public override void Tick()
		{
			base.Tick();

			_state.Timer += Time.deltaTime;

			if (Keyboard.current.f1Key.wasPressedThisFrame)
			{
				NextLevel();
			}

			if (Keyboard.current.f2Key.wasPressedThisFrame)
			{
				GameOver();
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

							targetsCount += 1;
						}
					}

					if (entity.PlayerControlled || targetsCount > 0)
					{
						entity.AttackTimestamp = Time.time + entity.AttackCooldown;
						entity.Component.Animator.Play("Attack");
					}
				}
			}

			for (int entityIndex = _state.Entities.Count - 1; entityIndex >= 0; entityIndex--)
			{
				var entity = _state.Entities[entityIndex];

				if (entity.PlayerControlled)
				{
					if (_fogTilemap != null)
					{
						for (int x = -_config.FogDistance; x < _config.FogDistance; x++)
						{
							for (int y = -_config.FogDistance; y < _config.FogDistance; y++)
							{
								var pos = new Vector3(entity.Position.x + x, entity.Position.y + y, 0);
								var cellPosition = _fogTilemap.WorldToCell(pos);
								_fogTilemap.SetTile(cellPosition, null);
							}
						}
					}

					if (entity.HealthCurrent == 0)
					{
						GameOver();
					}

					// Movement
					if (Time.time > entity.AttackTimestamp)
					{
						var moveInput = _controls.Gameplay.Move.ReadValue<Vector2>();
						entity.Velocity = (float2)moveInput * entity.MoveSpeed;
					}

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

				if (entity.TriggerVictory)
				{
					var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.AttackRadius, LayerMask.GetMask("Entity"));
					foreach (var collider in colliders)
					{
						var otherComponent = collider.GetComponentInParent<EntityComponent>();
						if (otherComponent != null && otherComponent.Entity.PlayerControlled)
						{
							NextLevel();
						}
					}
				}

				if (entity.HealthCurrent <= 0 && entity.FlaggedForDestroy == false)
				{
					if (entity.Human && entity.Flock == _followersFlock)
					{
						_state.FollowersCounter += 1;
					}
					if (entity.Human && entity.Alliance == Alliances.Foe)
					{
						_state.CopsCounter += 1;
					}
					entity.Component.Animator.Play("Destroy");
					entity.FlaggedForDestroy = true;
					entity.CastShadow = false;
					entity.DestroyTimestamp = Time.time + 0.5f;
				}

				if (entity.ShootOnSight)
				{
					Shoot(entity);
				}

				if (entity.Flock != null)
				{
					entity.Flock.Tick(entity);
				}

				if (entity.AttackOnCollision)
				{
					var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.AttackRadius, LayerMask.GetMask("Entity", "Obstacle"));
					foreach (var collider in colliders)
					{
						if (collider == entity.Component.Collider)
						{
							continue;
						}

						var otherComponent = collider.GetComponentInParent<EntityComponent>();
						if (otherComponent == null)
						{
							// Obstacle
						}
						else
						{
							var otherEntity = otherComponent.Entity;
							if (!otherEntity.CanBeHit || otherEntity.Alliance == entity.Alliance)
							{
								continue;
							}

							var damage = 1;
							otherEntity.HealthCurrent = math.max(0, otherEntity.HealthCurrent - damage);
							otherEntity.Component.Animator.Play("Hit");
						}

						UnityEngine.Debug.Log(entity.Name + " => " + collider.name);
						entity.HealthCurrent = 0;
						entity.Velocity = 0;
						entity.AttackOnCollision = false;

						break;
					}
				}

				if (entity.FlaggedForDestroy && Time.time > entity.DestroyTimestamp)
				{
					GameObject.Destroy(entity.Component.gameObject);
					_state.Entities.RemoveAt(entityIndex);
				}
			}

			Rendering.UpdateEntities(_state.Entities);
		}

		private void Shoot(Entity entity)
		{
			if (entity.FlaggedForDestroy || Time.time <= entity.AttackTimestamp)
			{
				return;
			}

			var colliders = Physics2D.OverlapCircleAll(entity.Position, entity.AttackRadius, LayerMask.GetMask("Entity"));
			foreach (var collider in colliders)
			{
				if (collider == entity.Component.Collider)
				{
					continue;
				}

				var otherComponent = collider.GetComponentInParent<EntityComponent>();
				var otherEntity = otherComponent.Entity;

				var difference = otherEntity.Position - entity.Position;
				var direction = math.normalizesafe(difference);

				if (otherEntity.CanBeHit && otherEntity.Alliance != entity.Alliance && otherEntity.Alliance != Alliances.None)
				{
					var hasLineOfSight = true;
					var hits = Physics2D.RaycastAll(entity.Position, direction, ((Vector2)difference).magnitude, LayerMask.GetMask("Entity", "Obstacle"));
					foreach (var hit in hits)
					{
						if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
						{
							// UnityEngine.Debug.Log("bla : " + hit.collider.transform.parent.name + " / other: " + otherEntity.Name);
							hasLineOfSight = false;
							break;
						}
					}

					if (hasLineOfSight)
					{
						UnityEngine.Debug.Log(entity.Name + " SHOOT => " + otherEntity.Name);

						var projectile = new Entity
						{
							Name = "Projectile",
							Position = entity.Position + direction * 0.5f,
							Color = entity.Color,
							RigidbodyType = RigidbodyType2D.Kinematic,
							ColliderScale = 0.1f,
							Sprite = _config.ProjectileSprite,
							Alliance = entity.Alliance,
							ColliderType = 1,
							SortingOrder = 1,
							AttackRadius = 0.1f,
							AttackOnCollision = true,
							Direction = direction,
							MoveSpeed = 15f,
							CastShadow = false,
							AnimatorController = _config.StaticAnimatorController,
						};
						projectile.Velocity = direction * projectile.MoveSpeed;
						Rendering.SpawnEntity(projectile, _config.EntityPrefab.GetComponent<EntityComponent>());
						_state.Entities.Add(projectile);

						entity.AttackTimestamp = Time.time + entity.AttackCooldown;
						entity.Component.Animator.Play("Attack");

						return;
					}
				}
			}
		}

		public override async UniTask Exit()
		{
			await base.Exit();

			_controls.Gameplay.Disable();
			await _ui.FadeIn(Color.black);

			foreach (var entity in _state.Entities)
			{
				GameObject.Destroy(entity.Component.gameObject);
			}
			_state.Entities.Clear();
			_levelsContainer.GetChild(_state.CurrentLevelIndex).gameObject.SetActive(false);
		}

		private void NextLevel()
		{
			if (_isTransitioning)
			{
				return;
			}

			_isTransitioning = true;

			if (_state.CurrentLevelIndex == _levelsContainer.childCount - 1)
			{
				_fsm.Fire(GameFSM.Triggers.Won);
				return;
			}

			// _state.Scores[_state.CurrentLevelIndex] = new Score
			// {
			// 	Timer = _levelTimer,
			// 	// TODO: Check the range of followers
			// 	Followers = _state.Entities.Where(e => e.Flock == _followersFlock && e.HealthCurrent > 0).Count(),
			// };
			_fsm.Fire(GameFSM.Triggers.NextLevel);
		}

		private void GameOver()
		{
			if (_isTransitioning)
			{
				return;
			}
			_isTransitioning = true;
			_fsm.Fire(GameFSM.Triggers.Lost);
		}
	}
}
