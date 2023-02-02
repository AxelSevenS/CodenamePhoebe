using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    [CreateAssetMenu(fileName = "new Character Animations", menuName = "Animation Set/Character", order = 0)]
    public class CharacterAnimationSet : AddressableAsset<CharacterAnimationSet>, IAnimationClipSource {

        
        public AnimationClip idleAnimation;

        [Space(15)]
        public AnimationClip moveStartSlowAnimation;
        public AnimationClip moveStartAnimation;
        public AnimationClip moveStartFastAnimation;

        [Space(15)]
        public AnimationClip moveCycleSlowAnimation;
        public AnimationClip moveCycleAnimation;
        public AnimationClip moveCycleFastAnimation;

        [Space(15)]
        public AnimationClip moveStopSlowAnimation;
        public AnimationClip moveStopAnimation;
        public AnimationClip moveStopFastAnimation;

        [Space(15)]
        public AnimationClip jumpAnimation;

        public AnimationClip fallAnimation;
        public AnimationClip landAnimation;

        [Space(15)]
        public AnimationClip evadeForwardAnimation;
        public AnimationClip evadeRightAnimation;
        public AnimationClip evadeLeftAnimation;
        public AnimationClip evadeBackwardAnimation;

        [Space(15)]
        public AnimationClip evadeAirForwardAnimation;
        public AnimationClip evadeAirRightAnimation;
        public AnimationClip evadeAirLeftAnimation;
        public AnimationClip evadeAirBackwardAnimation;




        public void GetAnimationClips(List<AnimationClip> results) {
            results.Add(idleAnimation);

            results.Add(moveStartSlowAnimation);
            results.Add(moveStartAnimation);
            results.Add(moveStartFastAnimation);

            results.Add(moveCycleSlowAnimation);
            results.Add(moveCycleAnimation);
            results.Add(moveCycleFastAnimation);

            results.Add(moveStopSlowAnimation);
            results.Add(moveStopAnimation);
            results.Add(moveStopFastAnimation);

            results.Add(jumpAnimation);

            results.Add(fallAnimation);
            results.Add(landAnimation);

            results.Add(evadeForwardAnimation);
            results.Add(evadeRightAnimation);
            results.Add(evadeLeftAnimation);
            results.Add(evadeBackwardAnimation);

            results.Add(evadeAirForwardAnimation);
            results.Add(evadeAirRightAnimation);
            results.Add(evadeAirLeftAnimation);
            results.Add(evadeAirBackwardAnimation);
        }
        
    }
}
