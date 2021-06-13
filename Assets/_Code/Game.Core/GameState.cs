using System.Collections.Generic;

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
		public int FollowersCounter;
		public int CopsCounter;
		public float Timer;
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
