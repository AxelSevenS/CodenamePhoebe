using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class WeatherManager : Singleton<WeatherManager> {
        
        [SerializeField] private GameObject rainEffects;
        [SerializeField] private GameObject snowEffects;
        [SerializeField] private Light sun;
        [SerializeField] private Light moon;

        private ParticleSystem[] rainParticles;
        private ParticleSystem[] snowParticles;

        [Space(15f)]

        [Header("Sun and Moon")]

        public float sunLight = 10000f;
        public Vector2 sunSpeed;
        public Quaternion sunRotation;
        public float lightLevel = 1f;
        public Vector2 moonSpeed;
        public Quaternion moonRotation;

        [SerializeField] [ColorUsage(false, true)] private Color _ambientLight = new Color(1f/199f, 0, 1f/57f);


        [Space(15f)]

        [Header("Weather Properties")]

        [SerializeField] private Vector3 _windDirection = new Vector3(1f,0,1f);
        [HideInInspector] public Vector3 windDirection = new Vector3(1f,0,1f);

        [Range(0, 2f)] public float snowAmount = 0f;
        public bool precipitation = false;
        public bool snow = false;



        public Vector3 sunForward => sun.transform.forward;
        public Vector3 moonForward => moon.transform.forward;




        private void SetGlobals() {
            RenderSettings.ambientLight = _ambientLight;
            RenderSettings.fogColor = _ambientLight;
            Shader.SetGlobalVector("_WindDirection", new Vector4(windDirection.x, windDirection.y, windDirection.z, 0));
            Shader.SetGlobalFloat("_SnowAmount", snowAmount);
        }


        private void Awake(){
            rainParticles = rainEffects.GetComponentsInChildren<ParticleSystem>();
            snowParticles = snowEffects.GetComponentsInChildren<ParticleSystem>();
        }

        private void Start(){
            sunSpeed = new Vector3(0.8f, .15f);
            moonSpeed  = new Vector3(0, .15f);

            sunRotation = Quaternion.identity;
            moonRotation = Quaternion.Euler(35f, -15f, 0);
        }
        
        private void OnEnable() {
            SetCurrent();
        }

        private void Update() {
            windDirection = Vector3.Slerp(windDirection, _windDirection, 2f * GameUtility.timeDelta).normalized;
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, _ambientLight, 5f * GameUtility.timeDelta);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, _ambientLight, 5f * GameUtility.timeDelta);

            snowAmount = Mathf.MoveTowards(snowAmount, (snow && precipitation) ? 2f : 0f, Mathf.Pow(2f, Mathf.Min(snowAmount, 0.05f)) * 0.01f * GameUtility.timeDelta);

            SetGlobals();

            // Weather
            float newLightLevel = precipitation ? .85f : 1f;
            lightLevel = Mathf.MoveTowards(lightLevel, newLightLevel * sunLight, GameUtility.timeDelta);

            foreach (ParticleSystem particle in snowParticles) {
                var emission = particle.emission;
                emission.enabled = precipitation && snow;
            }

            foreach (ParticleSystem particle in rainParticles) {
                var emission = particle.emission;
                emission.enabled = precipitation && !snow;
            }


            // Sun and Moon
            // Vector3 deltaRotation = new Vector3(sunSpeed.x, sunSpeed.y, 0) * GameUtility.timeDelta;
            // sunRotation *= Quaternion.Euler(deltaRotation);

            // sun.transform.rotation = sunRotation;
            // moon.transform.rotation = moonRotation;



        }

        private void OnValidate(){
            if ( !Application.isPlaying ) {
                windDirection = _windDirection.normalized;
                RenderSettings.ambientLight = _ambientLight;
                RenderSettings.fogColor = _ambientLight;
            }
            SetGlobals();
        }

    }
}