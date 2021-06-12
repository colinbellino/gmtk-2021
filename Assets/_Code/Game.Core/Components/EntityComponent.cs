using UnityEngine;
using UnityEngine.UI;

namespace Game.Core
{
	public class EntityComponent : MonoBehaviour
	{
		public SpriteRenderer SpriteRenderer;
		public Animator Animator;
		public Rigidbody2D Rigidbody;
		public Collider2D Collider;
		public FlockAgent FlockAgent;
		public SpriteRenderer RecruitmentRadiusRenderer;
		public Canvas UI;
		public Slider HealthSlider;

		[HideInInspector] public Entity Entity;
	}
}
