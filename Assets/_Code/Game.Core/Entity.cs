using Unity.Mathematics;
using UnityEditor.Animations;
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
		public Color ColorHair;
		public Color ColorSkin;
		public bool ColorSwap;
		public Flock Flock;
		public float MoveSpeed;
		public float RecruitmentRadius;
		public bool PlayerControlled;
		public bool WillFollowerLeader;
		public RigidbodyType2D RigidbodyType;
		public bool CanBeHit;
		public Alliances Alliance;
		public int HealthCurrent = 1;
		public int HealthMax = 1;
		public float AttackRadius;
		public float AttackTimestamp;
		public float AttackCooldown = 0.5f;
		public int SortingOrder;
		public bool FlaggedForDestroy;
		public float DestroyTimestamp;
		public int ColliderType;
		public float ColliderScale = 1;
		public bool ShootOnSight;
		public bool AttackOnCollision;
		public float2 Direction = Vector2.right;
		public float2 FlockDirection;
		public bool TriggerVictory;
		public bool CastShadow;
		public AnimatorController AnimatorController;

		public EntityComponent Component;
	}

	public enum Alliances { None, Ally, Foe }
}
