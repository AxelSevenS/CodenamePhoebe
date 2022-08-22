using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class ArmedEntity : Entity {
        
        [SerializeReference]
        protected WeaponInventory _weapons = null;

        public WeaponInventory weapons {
            get {
                if (_weapons == null) {
                    ResetWeapons();
                }
                return _weapons;
            }
        }

        public virtual void ResetWeapons(){
            _weapons = new ListWeaponInventory(this, 1);
        }


        [Header("Parrying")]
        public float parryTimer;


        [Header("Evading")]
        public BoolData evading;
        public Vector3 evadeDirection = Vector3.forward;
        public float evadeTimer;
        public float evadeCount = 1f;

        public event Action<Vector3> onEvade;
        public event Action onParry;

        public override bool CanTurn() => !evading;

        public override void LoadModel() {
            base.LoadModel();
            foreach(Weapon.Instance weapon in weapons) {
                weapon.LoadModel();
            }
        }

        protected override void Awake(){
            base.Awake();
        }

        protected override void Reset(){
            base.Reset();
            ResetWeapons();
        }

        protected override void Update(){
            base.Update();

            evading.SetVal( evadeTimer > character.evadeCooldown );
            
            evadeTimer = Mathf.MoveTowards( evadeTimer, 0f, GameUtility.timeDelta );
            parryTimer = Mathf.MoveTowards( parryTimer, 0f, GameUtility.timeDelta );


            if ( KeyInputData.SimultaneousTap( controller.lightAttackInput, controller.heavyAttackInput ) )
                Parry();
            
            foreach (Weapon.Instance weapon in weapons)
                weapon?.Update();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            if ( moveDirection.sqrMagnitude > 0f && evadeTimer > character.totalEvadeDuration - 0.15f )
                evadeDirection = moveDirection.normalized;

            foreach (Weapon.Instance weapon in weapons)
                weapon?.FixedUpdate();
        }

        protected override void EntityAnimation() {
            base.EntityAnimation();

            // animator.SetFloat("WeaponType", (float)(weapons.current.weaponType) );
        }

        public override void SetStyle(int newStyle) {
            weapons.Switch(newStyle);
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

            evadeTime = Mathf.Clamp01( 1 - ( (evadeTimer - character.evadeCooldown) / character.evadeDuration ) );
            evadeCurve = Mathf.Clamp01( EntityManager.current.evadeCurve.Evaluate( evadeTime ) );
            return true;
        }

        public void GroundedEvade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;
            
            Vector3 newVelocity = rigidbody.velocity.NullifyInDirection( gravityDown );
            if (!onGround){
                newVelocity += -gravityDown.normalized * 5f;
            }
            rigidbody.velocity = newVelocity;

            StartEvade(evadeDirection);
        }
        public void Evade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;
            
            StartEvade(evadeDirection);
        }

        protected void StartEvade(Vector3 evadeDirection) {
            this.evadeDirection = evadeDirection;
            evadeTimer = character.totalEvadeDuration;

            animator.SetTrigger("Evade");


            onEvade?.Invoke(evadeDirection);
        }

    }
}