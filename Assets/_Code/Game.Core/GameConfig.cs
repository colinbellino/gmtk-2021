using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
	[CreateAssetMenu(menuName = "Game/Game Config")]
	public class GameConfig : ScriptableObject
	{
		[Header("Debug")]
		public bool DebugFSM;
		public int LockFPS = 60;
		public GameObject EntityPrefab;
		public Sprite LeaderSprite;
		public Color LeaderColor;
		public Sprite FollowerSprite;
		public Sprite CrateSprite;
		public Color CopColor;
		public Sprite CopSprite;
		public Sprite ProjectileSprite;

		[Header("Audio")]
		public AudioMixer AudioMixer;
		public AudioMixerGroup MusicAudioMixerGroup;
		public AudioMixerGroup SoundsAudioMixerGroup;
		public AudioMixerSnapshot DefaultAudioSnapshot;
		public AudioMixerSnapshot PauseAudioSnapshot;
		[Range(0f, 1f)] public float MusicVolume = 1f;
		[Range(0f, 1f)] public float SoundVolume = 1f;
		public AudioClip MenuTextAppearClip;
		public AudioClip MenuConfirmClip;
	}
}
