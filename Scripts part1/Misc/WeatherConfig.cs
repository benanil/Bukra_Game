
using AnilTools.Update;
using System;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MiddleGames.Misc
{
    [Serializable]
    public class SkyboxProperties
    {
        public static readonly int _SkyTint     = Shader.PropertyToID("_SkyTint");
        public static readonly int _HorizonColor= Shader.PropertyToID("_HorizonColor");
        public static readonly int _GroundColor = Shader.PropertyToID("_GroundColor");
        public static readonly int _HorizonSize = Shader.PropertyToID("_HorizonSize");
        public static readonly int _SunColor    = Shader.PropertyToID("_SunColor");

        public Color Sky     = Color.blue * 1.5f;
        public Color Horizon = Color.yellow + Color.red;
        public Color Ground  = Color.white;

        [ColorUsage(true, true)]
        public Color sunSkyColor = Color.yellow * 2;
    
        public float HorizonSize = .5f;

        public void GetValuesFromScene()
        {
            Sky         = WeatherConfig.SkyboxMaterial.GetColor(_SkyTint);
            Ground      = WeatherConfig.SkyboxMaterial.GetColor(_GroundColor);
            Horizon     = WeatherConfig.SkyboxMaterial.GetColor(_HorizonColor);
            sunSkyColor = WeatherConfig.SkyboxMaterial.GetColor(_SunColor);
            HorizonSize = WeatherConfig.SkyboxMaterial.GetFloat(_HorizonSize); 
        }

        public void Set(Material material)
        {
            material.SetColor(_SkyTint     , Sky        );
            material.SetColor(_HorizonColor, Horizon    );
            material.SetColor(_GroundColor , Ground     );
            material.SetColor(_SunColor    , sunSkyColor);
            material.SetFloat(_HorizonSize , HorizonSize);
            RenderSettings.skybox = material;
        }
    
        public void SetSlowly(Material material)
        {
            float startTime = Time.time;
            float endTime   = Time.time + WeatherManager.instance.SecondsBetweenWeathers;
            float ellapsedTime = 0,percent = 0;
    
            Color skyTintStart      = material.GetColor(_SkyTint     );
            Color horizonColorStart = material.GetColor(_HorizonColor);
            Color groundColorStart  = material.GetColor(_GroundColor );
            Color sunStartColor     = material.GetColor(_SunColor    );
            float horizonSizeStart  = material.GetFloat(_HorizonSize );

            RegisterUpdate.UpdateTill(null, () =>
            {
                ellapsedTime = Time.time - startTime;
                percent = ellapsedTime / WeatherManager.instance.SecondsBetweenWeathers;
                material.SetColor(_SkyTint     , Color.Lerp(skyTintStart     , Sky        , percent));
                material.SetColor(_HorizonColor, Color.Lerp(horizonColorStart, Horizon    , percent));
                material.SetColor(_GroundColor , Color.Lerp(groundColorStart , Ground     , percent));
                material.SetColor(_SunColor    , Color.Lerp(sunStartColor    , sunSkyColor, percent));
                material.SetFloat(_HorizonSize , Mathf.MoveTowards(horizonSizeStart , HorizonSize, percent));
                RenderSettings.skybox = material;
            }, endTime);

            RenderSettings.skybox = material;
        }
    }
    
    public class WeatherConfig : ScriptableObject
    {
        [Header("Fog")]
        public bool fog = false;
        public Color FogColor;
        public float fogDensity = 0.001f;
        private static Material _skyboxMaterial;
        public static Material SkyboxMaterial
        {
            get {
                if (_skyboxMaterial == null) _skyboxMaterial = Resources.Load<Material>("shaders/IndireGandi/Sky Default");
                if (_skyboxMaterial == null)
                {
                    _skyboxMaterial = new Material(Shader.Find("Custom/Color Skybox"));
#if UNITY_EDITOR
                    AssetDatabase.CreateAsset(_skyboxMaterial, "Assets/Resources/shaders/IndireGandi/Sky Default.asset");
#endif
                }

                return _skyboxMaterial;
            }
        }

        public SkyboxProperties skyboxProperties;
        [Header("Sun")]
        public Color SunLightColor = new Color(1,1,1);

        public float SunIntensity = 1f;
        // non seriallized
        public float sunAngle = 1.3f;
        public Vector3 SunRotation = new Vector3(90,0,0);
        [NonSerialized]
        public Mesh sunMesh;
        public bool Clouds;
        public bool IsNight;

        static string CopiedJsonString;

#if UNITY_EDITOR

        [MenuItem("CONTEXT/ScriptableObject/Copy WeatherConfig")]
        public static void CopyWeatherConfig(MenuCommand command){
            CopiedJsonString = JsonUtility.ToJson((WeatherConfig)command.context);
        }

        [MenuItem("CONTEXT/ScriptableObject/Paste WeatherConfig")]
        public static void PasteWeatherConfig(MenuCommand command){
            if (!(CopiedJsonString == null || CopiedJsonString == string.Empty)){
                JsonUtility.FromJsonOverwrite(CopiedJsonString, command.context as WeatherConfig);
            }
        }
#endif

        public static void EnsureSun()
        {
            if (RenderSettings.sun == null)
            {
                RenderSettings.sun = FindObjectsOfType<Light>().Where(x => x.type == LightType.Directional).FirstOrDefault();
                if (RenderSettings.sun == null)
                {
                    RenderSettings.sun = new GameObject("Directional Light").AddComponent<Light>();
                    RenderSettings.sun.type = LightType.Directional;
                }
            }
        }

        private const string _SunAngle = "_SunAngle";
        

        private void OnEnable()
        {
            EnsureSun();
        }

        private void OnValidate()
        {
            EnsureSun();
            if (!Application.isPlaying) SetImmediate();
        }


        public void SetImmediate(){

            if (WeatherManager.InEditor || Application.isPlaying)
            {
                Debug2.Log("weather changed " + name, Color.green);

                skyboxProperties.Set(SkyboxMaterial);
                
                RenderSettings.fogDensity = fogDensity;
                RenderSettings.fogColor = FogColor;
                RenderSettings.sun.color = SunLightColor;
                RenderSettings.sun.intensity = SunIntensity;
                RenderSettings.sun.gameObject.SetActive(true);
                RenderSettings.sun.transform.rotation = Quaternion.LookRotation(-new Vector3(0, Mathf.Sin(sunAngle), Mathf.Cos(sunAngle)));
                
                SkyboxMaterial.SetFloat(_SunAngle, sunAngle);
                
                ImmediateActions();
            }
        }

        public IEnumerator SetLerp()
        {
            Debug2.Log("weather lerping to " + name, Color.green);
            
            ImmediateActions();

            float UpdateStartTime  = Time.time;
            float endTime          = Time.time + WeatherManager.instance.SecondsBetweenWeathers;
            float fogStart         = RenderSettings.fogDensity;
            float sunIntenstyStart = RenderSettings.sun.intensity;
            float sunAngleStart    = SkyboxMaterial.GetFloat(_SunAngle); 
            float ellapsedTime, percent;

            Quaternion sunStartRotation  = RenderSettings.sun.transform.rotation;
            Quaternion targetRotation    = Quaternion.LookRotation(-new Vector3(0, Mathf.Sin(sunAngle), Mathf.Cos(sunAngle)));
            Color sunLightColorStart     = RenderSettings.sun.color;
            Color fogColorStart          = RenderSettings.fogColor;

            Debug.Log("target euler angles: " + targetRotation.eulerAngles);

            skyboxProperties.SetSlowly(SkyboxMaterial);

            while (Time.time < endTime)
            {
                ellapsedTime = Time.time - UpdateStartTime;
                percent      = ellapsedTime / WeatherManager.instance.SecondsBetweenWeathers;

                RenderSettings.fogDensity    = Mathf.MoveTowards(fogStart, fogDensity, percent);
                RenderSettings.sun.intensity = Mathf.MoveTowards(sunIntenstyStart, SunIntensity, percent);
                RenderSettings.sun.color     = Color.Lerp(sunLightColorStart, SunLightColor, percent);
                RenderSettings.fogColor      = Color.Lerp(fogColorStart, FogColor, percent);
                RenderSettings.sun.transform.rotation = Quaternion.Slerp(sunStartRotation, targetRotation, percent);
                SkyboxMaterial.SetFloat(_SunAngle, Mathf.MoveTowards(sunAngleStart, sunAngle, percent));
                yield return new WaitForEndOfFrame();
            }
        }

        private void ImmediateActions()
        {
            EnsureSun();
            // fog yoktu yeni oluşmaya başlıyor anlamına geliyor
            if (fog && !RenderSettings.fog) RenderSettings.fogDensity = 0;

            RenderSettings.fog = fog;
            SSMS.SSMSGlobalFog.instance.enabled = fog;  
            RenderSettings.sun.gameObject.SetActive(true); //.SetActive(!IsNight); you can disable sun at nights
            // todo change sun to the moon if needed

            for (int i = 0; i < WeatherManager.instance.PointLights.Length; i++)
                WeatherManager.instance.PointLights[i].enabled = IsNight;
            
            foreach (SkyboxProperty property in WeatherManager.instance.skyboxProperties) // material properties
            {
                if (property.isWindow){
                    property.material.SetColor("_Color", IsNight ? WeatherManager.instance.GlobalGlowColor * property.glowMultipler : RenderSettings.sun.color / 8);
                    continue;
                }
                if (property.SpecularAtMorning && !IsNight) property.material.EnableKeyword("SPECULAR");
                else                                        property.material.DisableKeyword("SPECULAR");

                if (property.material.passCount > 1)
                    property.material.SetShaderPassEnabled("Point", property.PointLightAtNight && IsNight);
                
                property.material.SetFloat("_Ambient", IsNight ? .35f : property.MorningAmbient);
            }

            if (Application.isPlaying && IsNight)
                for (int i = 0; i < NpcDatabase.npcControllers.Count; i++)
                    if (NpcDatabase.npcControllers[i]){
                        NpcDatabase.npcControllers[i].StopAllCoroutines();
                        //NpcDatabase.npcControllers[i].GoToHome();
                    }

            StarRendering.instance.CanUpdate = IsNight;
            GrassHandeller.instance.ChangeColors(IsNight);

            // todo: add clouds
            if (Clouds){

            }
        }
    }
}