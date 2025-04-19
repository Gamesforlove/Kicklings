using UnityEngine;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField] AudioClip _playerActionPerformedClip;

        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    
        public void PlayPlayerActionPerformed() => PlaySound(_playerActionPerformedClip);

        void PlaySound(AudioClip clip)
        {
            if (clip != null)
                _audioSource.PlayOneShot(clip);
        }
    }
}