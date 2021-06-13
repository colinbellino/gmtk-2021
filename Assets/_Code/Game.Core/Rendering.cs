using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
	public static class Rendering
	{
		public static void SetColorSwap(Material material, Color clothColor, Color hairColor, Color skinColor)
		{
			material.SetColor("ReplacementColor1", clothColor);
			material.SetColor("ReplacementColor2", hairColor);
			material.SetColor("ReplacementColor3", skinColor);
		}

		public static void UpdateEntities(List<Entity> entities)
		{
			for (int entityIndex = 0; entityIndex < entities.Count; entityIndex++)
			{
				var entity = entities[entityIndex];

				entity.Component.SpriteRenderer.sortingOrder = entity.SortingOrder;

				if (entity.RigidbodyType != RigidbodyType2D.Static)
				{
					entity.Component.transform.right = (Vector2)entity.Direction;
					entity.Component.Rigidbody.velocity = entity.Velocity;
				}

				entity.Component.UI.gameObject.SetActive(entity.HealthCurrent != entity.HealthMax && entity.HealthCurrent > 0);
				entity.Component.HealthSlider.value = (float)entity.HealthCurrent / entity.HealthMax;

				entity.Position = (Vector2)entity.Component.transform.position;
			}
		}

		public static void SpawnEntity(Entity entity, EntityComponent entityPrefab)
		{
			var component = GameObject.Instantiate(entityPrefab, Utils.GridToWorldPosition(entity.Position), Quaternion.identity);
			component.name = entity.Name;

			component.SpriteRenderer.sprite = entity.Sprite;
			component.SpriteRenderer.material = GameObject.Instantiate(component.SpriteRenderer.material);
			if (entity.ColorSwap)
			{
				component.SpriteRenderer.material.SetColor("ReplacementColor2", entity.Color);
			}

			component.RecruitmentRadiusRenderer.transform.localScale = new Vector2(entity.RecruitmentRadius * 2, entity.RecruitmentRadius * 2);
			component.AttackRadiusRenderer.transform.localScale = new Vector2(entity.AttackRadius * 2, entity.AttackRadius * 2);

			component.Collider = entity.ColliderType switch
			{
				1 => component.CircleCollider,
				_ => component.BoxCollider,
			};
			component.Collider.transform.localScale = Vector3.one * entity.ColliderScale;
			component.Collider.gameObject.SetActive(true);

			component.Rigidbody.bodyType = entity.RigidbodyType;
			component.gameObject.isStatic = entity.RigidbodyType == RigidbodyType2D.Static;

			component.Entity = entity;

			entity.Component = component;
		}
	}
}
