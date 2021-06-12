using Unity.Mathematics;

namespace Game.Core
{
	public class Entity
	{
		public float2 Position;
		public float2 Velocity;
		public float MoveSpeed = 100f;
		public bool IsPlayerControlled;
		public bool IsFollowingLeader;

		public EntityComponent Component;
	}
}
