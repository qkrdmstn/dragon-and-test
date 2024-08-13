using System;
using System.Collections.Generic;
using UnityEngine;

using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

public enum Direction
{
	BACK_L, BACK, BACK_R,
	FRONT_L, FRONT, FRONT_R
}

public class AnimController : MonoBehaviour
{
	[SpineSkin] public string baseSkinName;
	[SpineAnimation] public string[] anims;

	protected string curAnim;
	protected Direction curDirectionIdx, curBaseSkinIdx;

	[System.Serializable]
	public class SlotRegionPair
	{
		[SpineSlot] public string slot;
		[SpineAtlasRegion] public string region;
	}

	[System.Serializable]
	public class SkinType
	{
		[SpineSkin] public string[] skin;
	}
	
	public List<SlotRegionPair> attachments = new List<SlotRegionPair>();
	public List<SkinType> skins = new List<SkinType>();
	public SpineAtlasAsset atlasAsset;

	protected Atlas atlas;
	protected SkeletonAnimation skeletonAnimation;
	protected SkeletonRenderer skeletonRenderer;

    public float dirMinX = -0.2f;
    public float dirMaxX = 0.2f;

    public float dirMinY = -0.2f;
    public float dirMaxY = 0.2f;

    protected virtual void Awake()
	{
		skeletonRenderer = GetComponent<SkeletonRenderer>();
		skeletonAnimation = GetComponent<SkeletonAnimation>();

		atlas = atlasAsset.GetAtlas();
	}

    public virtual void SetAnim()
    {
        skeletonAnimation.skeleton.SetSkin(baseSkinName);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();

        skeletonAnimation.AnimationName = curAnim;
        skeletonAnimation.ApplyAnimation();
    }

	public virtual void SetAnim<T>(T _animState, float x = 0f, float y = 0f) where T : Enum
	{
    }

    public virtual void SetAnim<T>(T _animState, Direction direction) where T : Enum
    {
    }

    protected virtual void ChangeSkinSlot(Direction _region)
    {
		
	}

	public void SetMaterialColor(Color color)
    {
		skeletonAnimation.skeleton.SetColor(color);
	}
}


