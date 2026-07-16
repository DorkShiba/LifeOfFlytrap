using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public enum SoundType
    {
        Bgm,
        Effect,
        MaxCount,
    }

    private AudioSource[] _audioSources = new AudioSource[(int)SoundType.MaxCount];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)SoundType.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        // 씬 전환 시 재생 중인 효과음(버튼음 등)이 끊기지 않도록 배경음(Bgm)만 정지시킵니다.
        AudioSource bgmSource = _audioSources[(int)SoundType.Bgm];
        if (bgmSource != null)
        {
            bgmSource.clip = null;
            bgmSource.Stop();
        }

        _audioClips.Clear();
    }

    public void Play(string path, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, SoundType type = SoundType.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == SoundType.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)SoundType.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)SoundType.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void PlaySFX(string path, float pitch = 1.0f)
    {
        Play(path, SoundType.Effect, pitch);
    }

    public void PlayBGM(string path, float pitch = 1.0f)
    {
        Play(path, SoundType.Bgm, pitch);
    }

    private AudioClip GetOrAddAudioClip(string path, SoundType type = SoundType.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == SoundType.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                if (audioClip != null)
                    _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }
}
