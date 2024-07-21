using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using Spine.Unity.AttachmentTools;

public enum AnimState
{
	Idle, Run, Wave, Breath, knockBack
}

public enum Direction
{
	Back, Back_L, Back_R,
	Front, Front_L, Front_R
}

public class AnimController : MonoBehaviour
{
	SkeletonAnimation skeletonAnimation;

	// character skins
	[SpineSkin] public string baseSkinName;
	[SpineSkin] public string[] idleSkin;
	[SpineSkin] public string[] runSkin;
	[SpineSkin] public string[] waveSkin;

	[SpineAnimation] public string idleAnim;
	[SpineAnimation] public string runAnim;
	[SpineAnimation] public string waveAnim;
	string curAnim;

	public bool isBreath = false;
	int curDirectionIdx = -1;
	int curBaseSkinIdx = -1;

	[System.Serializable]
	public class SlotRegionPair
	{
		[SpineSlot] public string slot;
		[SpineAtlasRegion] public string region;
	}

	[SerializeField] protected SpineAtlasAsset atlasAsset;
	[SerializeField] protected bool inheritProperties = true;
	[SerializeField] protected List<SlotRegionPair> attachments = new List<SlotRegionPair>();

	Atlas atlas;
	SkeletonRenderer skeletonRenderer;	// 피격 시 깜빡임
	float mouseDirMinX = -0.2f;
	float mouseDirMaxX = 0.2f;
	float mouseDirMinY = -0.2f;
	float mouseDirMaxY = 0.2f;

	void Awake()
	{
		skeletonRenderer = GetComponent<SkeletonRenderer>();
		skeletonAnimation = GetComponent<SkeletonAnimation>();

		skeletonRenderer.OnRebuild += Apply;
	}

	private void Update()
	{
		if (skeletonRenderer.valid && isBreath) Apply(skeletonRenderer);
	}

	public void SetAnim(AnimState animState, float x = 0f, float y = 0f)
	{
		if (animState == AnimState.Idle)
        {
			if (x >= mouseDirMaxX && y >= mouseDirMaxY) curBaseSkinIdx = (int)Direction.Back_R;
			else if (x >= mouseDirMinX && x < mouseDirMaxX && y >= mouseDirMaxY) curBaseSkinIdx = (int)Direction.Back;
			else if (x >= mouseDirMaxX && y < mouseDirMaxY && y >= mouseDirMinY) curBaseSkinIdx = (int)Direction.Front_R;
			else if (x >= mouseDirMaxX && y < mouseDirMinY) curBaseSkinIdx = (int)Direction.Front_R;
			else if (x >= mouseDirMinX && x < mouseDirMaxX && y < mouseDirMaxY) curBaseSkinIdx = (int)Direction.Front;
			else if (x < mouseDirMinX && y < mouseDirMaxY && y >= mouseDirMinY) curBaseSkinIdx = (int)Direction.Front_L;
			else if (x < mouseDirMinX && y < mouseDirMinY) curBaseSkinIdx = (int)Direction.Front_L;
			else if (x < mouseDirMinX && y >= mouseDirMaxY) curBaseSkinIdx = (int)Direction.Back_L;
		}
		else if (animState == AnimState.Run || animState == AnimState.Wave)
        {
			if (x == 1 && y == 1) curBaseSkinIdx = (int)Direction.Back_R;
			else if (x == 0 && y == 1) curBaseSkinIdx = (int)Direction.Back;
			else if (x == 1 && y == 0) curBaseSkinIdx = (int)Direction.Front_R;
			else if (x == 1 && y == -1) curBaseSkinIdx = (int)Direction.Front_R;
			else if (x == 0 && y == 0) curBaseSkinIdx = (int)Direction.Front;
			else if (x == 0 && y == -1) curBaseSkinIdx = (int)Direction.Front;
			else if (x == -1 && y == -1) curBaseSkinIdx = (int)Direction.Front_L;
			else if (x == -1 && y == 0) curBaseSkinIdx = (int)Direction.Front_L;
			else if (x == -1 && y == 1) curBaseSkinIdx = (int)Direction.Back_L;
		}

		switch (animState)
		{
			case AnimState.Idle:
				baseSkinName = idleSkin[curBaseSkinIdx];
				curAnim = idleAnim;
				break;

			case AnimState.Run:
				baseSkinName = runSkin[curBaseSkinIdx];
				curAnim = runAnim;
				break;

			case AnimState.Wave:
				baseSkinName = waveSkin[curBaseSkinIdx];
				curAnim = waveAnim;
				break;
		}

		if (isBreath)
		{
			skeletonAnimation.AnimationName = curAnim;
			skeletonAnimation.ApplyAnimation();
			return;
		}

		skeletonAnimation.initialSkinName = baseSkinName;
		skeletonAnimation.Skeleton.SetSkin(baseSkinName);
		skeletonAnimation.Skeleton.SetSlotsToSetupPose();

		skeletonAnimation.AnimationName = curAnim;
		skeletonAnimation.ApplyAnimation();
	}

