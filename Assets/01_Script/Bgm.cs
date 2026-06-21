using System;
using UnityEngine;

namespace _01_Script
{
    public class Bgm : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip MainBgmClip;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = MainBgmClip;
            audioSource.Play();
        }
    }
}