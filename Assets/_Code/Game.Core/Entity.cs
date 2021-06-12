using System.Security.Policy;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Core
{
	public class Entity
	{
		public string Name;
		public Sprite Sprite;
		public float2 Position;
		public float2 Velocity;
		public Color Color;
		public Flock Flock;
		public float MoveSpeed = 500f;
		public float RecruitmentRadius;
		public float HitRadius = 1f;
		public bool PlayerControlled;
		public bool WillFollowerLeader;
		public bool Static;
		public bool CanBeHit;
		public Alliances Alliance;
		public int HealthCurrent = 1;
		public int HealthMax = 1;
		public float AttackTimestamp;
		public float AttackCooldown = 0.5f;
		public int SortingOrder;
		public bool FlaggedForDestroy;
		public float DestroyTimestamp;
		public int ColliderType;

		public EntityComponent Component;
	}

	public enum Alliances { None, Ally, Foe }
}
