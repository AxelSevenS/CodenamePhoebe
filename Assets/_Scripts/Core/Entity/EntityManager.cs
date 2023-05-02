using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [DefaultExecutionOrder(-1000)]
    public class EntityManager : Singleton<EntityManager> {

        public AnimationCurve evadeCurve;
        
        public RuntimeAnimatorController entityAnimatorController;
        
        public List<Entity> entityList = new List<Entity>();

        Coroutine hitStopCoroutine;

        // public Dictionary<string, string> entityCostumes = new Dictionary<string, string>();
        // public Dictionary<string, string> weaponCostumes = new Dictionary<string, string>();

        public float stepHeight = 0.35f;

        private void OnEnable() {
            SetCurrent();
        }

        public void HardHitStop() {
            if (hitStopCoroutine != null)
                StopCoroutine(hitStopCoroutine);

            hitStopCoroutine = StartCoroutine(HitStop(0.2f));
        }

        public void SoftHitStop() {
            if (hitStopCoroutine != null)
                StopCoroutine(hitStopCoroutine);

            hitStopCoroutine = StartCoroutine(HitStop(0.1f));
        }

        private IEnumerator HitStop(float time) {
            float oldTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(time);
            
            if (Time.timeScale == 0f){
                Time.timeScale = oldTimeScale;
            }
        }
    }
}