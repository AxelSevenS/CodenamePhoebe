using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public class ArmedEntity : Entity {
        
        public WeaponInventory weapons;


        [Header("Parrying")]
        public float parryTimer;
        public event Action onParry;


        [Header("Evading")]
        public BoolData evading;
        public Vector3 evadeDirection = Vector3.forward;
        public float evadeTimer;
        public float evadeCount = 1f;
        public event Action<Vector3> onEvade;

        public override bool CanTurn() => !evading;

        protected override void EntityAwake(){
            base.EntityAwake();
            
            evading = new BoolData();
            weapons = new WeaponInventory(this, 1);
        }

        protected override void EntityUpdate(){
            evading.SetVal( evadeTimer > data.evadeCooldown );
            
            evadeTimer = Mathf.MoveTowards( evadeTimer, 0f, Global.timeDelta );
            parryTimer = Mathf.MoveTowards( parryTimer, 0f, Global.timeDelta );

            bool lightActuated = lightAttackInput.trueTimer < 0.15f && lightAttackInput && heavyAttackInput.started;
            bool heavyActuated = heavyAttackInput.trueTimer < 0.15f && heavyAttackInput && lightAttackInput.started;
            if (lightActuated || heavyActuated)
                Parry();
            
            foreach (Weapon weapon in weapons)
                weapon?.WeaponUpdate();
        }

        protected override void EntityFixedUpdate() {
            if ( moveDirection.magnitude > 0f && evadeTimer > data.totalEvadeDuration - 0.15f )
                evadeDirection = moveDirection.normalized;

            foreach (Weapon weapon in weapons)
                weapon?.WeaponFixedUpdate();
        }

        // protected override void EntityLoadModel() {
        // }

        public void Parry(){
            Debug.Log("Parry");
            parryTimer = 0.15f;

            onParry?.Invoke();
        }


        public bool EvadeUpdate(out float evadeTime, out float evadeCurve){
            if ( !evading ) {
                evadeTime = 0f;
                evadeCurve = 0f;
                return false;
            }

            evadeTime = Mathf.Clamp01( 1 - ( (evadeTimer - data.evadeCooldown) / data.evadeDuration ) );
            evadeCurve = Mathf.Clamp01( data.evadeCurve.Evaluate( evadeTime ) );
            return true;
        }

        public void GroundedEvade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;
            
            Vector3 newVelocity = rb.velocity.NullifyInDirection( gravityDown );
            if (!onGround){
                newVelocity += -gravityDown.normalized * 5f;
            }
            rb.velocity = newVelocity;

            Evade(evadeDirection);
        }
        public void Evade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;

            this.evadeDirection = evadeDirection;
            
            AnimatorTrigger("Evade");
            evadeTimer = data.totalEvadeDuration;

            onEvade?.Invoke(evadeDirection);
        }

    }
}