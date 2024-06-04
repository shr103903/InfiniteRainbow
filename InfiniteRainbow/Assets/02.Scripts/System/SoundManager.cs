using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Define
{
	public enum Sound
	{
		Master,
		Bgm,
		Effect,
		UI,
		MaxCount, // 종류 개수 의미
	}
}

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);

			Init();
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	[SerializeField]
	private GameObject soundPanel = null;

	public Slider masterVolumeSlider = null;

	public Slider bgmVolumeSlider = null;

	public Slider effectVolumeSlider = null;

	public Slider uiVolumeSlider = null;

	[SerializeField]
	private AudioMixer mixer = null;

	public float masterValue { get; private set; }

	public float bgmValue { get; private set; }

	public float effectValue { get; private set; }

	public float uiValue { get; private set; }

	private AudioSource[] audioSources = new AudioSource[(int)Define.Sound.MaxCount];
	private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

	public void Init()
	{
		GameObject root = GameObject.Find("@Sound");
		if (root == null)
		{
			root = new GameObject { name = "@Sound" };
			root.transform.parent = this.gameObject.transform;

			string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
			for (int i = 0; i < soundNames.Length - 1; i++)
			{
				GameObject go = new GameObject { name = soundNames[i] };
				audioSources[i] = go.AddComponent<AudioSource>();
				go.transform.parent = root.transform;
			}

			audioSources[(int)Define.Sound.Bgm].loop = true;
		}

		audioSources[(int)Define.Sound.Master].outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
        audioSources[(int)Define.Sound.Bgm].outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        audioSources[(int)Define.Sound.Effect].outputAudioMixerGroup = mixer.FindMatchingGroups("Effect")[0];
        audioSources[(int)Define.Sound.UI].outputAudioMixerGroup = mixer.FindMatchingGroups("UI")[0];

		if (PlayerPrefs.HasKey("MasterVolume"))
		{
			masterValue = PlayerPrefs.GetFloat("MasterVolume");
		}
		else
		{
			masterValue = 0.5f;
		}
		if (PlayerPrefs.HasKey("BgmVolume"))
		{
			bgmValue = PlayerPrefs.GetFloat("BgmVolume");
		}
		else
		{
			bgmValue = 0.5f;
		}
		if (PlayerPrefs.HasKey("EffectVolume"))
		{
			effectValue = PlayerPrefs.GetFloat("EffectVolume");
		}
		else
		{
			effectValue = 0.5f;
		}
		if (PlayerPrefs.HasKey("UiVolume")) 
		{ 
			uiValue = PlayerPrefs.GetFloat("UiVolume");
		}
		else
		{
			uiValue = 0.5f;
		}

		MasterSoundVolume(masterValue);
		BGMSoundVolume(bgmValue);
		EffectSoundVolume(effectValue);
		UISoundVolume(uiValue);

		SetSoundValueImage();
	}

	public void ActiveSoundPanel(bool active)
    {
		if(soundPanel.activeSelf != active)
        {
			soundPanel.SetActive(active);
        }
	}

	public void Pause(Define.Sound type = Define.Sound.Bgm)
	{
		//bgm
		if (type == Define.Sound.Bgm)
		{
			audioSources[(int)Define.Sound.Bgm].Pause();
		}
		//effect
		else
		{
			audioSources[(int)Define.Sound.Effect].Pause();
		}

	}

	public void Stop(Define.Sound type = Define.Sound.Bgm)
	{
		if (type == Define.Sound.Bgm)
		{
			audioSources[(int)Define.Sound.Bgm].Stop();
		}
		else
		{
			audioSources[(int)Define.Sound.Effect].Stop();
		}

	}

	public void RePlay(Define.Sound type = Define.Sound.Bgm)
	{
		if (type == Define.Sound.Bgm)
		{
			audioSources[(int)Define.Sound.Bgm].Play();
		}
		else
		{
			audioSources[(int)Define.Sound.Effect].Play();
		}
	}

	public void Play(string path, Define.Sound type = Define.Sound.Effect, float volum = 0.8f)
	{
		AudioClip audioClip = GetOrAddAudioClip(path, type);
		Play(audioClip, type, volum);
	}

	public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float volume = 0.8f)
	{
		if (audioClip == null)
			return;

		if (type == Define.Sound.Bgm)
		{
			AudioSource audioSource = audioSources[(int)Define.Sound.Bgm];

			audioSource.volume = volume;
			audioSource.clip = audioClip;
			if (!audioSource.isPlaying)
				audioSource.Play();
		}
		else
		{
			AudioSource audioSource = audioSources[(int)Define.Sound.Effect];
			audioSource.volume = volume;
			audioSource.PlayOneShot(audioClip);
		}
	}

	AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
	{
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Load<AudioClip>(path);
		}
		else
		{
			if (audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Load<AudioClip>(path);
				audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.LogError($"AudioClip Missing ! {path}");

		return audioClip;
	}

	public void Clear()
	{
		// 재생기 전부 재생 스탑, 음반 빼기
		foreach (AudioSource audioSource in audioSources)
		{
			audioSource.clip = null;
			audioSource.Stop();
		}
		// 효과음 Dictionary 비우기
		audioClips.Clear();
	}

	public T Load<T>(string path) where T : Object
	{
		if (typeof(T) == typeof(GameObject))
		{
			string name = path;
			int index = name.LastIndexOf('/');
			if (index >= 0)
				name = name.Substring(index + 1);
		}
		return Resources.Load<T>(path);
	}

	public void MasterSoundVolume(float value)
	{
		if (value <= 0.005f)
		{
			mixer.SetFloat("Master", -80);
			return;
		}
		mixer.SetFloat("Master", Mathf.Log10(value) * 20);
	}

	public void BGMSoundVolume(float value)
	{
		if (value <= 0.005f)
		{
			mixer.SetFloat("BGM", -80);
			return;
		}
		mixer.SetFloat("BGM", Mathf.Log10(value) * 20);
	}

	public void EffectSoundVolume(float value)
	{
		if (value <= 0.005f)
		{
			mixer.SetFloat("Effect", -80);
			return;
		}
		mixer.SetFloat("Effect", Mathf.Log10(value) * 20);
	}

	public void UISoundVolume(float value)
	{
		if (value <= 0.005f)
		{
			mixer.SetFloat("UI", -80);
			return;
		}
		mixer.SetFloat("UI", Mathf.Log10(value) * 20);
	}

	public void MasterVolumValueSetint(float _value)
	{
		masterValue = _value;
		masterVolumeSlider.value = masterValue;
		MasterSoundVolume(masterValue);
		PlayerPrefs.SetFloat("MasterVolume", masterValue);
	}

	public void BGMVolumValueSetint(float _value)
	{
		bgmValue = _value;
		bgmVolumeSlider.value = bgmValue;
		BGMSoundVolume(bgmValue);
		PlayerPrefs.SetFloat("BgmVolume", bgmValue);
	}

	public void EffectVolumValueSetint(float _value)
	{
		effectValue = _value;
		effectVolumeSlider.value = effectValue;
		EffectSoundVolume(effectValue);
		PlayerPrefs.SetFloat("EffectVolume", effectValue);
	}

	public void UIVolumValueSetint(float _value)
	{
		uiValue = _value;
		uiVolumeSlider.value = uiValue;
		UISoundVolume(uiValue);
		PlayerPrefs.SetFloat("UiVolume", uiValue);
	}

	private void SetSoundValueImage()
	{
		masterVolumeSlider.value = masterValue;
		bgmVolumeSlider.value = bgmValue;
		effectVolumeSlider.value = effectValue;
		uiVolumeSlider.value = uiValue;
	}
}
