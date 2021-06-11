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
		public int3 GridSize = new int3(10, 10, 10);
		public int3 PathStart = new int3(0, 5, 0);
		public int3 PathEnd = new int3(9, 5, 9);
		public float SlideSpeed = 10f;

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

		[Header("Board")]
		public GameObject TileRendererPrefab;
		public GameObject UnitRendererPrefab;
		public Sprite[] UnitSprites;
		public Color[] AllianceColors;
	}
}
