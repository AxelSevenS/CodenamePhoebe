using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [DefaultExecutionOrder(-1000)]
    public class WeatherManager : Singleton<WeatherManager> {

        const float COLD_THRESHOLD = 0f;


        
        [SerializeField] private GameObject rainEffects;
        [SerializeField] private GameObject snowEffects;
        [SerializeField] private Light sun;
        [SerializeField] private Light moon;

        private ParticleSystem[] rainParticles;
        private ParticleSystem[] snowParticles;

        [Space(15f)]

        [SerializeField] private WeatherProfile _currentWeather;

        // [Header("Sun and Moon")]
        // public Vector2 sunSpeed;
        // public Quaternion sunRotation;
        // public Vector2 moonSpeed;
        // public Quaternion moonRotation;


        [Space(15f)]

        public float lightLevel = 1f;
        public float sunLight = 1.25f;
        public float moonLight = 0.1f;

        [ColorUsage(false, true)] public Color ambientLight = new Color(1f/199f, 0, 1f/57f);

        public bool precipitation = false;

        public float temperature = 20f;
        [Range(0, 2f)] public float snowAmount = 0f;

        public float windSpeed = 1f;
        [NormalVector] public Vector3 windDirection = new Vector3(1f,0,1f);



        public Vector3 sunForward => sun.transform.forward;
        public Vector3 moonForward => moon.transform.forward;
        public bool cold => temperature <= COLD_THRESHOLD;




        private void SetGlobals() {
            sun.intensity = sunLight * lightLevel;
            moon.intensity = moonLight * lightLevel;
            RenderSettings.sun = sun;
            RenderSettings.ambientLight = ambientLight;
            RenderSettings.fogColor = ambientLight;
            Shader.SetGlobalFloat("_SnowAmount", snowAmount);
            Shader.SetGlobalVector("_WindDirection", windDirection);
        }


        private void Awake() {
            rainParticles = rainEffects.GetComponentsInChildren<ParticleSystem>();
            snowParticles = snowEffects.GetComponentsInChildren<ParticleSystem>();
        }
        
        private void OnEnable() {
            SetCurrent();
        }

        private void Update() {

            temperature = Mathf.MoveTowards(temperature, _currentWeather.temperature, 0.1f * GameUtility.timeDelta);

            bool finalPrecipitation = _currentWeather.forcedPrecipitation || precipitation;
            bool rain = finalPrecipitation && !cold;
            bool snow = finalPrecipitation && cold;

            foreach (ParticleSystem particle in snowParticles) {
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = snow;
            }

            foreach (ParticleSystem particle in rainParticles) {
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = rain;
            }

            snowAmount = Mathf.MoveTowards(snowAmount, snow ? 2f : 0f, Mathf.Pow(2f, Mathf.Min(snowAmount, 0.05f)) * 0.01f * GameUtility.timeDelta);


            // Weather
            float newLightLevel = _currentWeather.lightLevel * (rain ? 0.85f : snow ? 0.95f : 1f);
            lightLevel = Mathf.MoveTowards(lightLevel, newLightLevel, GameUtility.timeDelta);

            sunLight = Mathf.MoveTowards(sunLight, _currentWeather.sunLight, GameUtility.timeDelta);
            moonLight = Mathf.MoveTowards(moonLight, _currentWeather.moonLight, GameUtility.timeDelta);

            
            ambientLight = Color.Lerp(ambientLight, _currentWeather.ambientLight, 5f * GameUtility.timeDelta);
            windSpeed = Mathf.MoveTowards(windSpeed, _currentWeather.windSpeed, 5f * GameUtility.timeDelta);
            windDirection = Vector3.Slerp(windDirection, _currentWeather.windDirection, 2f * GameUtility.timeDelta).normalized;


            SetGlobals();


            // Sun and Moon
            // Vector3 deltaRotation = new Vector3(sunSpeed.x, sunSpeed.y, 0) * GameUtility.timeDelta;
            // sunRotation *= Quaternion.Euler(deltaRotation);

            // sun.transform.rotation = sunRotation;
            // moon.transform.rotation = moonRotation;



        }

        private void OnValidate(){
            SetGlobals();
        }

    }
}