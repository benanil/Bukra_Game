
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using AnilTools.Save;
using System.IO;
using System;

namespace MiddleGames.Misc
{
    [Serializable]
    public struct SkyboxProperty{
        public Material material;
        public float MorningAmbient;
        public bool isWindow;
        [Tooltip("only for night")]
        public float glowMultipler;
        public bool PointLightAtNight;
        public bool SpecularAtMorning;
    }

    public class WeatherManager : MonoBehaviour {

        public static bool InEditor;

        public static WeatherManager _instance;
        public static WeatherManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<WeatherManager>();

                    Debug.Log("Count: " + FindObjectsOfType<WeatherManager>().Length);

                    if (_instance == null)
                    {
                        var gameManager = GameObject.Find("GameManager");
                        if (gameManager) _instance = gameManager.AddComponent<WeatherManager>();
                        else _instance = new GameObject(typeof(WeatherManager).Name).AddComponent<WeatherManager>();
                    }
                }
                return _instance;
            }
        }

        public static string SavesFolder
        {
            get
            {
                string directory = Application.persistentDataPath + "/Saves";
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                return directory + '/';
            }
        } 

        public List<WeatherConfig> weatherConfigs;
        [Tooltip("these are activated at night")] // its can be a street lamb or fire point light etc.
        public Light[] PointLights;
        public List<SkyboxProperty> skyboxProperties; // niye adi skybox prop alakası yok : material properties
        [ColorUsage(true,true)]
        public Color GlobalGlowColor;

        public bool LoadFromJsonAtStart;
        public short SecondsBetweenWeathers = 600; // 10dk
        
        public float SunSize  = 0.05f;

        [Tooltip("this is which weather config do yo want to start at begining of scene loading if it is 0 means early morning not even has sun")]
        public int StartConfingIndex = 4;

        private Queue<WeatherConfig> weatherConfigsQueue = new Queue<WeatherConfig>();

        private WeatherConfig currentWeather;

        public bool Active = true;

        [ContextMenu("CalculateSunPositions")]
        public void CalculateSunPositions()
        {
            _instance = this;
            for (int i = 0; i < weatherConfigs.Count; i++)
            {
                weatherConfigs[i].sunAngle = 3.11f - (i * (Mathf.PI / weatherConfigs.Count));
            }
        }

        private void OnValidate()
        {
            _instance = this;
            
            if (!Application.isPlaying) WeatherConfig.SkyboxMaterial.SetFloat("_SunSize", SunSize);
        }

        private void Awake(){
            _instance = this;

            if (!Active) return;

            if (LoadFromJsonAtStart){
                LoadFromJson();
            }

            ResetConfigs(() =>
            {
                currentWeather = weatherConfigsQueue.Dequeue();

                for (int i = 1; i < StartConfingIndex; i++)
                    currentWeather = weatherConfigsQueue.Dequeue();
                
                currentWeather.SetImmediate();
                StartCoroutine(UpdateWeather());
            });
            OnValidate();
        }
        
        private IEnumerator UpdateWeather(){

            while (true) 
            {
                currentWeather = weatherConfigsQueue.Dequeue();
                yield return currentWeather.SetLerp();

                if (weatherConfigsQueue.Count == 0) { // gece
                    ResetConfigs(null);
                }
            } 
        }

        private void ResetConfigs(Action callback) {
            weatherConfigsQueue.Clear();

            weatherConfigsQueue = new Queue<WeatherConfig>(weatherConfigs);

            callback?.Invoke();
        }

        public void SaveToJson() {
            JsonManager.SaveList("Weather/", "WeatherConfigs.json", weatherConfigs);
            Debug2.Log("succsesfully saved", Color.green);
        }

        public void LoadFromJson() {
            LoadList("Weather/", "WeatherConfigs.json");
        }

        public void LoadList(string path, string name)
        {
            CheckPath(path);

            for (int i = 0; i < weatherConfigs.Count; i++) {
                string konum = Path.Combine(SavesFolder + path + i.ToString() + name);

                if (File.Exists(konum)) {
                    string loadText = File.ReadAllText(konum);
                    JsonUtility.FromJsonOverwrite(loadText, weatherConfigs[i]);
                }
            }

            Debug2.Log("succsesfully Loaded", Color.green);
        }

        private static void CheckPath(string path)
        {
            if (path.StartsWith(Application.dataPath)) {
                if (!File.Exists(path)) Directory.CreateDirectory(path);
                return;
            }
            if (!File.Exists(SavesFolder))        Directory.CreateDirectory(SavesFolder);
            if (!File.Exists(SavesFolder + path)) Directory.CreateDirectory(SavesFolder + path);
        }

    }
}