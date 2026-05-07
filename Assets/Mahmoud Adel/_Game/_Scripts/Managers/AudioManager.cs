using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Utilities.Bases;

namespace Managers
{
    public class AudioManager : SimpleSingleton<AudioManager>
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioMixer audioMixer;

        private AudioSource musicSource;

        private const string MASTER = "Master";
        private const string MUSIC = "Music";
        private const string SFX = "SFX";

        protected override void Awake()
        {
            base.Awake();

            musicSource = GetComponent<AudioSource>();

            StartCoroutine(NextFrame());
        }

        private IEnumerator NextFrame()
        {
            yield return null;
            LoadSavedAudioSettings();
        }

        private void LoadSavedAudioSettings()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(MASTER, 1f));
            SetMusicVolume(PlayerPrefs.GetFloat(MUSIC, 1f));
            SetSFXVolume(PlayerPrefs.GetFloat(SFX, 1f));
        }

        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("Audio clip is null");
                return;
            }

            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }

        public void PlaySFXClip(AudioClip clip, Transform spawnPoint, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("Audio clip is null");
                return;
            }

            // instantiate a new game object to play the sound
            AudioSource audioSource = Instantiate(sfxSource, spawnPoint.position, Quaternion.identity);

            // assign the audio clip to the audio source
            audioSource.clip = clip;

            // set the volume
            audioSource.volume = volume;

            // play the sound
            audioSource.Play();

            // destroy the game object after the sound has finished playing
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }

        public void PlaySFXClipRandomPitch(AudioClip clip, Transform spawnPoint, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("Audio clip is null");
                return;
            }

            // instantiate a new game object to play the sound
            AudioSource audioSource = Instantiate(sfxSource, spawnPoint.position, Quaternion.identity);

            // assign the audio clip to the audio source
            audioSource.clip = clip;

            // set random pitch
            audioSource.pitch = Random.Range(0.9f, 1.1f);

            // set the volume
            audioSource.volume = volume;

            // play the sound
            audioSource.Play();

            // destroy the game object after the sound has finished playing
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }

        public void PlayRandomSFXClip(AudioClip[] clips, Transform spawnPoint, float volume = 1f)
        {
            if (clips == null || clips.Length == 0)
            {
                Debug.LogWarning("Audio clips are null or empty");
                return;
            }

            PlaySFXClip(clips[Random.Range(0, clips.Length)], spawnPoint, volume);
        }

        public void PlayRandomSFXClipRandomPitch(AudioClip[] clips, Transform spawnPoint, float volume = 1f)
        {
            if (clips == null || clips.Length == 0)
            {
                Debug.LogWarning("Audio clips are null or empty");
                return;
            }

            PlaySFXClipRandomPitch(clips[Random.Range(0, clips.Length)], spawnPoint, volume);
        }

        internal void SetMasterVolume(float value)
        {
            audioMixer.SetFloat(MASTER, Mathf.Log10(value) * 20);
        }

        internal void SetMusicVolume(float value)
        {
            audioMixer.SetFloat(MUSIC, Mathf.Log10(value) * 20);
        }

        internal void SetSFXVolume(float value)
        {
            audioMixer.SetFloat(SFX, Mathf.Log10(value) * 20);
        }
    }
}
