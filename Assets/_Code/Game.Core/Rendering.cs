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

		internal static void UpdateEntities(List<Entity> entities)
		{
			for (int entityIndex = 0; entityIndex < entities.Count; entityIndex++)
			{
				var entity = entities[entityIndex];

				if (entity.PlayerControlled)
				{
					entity.Component.SpriteRenderer.sortingOrder = 1;
				}

				if (entity.Static == false)
				{
					entity.Component.Rigidbody.velocity = entity.Velocity;
				}

				if (entity.HealthCurrent == 0)
				{
					entity.Destroyed = true;
				}

				if (entity.Destroyed)
				{
					entity.Component.gameObject.SetActive(false);
				}

				entity.Component.UI.gameObject.SetActive(entity.HealthCurrent != entity.HealthMax && entity.HealthCurrent > 0);
				entity.Component.HealthSlider.value = (float)entity.HealthCurrent / entity.HealthMax;

				entity.Position = (Vector2)entity.Component.transform.position;
			}
		}
	}
}
