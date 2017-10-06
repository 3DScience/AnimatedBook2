using UnityEngine;
using Spine.Unity;
using Spine;

namespace Fungus
{

    [CommandInfo("Spine", 
        "PlaySpineAnim", 
        "Set the active animation in a spine animation.")]	
    public class PlaySpineAnim : Command
    {
        public SkeletonAnimation skeletonAnimation;
		
        public string animationName;

        public float timeScale = 1;

        public bool loop;

        public bool waitUntilFinished = true;

        public override void OnEnter()
        {
            if (skeletonAnimation != null)
            {
                skeletonAnimation.timeScale = timeScale;
                skeletonAnimation.loop = loop;
                skeletonAnimation.AnimationName = animationName;

                if (waitUntilFinished)
                {
                    skeletonAnimation.state.Complete += CompleteDelegate;
                    return;
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (skeletonAnimation == null)
            {
                return "Error: No Spine skeleton animation selected";
            }

            return animationName;
        }

        void CompleteDelegate(TrackEntry trackEntry)
        {
            skeletonAnimation.state.Complete -= CompleteDelegate;
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 221, 169, 255);
        }
    }

}