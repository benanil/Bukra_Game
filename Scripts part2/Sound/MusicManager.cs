using AnilTools;
using System;
using System.Collections;
using UnityEngine;
using UrFairy;

namespace Assets.Resources.Scripts.Sound{

    public enum EmotionalStuation{ 
        // görev olmadığı zaman boş gezinirken
        Relax,
        // görev sırasında 
        Action ,
        // heyecan : boss fight vs.
        Excitement,
    }
    
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : Singleton<MusicManager>{

        public Music[] Musics;// müzik ismi düzgün olmalı 
        public EmotionalStuation emotion = EmotionalStuation.Relax;
        public float musicFadeSpeed = 1; // bazen müziğin hızlı girmesi gerekebilir

        [NonSerialized]
        public AudioSource MusicSource;
        private AudioClip currentMusic;

        private float MaxSound => XMLmanager.instance.list.ses;

        private void Start(){
            MusicSource = GetComponent<AudioSource>();
        }

        private void Update(){
            if (!MusicSource.isPlaying){
                TriggerMusic(Array.FindAll(Musics, x => x.emotion == emotion).GetRandom().sound);
            }
        }

        public void TriggerMusic(AudioClip music){
            currentMusic = music;

            if (currentMusic == null){
                Debug.LogError("girilmeye çalışılan müzik mevcut değil");
                return;
            }

            StartCoroutine(AudioFade(currentMusic, musicFadeSpeed, null));
        }

        /// trigger event görev başlangıcı vs. ile triggerlayabilmek için string
        public void TriggerMusic(string musicName){
            TriggerMusic(Array.Find(Musics, x => x.sound.name == musicName).sound);
        }

        /// <summary>
        /// sesi ilk düşürür sonrasında yeni müzik ekleyip sesi arttırır
        /// </summary>
        IEnumerator AudioFade(AudioClip music, float speed,Action endAction){
            float startTime = Time.time;
            float dt;

            while (MusicSource.volume.Difrance(0) < 0.01f){
                dt = Time.time - startTime;
                MusicSource.volume = Mathf.Lerp(MusicSource.volume, 0, speed * dt);
                startTime = Time.time;
                yield return new WaitForEndOfFrame();
            }
            
            MusicSource.volume = 0;
            MusicSource.clip = music; 

            while (MusicSource.volume.Difrance(MaxSound) < 0.01f)
            {
                dt = Time.time - startTime;
                MusicSource.volume = Mathf.Lerp(MusicSource.volume, MaxSound, speed * dt);
                startTime = Time.time;
                yield return new WaitForEndOfFrame();
            }
            MusicSource.volume = MaxSound;
            endAction.IfJust(x => x.Invoke());
            MusicSource.Play();
        }

    }
    
    [Serializable]
    public struct Music{
        public AudioClip sound;
        public EmotionalStuation emotion;
    }
}