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
		public bool PlayerControlled;
		public bool WillFollowerLeader;
		public bool Static;

		public EntityComponent Component;
	}
}
