using System;
using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace SeleneGame.Core {

    [ExecuteAlways]
    [DefaultExecutionOrder(-1000)]
    public class WaterManager : Singleton<WaterManager> {

        public static Vector4 defaultWave = new Vector4(0, 0, 0, 1);
        
        private static List<IWaterDisplaceable> waterDisplaceables = new List<IWaterDisplaceable>();
        
        private GraphicsBuffer gerstnerWavesBuffer;
        private ComputeBuffer samplePositionBuffer;
        private ComputeBuffer waveHeightBuffer;
        private Vector3[] samplePositions = new Vector3[0];
        // private float[] waveHeights = new float[0];

        [SerializeField] private ComputeShader waterDisplacementShader;
        [SerializeField] private WaterProfile _waterProfile;
        [SerializeField] private List<Vector4> _waves = new List<Vector4>();
        

        public WaterProfile waterProfile {
            get => _waterProfile;
        }

        public List<Vector4> waves {
            get => _waves;
        }


        

        public void TransitionToWaterProfile(WaterProfile waterProfile) {
            
            if (waterProfile == null) return;
            
            _waterProfile = waterProfile;
        }


        private void AllocateWaveHeightSampleBuffers() {
            ReleaseWaveHeightSampleBuffers();
            
            int count = waterDisplaceables.Count;
            if (count == 0) return;

            samplePositionBuffer = new ComputeBuffer(count, sizeof(float) * 3);
            waveHeightBuffer = new ComputeBuffer(count, sizeof(float));

            samplePositions = new Vector3[count];
        }

        private void ReleaseWaveHeightSampleBuffers() {
            samplePositionBuffer?.Release();
            waveHeightBuffer?.Release();
        }

        private void ReleaseWaveDataBuffer() {
            gerstnerWavesBuffer?.Release();
        }

        private void UpdateWaveDataBuffer() {
            ReleaseWaveDataBuffer();

            int count = _waves.Count;
            if (count == 0) return;

            gerstnerWavesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 4);

            gerstnerWavesBuffer?.SetData(_waves);
            Shader.SetGlobalBuffer("_GerstnerWaves", gerstnerWavesBuffer);
        }


        public static void AddWaterDisplaceable(IWaterDisplaceable displaceable) {
            if (waterDisplaceables.Contains(displaceable)) return;

            waterDisplaceables.Add(displaceable);

            current?.AllocateWaveHeightSampleBuffers();
        }

        public static void RemoveWaterDisplaceable(IWaterDisplaceable displaceable) {
            if (!waterDisplaceables.Contains(displaceable)) return;

            waterDisplaceables.Remove(displaceable);

            current?.AllocateWaveHeightSampleBuffers();
        }

        private void WaterProfileFadeIn() {
            
            int sharedCount = Math.Min(_waves.Count, waterProfile.waves.Length);
            int maxCount = Math.Max(_waves.Count, waterProfile.waves.Length);

            int difference = _waves.Count - waterProfile.waves.Length;

            // Add to current waves if there are more in the new profile
            while (_waves.Count < waterProfile.waves.Length) {
                Vector4 addedWave = waterProfile.waves[_waves.Count];
                addedWave.z = 0f;
                _waves.Add(addedWave);
            }

            // Update the waves
            for ( int i = 0; i < _waves.Count; i++ ) {
                if (i < sharedCount) {
                    Vector4 updatedWave = Vector4.MoveTowards(_waves[i], waterProfile.waves[i], 0.1f * GameUtility.timeDelta);
                    _waves[i] = updatedWave;
                } else {
                    Vector4 updatedWave = _waves[i];
                    updatedWave.z = Mathf.MoveTowards(updatedWave.z, 0f, 0.1f * GameUtility.timeDelta);
                    _waves[i] = updatedWave;
                }
            }

            // Remove from current waves if there are less in the new profile AND the last wave in the current profile is zero
            for (int i = waterProfile.waves.Length; i < _waves.Count; i++) {
                if (_waves[i].z == 0f) {
                    _waves.RemoveAt(i);
                    i--;
                }
            }
        }

        private void CalculateWaveHeight() {

            if (waterDisplaceables.Count == 0) return;


            for (int i = 0; i < waterDisplaceables.Count; i++) {
                samplePositions[i] = waterDisplaceables[i].position;
            }

            samplePositionBuffer.SetData(samplePositions);

            waterDisplacementShader.SetFloat("_Time", Time.time);
            waterDisplacementShader.SetBuffer(0, "samplePositions", samplePositionBuffer);
            waterDisplacementShader.SetBuffer(0, "waveHeights", waveHeightBuffer);

            waterDisplacementShader.Dispatch(0, waterDisplaceables.Count, 1, 1);


            if (waveHeightBuffer == null) return;

            // TODO: USE THIS FOR HIGHER END DEVICES/HIGH QUALITY SETTINGS
            // Wait for the wave height data to be available on the CPU
            // This causes CPU to wait, lowering framerate but is synchronous with visuals
            /* float[] waveHeights = new float[waterDisplaceables.Count];
            waveHeightBuffer.GetData(waveHeights);

            for (int i = 0; i < count; i++) {
                IWaterDisplaceable waterDisplaceable = waterDisplaceables[i];
                Debug.Log($"{waterDisplaceable} wave height: {waveHeights[i]}");
                waterDisplaceable.waveHeight = waveHeights[i];
            } */



            // TODO: USE THIS FOR LOWER END DEVICES/LOW QUALITY SETTINGS
            // Request the wave height data from the GPU and don't wait for it to finish
            // This causes slight delay with visuals but is non-synchronous with CPU
            IWaterDisplaceable[] bufferDisplaceables = new IWaterDisplaceable[waterDisplaceables.Count];
            waterDisplaceables.CopyTo(bufferDisplaceables);

            AsyncGPUReadback.Request(waveHeightBuffer, 
                (request) => {
                    
                    if (!Application.isPlaying) return;

                    if (request.hasError) {
                        Debug.LogError("Error reading back wave height buffer");
                        return;
                    }

                    float[] waveHeights = request.GetData<float>().ToArray();
                    for (int i = 0; i < bufferDisplaceables.Length; i++) {
                        IWaterDisplaceable waterDisplaceable = bufferDisplaceables[i];
                        if (waterDisplaceable != null)
                            waterDisplaceable.waveHeight = waveHeights[i];
                    }
                        
                }
            );
        }

        private void OnEnable() {

            SetCurrent();

            AllocateWaveHeightSampleBuffers();

            UpdateWaveDataBuffer();

        }

        private void OnDisable() {
            ReleaseWaveHeightSampleBuffers();
            
            ReleaseWaveDataBuffer(); 
        }

        // Executed in edit mode, this clears the buffer after/before the scene is rendered, preventing memory leaks
        private void Update() {
            #if UNITY_EDITOR
                ReleaseWaveDataBuffer();
            #endif
            WaterProfileFadeIn();
        }

        private void LateUpdate() {
            #if UNITY_EDITOR
                UpdateWaveDataBuffer();
            #endif
            CalculateWaveHeight();
        }

        private void OnValidate() {
            if (_waves.ToArray() != waterProfile.waves) {
                if (!Application.isPlaying) {
                    _waves.Clear();
                    _waves.AddRange(waterProfile.waves);
                } else {
                    TransitionToWaterProfile(waterProfile);
                }
            }
        }
    }
}
