using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class ArmedEntity : Entity {
        
        [SerializeReference] protected WeaponInventory _weapons = null;


        [Header("Parrying")]
        public float parryTimer;


        [Header("Evading")]
        public BoolData evading;
        public Vector3 evadeDirection = Vector3.forward;
        public float evadeTimer { get; protected set; }
        public float evadeCount = 1f;

        public float evadeTime { get; protected set; }
        public float evadeCurve { get; protected set; }

        public event Action<Vector3> onEvade;
        public event Action onParry;


        public override float weight => Mathf.Min( base.weight, weapons.current.weightModifier ); 
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


        public override void LoadModel() {
            base.LoadModel();
            foreach(Weapon.Instance weapon in weapons) {
                weapon.LoadModel();
            }
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



        public void Evade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;
            
            if (gravityMultiplier > 0f) {

                Vector3 newVelocity = rigidbody.velocity.NullifyInDirection( gravityDown );
                if (!onGround){
                    newVelocity += -gravityDown.normalized * 5f;
                }
                rigidbody.velocity = newVelocity;
            }
            

            this.evadeDirection = evadeDirection;
            evadeTimer = character.totalEvadeDuration;

            animator.SetTrigger("Evade");


            onEvade?.Invoke(evadeDirection);
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

            if ( !isIdle && evadeTimer > character.totalEvadeDuration - 0.15f )
                evadeDirection = absoluteForward;

            if ( !evading ) {
                evadeTime = 0f;
                evadeCurve = 0f;
            } else {
                evadeTime = Mathf.Clamp01( 1 - ( (evadeTimer - character.evadeCooldown) / character.evadeDuration ) );
                evadeCurve = Mathf.Clamp01( EntityManager.current.evadeCurve.Evaluate( evadeTime ) );
            }
            
            foreach (Weapon.Instance weapon in weapons)
                weapon?.Update();
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();

            if (evading)
                Move( evadeCurve * character.evadeSpeed * evadeDirection );

            foreach (Weapon.Instance weapon in weapons)
                weapon?.FixedUpdate();
        }

    }
}