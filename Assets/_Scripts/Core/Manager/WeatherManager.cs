using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class WeatherManager : Singleton<WeatherManager> {

        private static List<IWaterDisplaceable> waterDisplaceables = new List<IWaterDisplaceable>();
        
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

        [SerializeField] private ComputeShader waterDisplacementShader;
        // private static ComputeBuffer _waveBuffer = null;



        public Vector3 sunForward => sun.transform.forward;
        public Vector3 moonForward => moon.transform.forward;



        public static void AddWaterDisplaceable(IWaterDisplaceable displaceable){
            if ( waterDisplaceables.Contains(displaceable) ) return;
            
            waterDisplaceables.Add(displaceable);
            // AllocateWaveBuffer();
        }

        public static void RemoveWaterDisplaceable(IWaterDisplaceable displaceable){
            if ( !waterDisplaceables.Contains(displaceable) ) return;
            
            waterDisplaceables.Remove(displaceable);
            // AllocateWaveBuffer();
        }

        // private static void AllocateWaveBuffer() {
        //     if (waterDisplaceables.Count == 0)
        //         _waveBuffer = null;
        //     else
        //         _waveBuffer = new ComputeBuffer(waterDisplaceables.Count, WaveData.size);
        // }



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

        private void Update()
        {
            windDirection = Vector3.Slerp(windDirection, _windDirection, 2f * GameUtility.timeDelta).normalized;
            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, _ambientLight, 5f * GameUtility.timeDelta);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, _ambientLight, 5f * GameUtility.timeDelta);

            snowAmount = Mathf.MoveTowards(snowAmount, (snow && precipitation) ? 2f : 0f, Mathf.Pow(2f, Mathf.Min(snowAmount, 0.05f)) * 0.01f * GameUtility.timeDelta);

            SetGlobals();

            // Weather
            if (precipitation)
            {
                lightLevel = Mathf.MoveTowards(lightLevel, .85f * sunLight, GameUtility.timeDelta);
            }
            else
            {
                lightLevel = Mathf.MoveTowards(lightLevel, 1f * sunLight, GameUtility.timeDelta);
            }

            foreach (ParticleSystem particle in snowParticles)
            {
                var emission = particle.emission;
                emission.enabled = precipitation && snow;
            }

            foreach (ParticleSystem particle in rainParticles)
            {
                var emission = particle.emission;
                emission.enabled = precipitation && !snow;
            }


            // Sun and Moon
            Vector3 deltaRotation = new Vector3(sunSpeed.x, sunSpeed.y, 0) * GameUtility.timeDelta;
            sunRotation *= Quaternion.Euler(deltaRotation);

            sun.transform.rotation = sunRotation;
            moon.transform.rotation = moonRotation;


            // Water Displacement

            if (waterDisplaceables.Count != 0)
                CalculateWaveHeight();

        }

        private void CalculateWaveHeight() {
            ComputeBuffer waveDataBuffer = new ComputeBuffer(waterDisplaceables.Count, WaveData.size);
            ComputeBuffer waveHeightBuffer = new ComputeBuffer(waterDisplaceables.Count, sizeof(float));

            WaveData[] waveData = new WaveData[waterDisplaceables.Count];
            foreach (IWaterDisplaceable waterDisplaceable in waterDisplaceables) {
                WaveData wave = new WaveData(waterDisplaceable.position, waterDisplaceable.waveStrength, waterDisplaceable.waveSpeed, waterDisplaceable.waveFrequency);
                waveData[waterDisplaceables.IndexOf(waterDisplaceable)] = wave;
            }
            waveDataBuffer.SetData(waveData);


            waterDisplacementShader.SetFloat("_Time", Time.time);
            waterDisplacementShader.SetBuffer(0, "waveData", waveDataBuffer);
            waterDisplacementShader.SetBuffer(0, "waveHeight", waveHeightBuffer);

            waterDisplacementShader.Dispatch(0, waterDisplaceables.Count, 1, 1);

            float[] waveHeights = new float[waterDisplaceables.Count];
            waveHeightBuffer.GetData(waveHeights);

            foreach (IWaterDisplaceable waterDisplaceable in waterDisplaceables) {
                waterDisplaceable.waveHeight = waveHeights[waterDisplaceables.IndexOf(waterDisplaceable)];
            }
            waveHeightBuffer.Release();

            waveDataBuffer.Release();
        }



        private void SetGlobals() {
            RenderSettings.ambientLight = _ambientLight;
            RenderSettings.fogColor = _ambientLight;
            Shader.SetGlobalVector("_WindDirection", new Vector4(windDirection.x, windDirection.y, windDirection.z, 0));
            Shader.SetGlobalFloat("_SnowAmount", snowAmount);
        }

        private void OnValidate(){
            SetGlobals();
        }



        
        public struct WaveData {
            readonly public Vector3 position;
            readonly public float waveStrength;
            readonly public float waveSpeed;
            readonly public float waveFrequency;

            public WaveData(Vector3 position, float waveStrength, float waveSpeed, float waveFrequency){
                this.position = position;
                this.waveStrength = waveStrength;
                this.waveSpeed = waveSpeed;
                this.waveFrequency = waveFrequency;
            }

            public static int size = sizeof(float) * 3 + sizeof(float) * 3;
        }
    }
}