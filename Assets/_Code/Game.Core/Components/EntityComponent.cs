using UnityEngine;

namespace Game.Core
{
	public class EntityComponent : MonoBehaviour
	{
		public Entity Entity;
		public SpriteRenderer SpriteRenderer;
		public Rigidbody2D Rigidbody;
		public Collider2D Collider;
		public FlockAgent FlockAgent;

		public SpriteRenderer RecruitmentRadiusRenderer;
	}
}
