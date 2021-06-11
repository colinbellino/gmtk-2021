using Unity.Mathematics;
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

		private static float3 GridToWorldPosition(float2 position, float z = 0)
		{
			return GridToWorldPosition(position.x, position.y, z);
		}

		private static float3 GridToWorldPosition(float x, float y, float z = 0)
		{
			// Uncomment this to render using classic 3d axis order (x,z,y)
			// return new float3(x, z, y);

			return new float3(x, y, z);
		}
	}
}
