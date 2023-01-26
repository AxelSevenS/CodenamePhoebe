using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class WeatherManager : Singleton<WeatherManager>{
        
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
        public Vector2 moonSpeed;
        public float lightLevel = 1f;
        public Quaternion sunRotation;
        public Quaternion moonRotation;

        [SerializeField] [ColorUsage(true, true)] private Color _ambientLight = new Color(1f/199f, 0, 1f/57f);
        // [HideInInspector] [ColorUsage(true, true)] public Color ambientLight = new Color(1f/199f, 0, 1f/57f);

        // [SerializeField] [Range(0f, 1f)] private float _ambientStrength = 0.025f;
        // [HideInInspector] public float ambientStrength = 0.025f;


        [Space(15f)]

        [Header("Weather Properties")]

        [SerializeField] private Vector3 _windDirection = new Vector3(1f,0,1f);
        [HideInInspector] public Vector3 windDirection = new Vector3(1f,0,1f);

        [Range(0, 2f)]public float snowAmount = 0f;
        public bool precipitation = false;
        public bool snow = false;


        public Vector3 sunForward => sun.transform.forward;
        public Vector3 moonForward => moon.transform.forward;

        private void Awake(){
            rainParticles = rainEffects.GetComponentsInChildren<ParticleSystem>();
            snowParticles = snowEffects.GetComponentsInChildren<ParticleSystem>();
        }

        private void Start(){
            sunSpeed = new Vector3(.85f, .15f);
            moonSpeed  = new Vector3(0, .15f);

            sunRotation = Quaternion.identity;
            moonRotation = Quaternion.Euler(35f, -15f, 0);
        }
        
        private void OnEnable() {
            SetCurrent();
        }

        private void Update(){
            windDirection = Vector3.Slerp(windDirection, _windDirection, 2f * GameUtility.timeDelta).normalized;
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, _ambientLight, 5f * GameUtility.timeDelta);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, _ambientLight, 5f * GameUtility.timeDelta);
            
            snowAmount = Mathf.MoveTowards(snowAmount, System.Convert.ToSingle(snow && precipitation) * 2f, Mathf.Pow( 2f, Mathf.Min(snowAmount, 0.05f) ) * 0.01f * GameUtility.timeDelta);

            SetGlobals();

            if(precipitation){
                lightLevel = Mathf.MoveTowards(lightLevel, .85f*sunLight, GameUtility.timeDelta);
            }else{
                lightLevel = Mathf.MoveTowards(lightLevel, 1f*sunLight, GameUtility.timeDelta);
            }

            foreach (ParticleSystem particle in snowParticles){
                particle.enableEmission = precipitation && snow;
            }

            foreach (ParticleSystem particle in rainParticles){
                particle.enableEmission = precipitation && !snow;
            }

            Vector3 deltaRotation = new Vector3(sunSpeed.x, sunSpeed.y, 0)*GameUtility.timeDelta;
            sunRotation *= Quaternion.Euler(deltaRotation);

            // deltaRotation = new Vector3(moonSpeed.x, moonSpeed.y, 0)*GameUtility.timeDelta;
            // moonRotation *= Quaternion.Euler(deltaRotation);

            // sun.intensity = lightLevel;
            sun.transform.rotation = sunRotation;
            moon.transform.rotation = moonRotation;

            // FollowObject();
            
        }

        #if UNITY_EDITOR
        private void OnValidate(){
            RenderSettings.ambientLight = _ambientLight;
            RenderSettings.fogColor = _ambientLight;
            SetGlobals();
        }
        #endif



        private void SetGlobals(){
            // Shader.SetGlobalColor("_AmbientLight", ambientLight);
            // Shader.SetGlobalFloat("_AmbientStrength", ambientStrength);
            Shader.SetGlobalVector("_WindDirection", new Vector4(windDirection.x, windDirection.y, windDirection.z, 0));
            Shader.SetGlobalFloat("_SnowAmount", snowAmount);
        }

    }
}