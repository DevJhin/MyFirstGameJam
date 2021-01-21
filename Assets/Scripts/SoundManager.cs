using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SoundManager : SerializedMonoBehaviour {

    public static SoundManager Instance { get; private set; }

    //�����Ŭ������ ������ �̸� Inspector���� ����
    [System.Serializable]
    public class Sound {
        public AudioClip clip;
        public float volume;
    }

    //������
    [SerializeField]
    public Dictionary<string, Sound> sound = new Dictionary<string, Sound>();

    //Output: BGM Audio Mixer Group
    private AudioSource bgmAudioSource;

    //Output: SFX Audio Mixer Group
    private AudioSource sfxAudioSource;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        bgmAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        sfxAudioSource = transform.GetChild(1).GetComponent<AudioSource>();
    }

    //SFX CONTROLS

    /// <summary>
    /// �ڱ� ������ҽ� ���� PlayOneShot()
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    public void PlayOneShotHere(AudioSource source, string name) {
        source.PlayOneShot(sound[name].clip, sound[name].volume);
    }
    //OR audioSource.PlayOneShot(SoundManager.Instance.sound[string].clip, float volume);

    /// <summary>
    /// �ڱ� ������ҽ� ���� Play()
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    public void PlayHere(AudioSource source, string name) {
        source.Stop();
        source.clip = sound[name].clip;
        source.volume = sound[name].volume;
        source.Play();
    }

    /// <summary>
    /// SoundManager ������ҽ� ���� PlayOneShot()
    /// </summary>
    /// <param name="name"></param>
    public void PlayOneShotThere(string name) {
        sfxAudioSource.PlayOneShot(sound[name].clip, sound[name].volume);
    }

    /// <summary>
    /// SoundManager ������ҽ� ���� Play()
    /// </summary>
    /// <param name="name"></param>
    public void PlayThere(string name) {
        sfxAudioSource.Stop();
        sfxAudioSource.clip = sound[name].clip;
        sfxAudioSource.volume = sound[name].volume;
        sfxAudioSource.Play();
    }

    /// <summary>
    /// SoundManager ������ҽ� ���� Pause()
    /// </summary>
    public void PauseThere() {
        sfxAudioSource.Pause();
    }

    /// <summary>
    /// SoundManager ������ҽ� ���� Stop()
    /// </summary>
    public void StopThere() {
        sfxAudioSource.Stop();
    }

    /// <summary>
    /// PlayClipAtPoint()
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pos"></param>
    public void PlayClipAtPoint(string name, Vector2 pos) {
        AudioSource.PlayClipAtPoint(sound[name].clip, pos, sound[name].volume);
    }

    //BGM CONTROLS

    public void PlayBgm() {
        bgmAudioSource.Play();
    }

    public void PauseBgm() {
        bgmAudioSource.Pause();
    }

    public void StopBgm() {
        bgmAudioSource.Stop();
    }

    public void ChangeBgm(string name) {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = sound[name].clip;
        bgmAudioSource.volume = sound[name].volume;
        bgmAudioSource.Play();
    }
}