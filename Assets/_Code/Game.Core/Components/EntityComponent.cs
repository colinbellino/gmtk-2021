using UnityEngine;
using UnityEngine.UI;

namespace Game.Core
{
	public class EntityComponent : MonoBehaviour
	{
		public SpriteRenderer SpriteRenderer;
		public Animator Animator;
		public Rigidbody2D Rigidbody;
		public Collider2D BoxCollider;
		public Collider2D CircleCollider;
		public FlockAgent FlockAgent;
		public SpriteRenderer RecruitmentRadiusRenderer;
		public SpriteRenderer AttackRadiusRenderer;
		public Canvas UI;
		public Slider HealthSlider;

		[HideInInspector] public Collider2D Collider;
		[HideInInspector] public Entity Entity;
	}
}
