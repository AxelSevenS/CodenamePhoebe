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

        public void PlayerHitStop(float damage) {
            if (hitStopCoroutine != null)
                StopCoroutine(hitStopCoroutine);

            hitStopCoroutine = StartCoroutine(HitStop(damage * 5f));
        }

        public void EnemyHitStop(float damage) {
            if (hitStopCoroutine != null)
                StopCoroutine(hitStopCoroutine);

            hitStopCoroutine = StartCoroutine(HitStop(damage));
        }

        private IEnumerator HitStop(float damage) {
            Time.timeScale = 0f;
            float time = Mathf.Pow(damage, 0.3f) * 0.5f;
            yield return new WaitForSecondsRealtime(0.1f * time);
            Time.timeScale = 1f;
        }
    }
}