using System;
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

				entity.Component.Rigidbody.velocity = entity.Velocity;
				entity.Position = (Vector2)entity.Component.transform.position;
			}
		}
	}
}
