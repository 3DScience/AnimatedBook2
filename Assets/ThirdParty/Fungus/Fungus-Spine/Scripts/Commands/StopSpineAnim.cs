using UnityEngine;
using Spine.Unity;

namespace Fungus
{

	[CommandInfo("Spine", 
	             "Stop Spine Anim", 
	             "Stops the active animation in a spine animation.")]	
	public class StopSpineAnim : Command
	{
		public SkeletonAnimation skeletonAnimation;

		public override void OnEnter()
		{
			if (skeletonAnimation != null)
			{
				skeletonAnimation.AnimationName = null;
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (skeletonAnimation == null)
			{
				return "Error: No skeleton animation selected";
			}

			return skeletonAnimation.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 221, 169, 255);
		}
	}

}