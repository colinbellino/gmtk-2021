using UnityEngine;

namespace Game.Core
{
	public class EntityComponent : MonoBehaviour
	{
		public SpriteRenderer SpriteRenderer;
		public Rigidbody2D Rigidbody;
		public Collider2D Collider;
		public FlockAgent FlockAgent;
	}
}
