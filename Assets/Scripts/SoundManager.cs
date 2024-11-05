using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance;
	[SerializeField] private AudioSource _MusicSource, _SFXSource;
	[SerializeField] private AudioClip _TestClip;

	void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void PlayMusic(AudioClip clip)
	{
		_MusicSource.PlayOneShot(clip);
	}
	
	void PlaySFX(AudioClip clip)
	{
		_SFXSource.PlayOneShot(clip);
	}

	void Start()
	{
		PlaySFX(_TestClip);
	}

}
