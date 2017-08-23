using UnityEngine;
using UnityEditor;
using System.Collections;
using Spine;
using Spine.Unity;

namespace Fungus.EditorUtils
{

	[CustomEditor (typeof(PlaySpineAnim))]
	public class PlaySpineAnimEditor : CommandEditor 
	{
		protected SerializedProperty skeletonAnimationProp;
		protected SerializedProperty animationNameProp;
		protected SerializedProperty timeScaleProp;
		protected SerializedProperty loopProp;
		protected SerializedProperty waitUntilFinishedProp;

		protected virtual void OnEnable()
		{
			skeletonAnimationProp = serializedObject.FindProperty("skeletonAnimation");
			animationNameProp = serializedObject.FindProperty("animationName");
			timeScaleProp = serializedObject.FindProperty("timeScale");
			loopProp = serializedObject.FindProperty("loop");
			waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			PlaySpineAnim playSpineAnim = target as PlaySpineAnim;

			EditorGUILayout.PropertyField(skeletonAnimationProp);

			SkeletonAnimation skeletonAnimation = playSpineAnim.skeletonAnimation;

			if (skeletonAnimation != null)
			{
				SkeletonData skeletonData = skeletonAnimation.skeletonDataAsset.GetSkeletonData(true);
				if (skeletonData != null)
				{
					string[] animations = new string[skeletonData.Animations.Count + 1];
					animations[0] = "<None>";
					int animationIndex = 0;
					for (int i = 0; i < animations.Length - 1; i++) 
					{
						string name = skeletonData.Animations.Items[i].Name;
						animations[i + 1] = name;
						if (name == animationNameProp.stringValue)
						{
							animationIndex = i + 1;
						}
					}

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Animation", GUILayout.Width(EditorGUIUtility.labelWidth));
					animationIndex = EditorGUILayout.Popup(animationIndex, animations);
					EditorGUILayout.EndHorizontal();

					string selectedAnimationName = animationIndex == 0 ? null : animations[animationIndex];
					if (animationNameProp.stringValue != selectedAnimationName) 
					{
						animationNameProp.stringValue = selectedAnimationName;
					}
				}
			}

			EditorGUILayout.PropertyField(timeScaleProp);
			EditorGUILayout.PropertyField(loopProp);
			EditorGUILayout.PropertyField(waitUntilFinishedProp);

			serializedObject.ApplyModifiedProperties();
		}
	}

}