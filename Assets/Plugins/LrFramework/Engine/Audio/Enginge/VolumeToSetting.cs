using Audio;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace Audio
{

    public class VolumeToSetting : MonoBehaviour
    {
        SoundPlayer      sound;
        SubtitlePlayer   subtitle;
        BackGroundPlayer backGround;

        string sign => $"{Application.productName}_balanceRadio";
        

        private void Awake()
        {
            sound      = GetComponent<SoundPlayer>();
            subtitle   = GetComponent<SubtitlePlayer>();
            backGround = GetComponent<BackGroundPlayer>();

            AudioMgr.BGVolume  .action = SetBG;
            AudioMgr.GameVolume.action = SetSound;
        }


        [Range(0.0f, 1.0f)]
        public float balanceRadio;


        float currentSoundRadio;

        private void Start()
        {
            balanceRadio = PlayerPrefs.GetFloat(sign, 0.5f);
        }
        

        private void Update()
        {
            if (subtitle.playing)
            {
                if (backGround.volume != balanceRadio)
                {
                    sound     .SetVolume(balanceRadio);
                    backGround.SetVolume(balanceRadio);

                    currentSoundRadio  = balanceRadio;
                }
            }
            else
            {
                if (backGround.volume != 1)
                {
                    sound     .SetVolume(1);
                    backGround.SetVolume(1);

                    currentSoundRadio = 1;
                }
            }
        }

        public void SetBG(float volume)
        {       
            backGround.SetVolume(currentSoundRadio);
        }

        public void SetSound(float volume)
        {
            sound   .SetVolume(currentSoundRadio);
            subtitle.SetVolume(1);
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetFloat(sign, balanceRadio);

            AudioMgr.BGVolume  .action = null;
            AudioMgr.GameVolume.action = null;
        }
    }
}