using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class ArmedEntity : Entity {
        
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

        protected override void Awake(){
            base.Awake();

            // weapons = new WeaponInventory(this, 1);
        }

        protected override void Update(){
            base.Update();

            evading.SetVal( evadeTimer > data.evadeCooldown );
            
            evadeTimer = Mathf.MoveTowards( evadeTimer, 0f, GameUtility.timeDelta );
            parryTimer = Mathf.MoveTowards( parryTimer, 0f, GameUtility.timeDelta );


            if ( KeyInputData.SimultaneousTap( lightAttackInput, heavyAttackInput ) )
                Parry();
            
            foreach (Weapon weapon in weapons)
                weapon?.WeaponUpdate();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            if ( moveDirection.magnitude > 0f && evadeTimer > data.totalEvadeDuration - 0.15f )
                evadeDirection = moveDirection.normalized;

            foreach (Weapon weapon in weapons)
                weapon?.WeaponFixedUpdate();
        }

        protected override void EntityAnimation() {
            base.EntityAnimation();

            animator.SetFloat("WeaponType", (float)(weapons.current.weaponType) );
        }

        public override void SetStyle(int style) {
            weapons.Switch(style);
        }

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
            evadeCurve = Mathf.Clamp01( EntityManager.current.evadeCurve.Evaluate( evadeTime ) );
            return true;
        }

        public void GroundedEvade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;
            
            Vector3 newVelocity = rb.velocity.NullifyInDirection( gravityDown );
            if (!onGround){
                newVelocity += -gravityDown.normalized * 5f;
            }
            rb.velocity = newVelocity;

            StartEvade(evadeDirection);
        }
        public void Evade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;
            
            StartEvade(evadeDirection);
        }

        protected void StartEvade(Vector3 evadeDirection) {
            this.evadeDirection = evadeDirection;
            evadeTimer = data.totalEvadeDuration;

            animator?.SetTrigger("Evade");


            onEvade?.Invoke(evadeDirection);
        }

    }
}