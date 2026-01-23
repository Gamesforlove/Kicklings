using System;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemPlay : MonoBehaviour
    {
        ParticleSystem _particleSystem;
        void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        void Start()
        {
            if(!_particleSystem.isPlaying) _particleSystem.Play();
        }
    }
}
