using System.Collections.Generic;
using Unity.Mathematics;

namespace Game.Core
{
	public class GameState
	{
		public float InitialMusicVolume;
		public float CurrentMusicVolume;
		public float InitialSoundVolume;
		public float CurrentSoundVolume;

		public List<Entity> Entities;
		public Score[] Scores;
		public int CurrentLevelIndex;
	}

	public class Score
	{
		public float Timer;
		public int Followers;

		public override string ToString()
		{
			return $"Score: {Followers} in {Timer} seconds";
		}
	}
}
