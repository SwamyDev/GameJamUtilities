using UnityEngine;
using System.Collections;

namespace JamUtilities {

public class SoundManager : MonoBehaviour 
{
	[SerializeField]
	private AudioClip playerShot;
	[SerializeField]
	private AudioClip enemyShot;
	[SerializeField]
    private AudioClip lockonMissile;
	[SerializeField]
	private AudioClip instanced;

	private AudioSource[] sources;

    private static SoundManager instance;

    public static SoundManager Instance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<SoundManager>();
        }

        return instance;
    }

    void Awake()
    {
		sources = GetComponentsInChildren<AudioSource>();
    }

    public void PlayPlayerShot()
    {
		if (sources.Length < 1)
		{
			Debug.LogWarning("Not enough sound slots set up to play player shot");
			return;
		}

		PlayClip(sources[0], playerShot);
    }

	public void PlayEnemyShot(AudioClip clip = null)
	{
		if (sources.Length < 2)
		{
			Debug.LogWarning("Not enough sound slots set up to play enemy shot");
			return;
		}

		AudioSource source = sources[3];
		if (clip == null)
		{
			source = sources[1];
			clip = enemyShot;
		}

		PlayClip(source, clip);
	}

    public void PlayMissileShot()
	{
		if (sources.Length < 3)
		{
			Debug.LogWarning("Not enough sound slots set up to play missile shot");
			return;
		}

		PlayClip(sources[2], lockonMissile);
    }

	public void PlayInstanced()
	{
		if (sources.Length < 5)
		{
			Debug.LogWarning("Not enough sound slots set up to play instanced sfx");
			return;
		}

		PlayClip(sources[4], instanced);
	}

	private void PlayClip(AudioSource source, AudioClip clip)
	{
		if (source.isPlaying && source.time < 0.2f)
			return;

		source.Stop();
		source.clip = clip;
		source.Play();
		source.loop = false;
	}
}

}	// namespace JamUtilities
