using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace SeleneGame.Core {

    [ExecuteAlways]
    [DefaultExecutionOrder(-1000)]
    public class WaterManager : Singleton<WaterManager> {
        
        private static List<IWaterDisplaceable> waterDisplaceables = new List<IWaterDisplaceable>();
        
        private GraphicsBuffer gerstnerWavesBuffer;
        private ComputeBuffer samplePositionBuffer;
        private ComputeBuffer waveHeightBuffer;
        private Vector3[] samplePositions = new Vector3[0];
        // private float[] waveHeights = new float[0];

        [SerializeField] private ComputeShader waterDisplacementShader;
        [SerializeField] private Vector4[] waves = new Vector4[0];
        



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

            int count = waves.Length;
            if (count == 0) return;

            gerstnerWavesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 4);

            gerstnerWavesBuffer?.SetData(waves);
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
            // This causes slight delay with visuals but is asynchronous with CPU
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
        }

        private void LateUpdate() {
            #if UNITY_EDITOR
                UpdateWaveDataBuffer();
            #endif
            CalculateWaveHeight();
        }

        [ContextMenu("Randomize Waves")]
        private void RandomizeWaves() {
            for (int i = 0; i < waves.Length; i++) {
                Vector2 direction = Random.insideUnitCircle.normalized;
                waves[i] = new Vector4(direction.x, direction.y, Random.value * 0.075f, Random.value * 40f + 2f);
            }
        }
    }
}
