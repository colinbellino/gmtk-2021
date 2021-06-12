using Unity.Mathematics;
using UnityEngine;

namespace Game.Core
{
	public class Entity
	{
		public string Name;
		public float2 Position;
		public float2 Velocity;
		public float MoveSpeed = 500f;
		public Color Color;
		public Flock Flock;
		public float RecruitmentRadius;
		public bool PlayerControlled;
		public bool WillFollowerLeader;

		public EntityComponent Component;
	}
}
