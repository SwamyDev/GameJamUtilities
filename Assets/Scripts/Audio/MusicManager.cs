using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace CyberG {

public class MusicManager : MonoBehaviour
{
	[System.Serializable]
	private struct FillerData
	{
		public AudioClip clip;
		[Range(0.0f, 1.0f)]
		public float volume;
	}

	[System.Serializable]
	private struct TransitionData
	{
		public AudioMixerSnapshot snapShot;
		public FillerData[] fillers;
		[Space(10.0f)]
		[Tooltip("In beats")]
		public float duration;
	}

	[Header("Genera")]
	[SerializeField]
	private float beatsPerMinute = 128;

	[Header("Transitions")]
	[SerializeField]
	private TransitionData inInstances;
	[SerializeField]
	private TransitionData outInstances;

	private Stack<TransitionData> queuedTransitions = new Stack<TransitionData>();
	private AudioSource fillerSource;
	private float beatLength;
	private float timeInBeat;

	void Awake()
	{
		beatLength = 60.0f / beatsPerMinute;
		fillerSource = transform.FindChild("FillerSource").GetComponent<AudioSource>();
	}

	void Start()
	{
		GameEvents events = FindObjectOfType<GameEvents>();
		events.RegisterFor(GameEvents.Events.ON_INSTANCED_IN, OnInstancedIn);
		events.RegisterFor(GameEvents.Events.ON_INSTANCED_OUT, OnInstancedOut);
	}

	void Update()
	{
		if (timeInBeat >= beatLength)
		{
			while (queuedTransitions.Count > 0)
			{
				TransitionData transition = queuedTransitions.Pop();
				transition.snapShot.TransitionTo(transition.duration * beatLength);
			}

			timeInBeat = 0.0f;
		}

		timeInBeat += TimeProvider.GetMyDeltaTime();
	}

	private void OnInstancedIn(object msg, GameObject instigator)
	{
		float fillerLength = beatLength / (float)inInstances.fillers.Length;
		int fillerIdx = (int)(timeInBeat / fillerLength);
		float remaining = timeInBeat - ((float)fillerIdx * fillerLength);

		StartCoroutine(PerformFillerSound(remaining, inInstances.fillers[fillerIdx]));
		queuedTransitions.Push(inInstances);
	}

	private IEnumerator PerformFillerSound(float wait, FillerData filler)
	{
		yield return new WaitForSeconds(wait);
		fillerSource.Stop();
		fillerSource.clip = filler.clip;
		fillerSource.volume = filler.volume;
		fillerSource.Play();
		fillerSource.loop = false;
	}

	private void OnInstancedOut(object msg, GameObject instigator)
	{
		queuedTransitions.Push(outInstances);
	}
}

}	// namespace CyberG