using System;
using System.Collections;
using System.Collections.Generic;
using SevenGame.Utility;
using UnityEngine;

namespace SeleneGame.Core {

    public class Health : MonoBehaviour {

        [Tooltip("The current health.")]
        public float _maxAmount = 100f;
        [SerializeField] private float _amount;

        [Tooltip("The health, before it took damage. Slowly moves toward the true health.")]
        [SerializeField] private float _damagedHealth;

        [SerializeField] private TimeInterval _damagedHealthTimer = new();
        [SerializeField] private float _damagedHealthVelocity = 0f;


        public event Action<float> OnUpdate;



        public float MaxAmount {
            get => _maxAmount;
            set {
                _maxAmount = Mathf.Max(value, 1f);
                _damagedHealth = Mathf.Min(_damagedHealth, _maxAmount);
            }
        }

        public float Amount {
            get => _amount;
            set {
                _amount = Mathf.Clamp(value, 0f, _maxAmount);

                const float damagedHealthDuration = 1.25f;
                _damagedHealthTimer.SetDuration(damagedHealthDuration);

                OnUpdate?.Invoke(_amount);
            }
        }


        private void Awake() {
            _amount = _maxAmount;
            _damagedHealth = _amount;
        }

        private void Update() {

            if ( _damagedHealthTimer.isDone )
                _damagedHealth = Mathf.SmoothDamp(_damagedHealth, _amount, ref _damagedHealthVelocity, 0.2f);
            else {
                _damagedHealthVelocity = 0f;
            }
        
        }
    }
}
