using UnityEngine;
using Unity.Entities;
public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	private Transform cameraTransform;
	public AudioSource musicSource;
	EntityManager asd;
	public void Awake()
	{
		instance = this;
	}
	public void PlaySfxRequest(string name)
	{
		AudioClip audio = Resources.Load<AudioClip>($"SFX/{name}");
		if (audio == null) return;
		AudioSource.PlayClipAtPoint(audio, Camera.main.transform.position);
	}

	public void PlayMusicRequest(string name)
	{
		AudioClip audio = Resources.Load<AudioClip>($"Music/{name}");
		if (audio == null) return;

		if (!musicSource.clip.Equals(audio))
		{
			musicSource.clip = audio;
			musicSource.Stop();
			musicSource.Play();
		}
	}

	public void FixAudioListener()
	{
		if (TryGetComponent(out AudioListener audioListener))
		{
			Destroy(audioListener);
		}
	}
}
