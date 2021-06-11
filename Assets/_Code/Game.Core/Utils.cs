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
	}
}
