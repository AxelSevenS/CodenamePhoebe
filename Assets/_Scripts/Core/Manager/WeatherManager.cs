using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public class WeatherManager : ObjectFollower{

        public static WeatherManager current;

        [Space(15)]
        
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
        public Vector3 sunForward => sun.transform.forward;
        public Vector3 moonForward => moon.transform.forward;

        [SerializeField] [ColorUsage(true, true)] private Color _ambientLight = new Color(1f/199f, 0, 1f/57f);
        [HideInInspector] [ColorUsage(true, true)] public Color ambientLight = new Color(1f/199f, 0, 1f/57f);


        [Space(15f)]

        [Header("Weather Properties")]

        [SerializeField] private Vector3 _windDirection = new Vector3(1f,0,1f);
        [HideInInspector] public Vector3 windDirection = new Vector3(1f,0,1f);

        [Range(0, 2f)]public float snowAmount = 0f;
        public bool precipitation = false;
        public bool snow = false;

        [Space(15f)]

        [Header("Water Properties")]
        public Color waterShallowColor;
        public Color waterDeepColor;
        public Texture2D waterNoiseMap;
        public Texture2D waterNormalMap;
        [Range(0, 1)] public float waterShininess;
        [Range(0, 1)] public float waterSmoothness;
        [Range(0, 1)] public float waterNormalIntensity = 0.47f;

        [Space(15f)]
        [Range(0, 1)] public float waterWaveStrength;
        [Range(0, 1)] public float waterWaveSpeed;
        [Range(0, 1)] public float waterWaveFrequency;
        [Range(0, 0.1f)] public float waterNoiseScale;
        [Range(0, 1)] public float waterNoiseSpeed;
        [Range(0, 10)] public float waterNoiseStrength;
        public float waterFoamDistance;
        public bool waterIsPlane = true;

        void OnEnable(){
            if (current != null)
                Destroy(current);
            current = this;
        }

        void Awake(){
            rainParticles = rainEffects.GetComponentsInChildren<ParticleSystem>();
            snowParticles = snowEffects.GetComponentsInChildren<ParticleSystem>();
        }

        void Start(){
            sunSpeed = new Vector3(.85f, .15f);
            moonSpeed  = new Vector3(0, .15f);

            sunRotation = Quaternion.identity;
            moonRotation = Quaternion.Euler(35f, -15f, 0);
        }

        // Update is called once per frame

        #if UNITY_EDITOR
        void OnValidate(){
            ambientLight = _ambientLight;
            SetGlobals();
        }
        #endif

        private void SetGlobals(){
            Shader.SetGlobalColor("_AmbientLight", new Color(ambientLight.r, ambientLight.g, ambientLight.b, ambientLight.a));
            Shader.SetGlobalVector("_WindDirection", new Vector4(windDirection.x, windDirection.y, windDirection.z, 0));
            Shader.SetGlobalFloat("_SnowAmount", snowAmount);

            Shader.SetGlobalColor("_WaterColorShallow", waterShallowColor);
            Shader.SetGlobalColor("_WaterColorDeep", waterDeepColor);
            Shader.SetGlobalTexture("_WaterNormalMap", waterNormalMap);
            Shader.SetGlobalTexture("_WaterNoiseMap", waterNoiseMap);
            Shader.SetGlobalFloat("_WaterShininess", waterShininess);
            Shader.SetGlobalFloat("_WaterSmoothness", waterSmoothness);
            Shader.SetGlobalFloat("_WaterNormalIntensity", waterNormalIntensity);
            Shader.SetGlobalFloat("_WaterWaveStrength", waterWaveStrength);
            Shader.SetGlobalFloat("_WaterWaveSpeed", waterWaveSpeed);
            Shader.SetGlobalFloat("_WaterWaveFrequency", waterWaveFrequency);
            Shader.SetGlobalFloat("_WaterNoiseStrength", waterNoiseStrength);
            Shader.SetGlobalFloat("_WaterNoiseSpeed", waterNoiseSpeed);
            Shader.SetGlobalFloat("_WaterNoiseScale", waterNoiseScale);
            Shader.SetGlobalInt ("_WaterCull", 2);
        }

        void Update(){
            windDirection = Vector3.Slerp(windDirection, _windDirection, 2f * Global.timeDelta).normalized;
            ambientLight = Color.Lerp(ambientLight, _ambientLight, 5f * Global.timeDelta);
            snowAmount = Mathf.MoveTowards(snowAmount, System.Convert.ToSingle(snow && precipitation) * 2f, Mathf.Pow( 2f, Mathf.Min(snowAmount, 0.05f) ) * 0.01f * Global.timeDelta);

            SetGlobals();

            if(precipitation){
                lightLevel = Mathf.MoveTowards(lightLevel, .85f*sunLight, Global.timeDelta);
            }else{
                lightLevel = Mathf.MoveTowards(lightLevel, 1f*sunLight, Global.timeDelta);
            }

            foreach (ParticleSystem particle in snowParticles){
                particle.enableEmission = precipitation && snow;
            }

            foreach (ParticleSystem particle in rainParticles){
                particle.enableEmission = precipitation && !snow;
            }

            Vector3 deltaRotation = new Vector3(sunSpeed.x, sunSpeed.y, 0)*Global.timeDelta;
            sunRotation *= Quaternion.Euler(deltaRotation);

            // deltaRotation = new Vector3(moonSpeed.x, moonSpeed.y, 0)*Global.timeDelta;
            // moonRotation *= Quaternion.Euler(deltaRotation);

            // sun.intensity = lightLevel;
            sun.transform.rotation = sunRotation;
            moon.transform.rotation = moonRotation;

            FollowObject();
            
        }
    }
}