	[SpineAtlasRegion] public string[] idleFace;
	[SpineAtlasRegion] public string[] runFace;

	public void SetBreath(float x, float y)
    {
		if (x >= mouseDirMaxX && y >= mouseDirMaxY)
			curDirectionIdx = (int)Direction.Back_R;
		else if (x >= mouseDirMinX && x < mouseDirMaxX && y >= mouseDirMaxY)
			curDirectionIdx = (int)Direction.Back;
		else if (x >= mouseDirMaxX && y < mouseDirMaxY && y >= mouseDirMinY)
			curDirectionIdx = (int)Direction.Front_R;
		else if (x >= mouseDirMaxX && y < mouseDirMinY)
			curDirectionIdx = (int)Direction.Front_R;
		else if (x >= mouseDirMinX && x < 60 && y < mouseDirMaxY)
			curDirectionIdx = (int)Direction.Front;
		else if (x < mouseDirMinX && y < mouseDirMaxY && y >= mouseDirMinY)
			curDirectionIdx = (int)Direction.Front_L;
		else if (x < mouseDirMinX && y < mouseDirMinY)
			curDirectionIdx = (int)Direction.Front_L;
		else if (x < mouseDirMinX && y >= mouseDirMaxY)
			curDirectionIdx = (int)Direction.Back_L;

		if (Player.instance.stateMachine.currentState == Player.instance.idleState)
        {
			isBreath = true;
			attachments[0].slot = "IDLE_basicface";
			attachments[0].region = idleFace[curDirectionIdx];
		}
		else if(Player.instance.stateMachine.currentState == Player.instance.moveState)
        {
			isBreath = true;
			attachments[0].slot = "RUN_basicface";
			attachments[0].region = runFace[curDirectionIdx];
		}
    }

	public void SetMaterialColor(Color color)
    {
		skeletonAnimation.skeleton.SetColor(color);
	}

	void Apply(SkeletonRenderer skeletonRenderer)
	{
		if (!this.enabled) return;

		atlas = atlasAsset.GetAtlas();
		if (atlas == null) return;
		float scale = skeletonRenderer.skeletonDataAsset.scale;

		foreach (SlotRegionPair entry in attachments)
		{
			Slot slot = skeletonRenderer.Skeleton.FindSlot(entry.slot);
			Attachment originalAttachment = slot.Attachment;
			AtlasRegion region = atlas.FindRegion(entry.region);

			if (region == null)
			{
				slot.Attachment = null;
			}
			else if (inheritProperties && originalAttachment != null)
			{
				slot.Attachment = originalAttachment.GetRemappedClone(region, true, true, scale);
			}
			else
			{
				RegionAttachment newRegionAttachment = region.ToRegionAttachment(region.name, scale);
				slot.Attachment = newRegionAttachment;
			}
		}
	}
}


