using Unity.Mathematics;
using UnityEngine;

namespace Game.Core
{
	public static class Utils
	{
		public static readonly string[] NAMES = new string[] {
			"Micah",
			"Vernon",
			"Rena",
			"Riku",
			"Andre",
			"Thea",
			"Mariel",
			"Jesse",
			"Marceline",
			"Gaius",
			"Alma",
			"Ursula",
			"Celeste",
			"Madeline",
			"Theo/thea",
			"Jill",
			"Eliot",
			"Akash",
			"Chel",
			"Rami",
			"Aivi ",
			"Ada",
			"Emna",
		};

		public static bool IsDevBuild()
		{
#if UNITY_EDITOR
			return true;
#endif

#pragma warning disable 162
			return false;
#pragma warning restore 162
		}

		public static int Mod(int x, int m)
		{
			return (x % m + m) % m;
		}

		public static void SpawnEntity(Entity entity, EntityComponent entityPrefab)
		{
			var component = GameObject.Instantiate(entityPrefab, GridToWorldPosition(entity.Position), Quaternion.identity);
			entity.Component = component;
		}

		public static float3 GridToWorldPosition(float2 position, float z = 0)
		{
			return GridToWorldPosition(position.x, position.y, z);
		}

		public static float3 GridToWorldPosition(float x, float y, float z = 0)
		{
			// Uncomment this to render using classic 3d axis order (x,z,y)
			// return new float3(x, z, y);

			return new float3(x, y, z);
		}
	}
}
