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

	public bool isBreath = false;
	int curDirectionIdx = -1;

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
	SkeletonRenderer skeletonRenderer;

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
		switch (animState)
		{
			
			case AnimState.Idle:


				if (isBreath)
				{
					skeletonAnimation.AnimationName = idleAnim;
					skeletonAnimation.ApplyAnimation();
					break;
				}

				if (x >= 60 && y >= 60)
                {
					baseSkinName = idleSkin[(int)Direction.Back_R];
					curDirectionIdx = (int)Direction.Back_R;
				}
                else if(x >= 45 && x < 60 && y >= 60)
                {
                    baseSkinName = idleSkin[(int)Direction.Back];
					curDirectionIdx = (int)Direction.Back;

				}
				else if(x >= 60 && y < 60 && y >= 45)
                {
                    baseSkinName = idleSkin[(int)Direction.Front_R];
					curDirectionIdx = (int)Direction.Front_R;

				}
				else if(x >= 60 && y < 45)
                {
                    baseSkinName = idleSkin[(int)Direction.Front_R];
					curDirectionIdx = (int)Direction.Front_R;

				}
				else if(x >= 45 && x < 60 && y < 60)
                {
                    baseSkinName = idleSkin[(int)Direction.Front];
					curDirectionIdx = (int)Direction.Front;

				}
				else if(x < 45 && y < 60 && y >= 45)
                {
                    baseSkinName = idleSkin[(int)Direction.Front_L];
					curDirectionIdx = (int)Direction.Front_L;

				}
				else if(x < 45 && y < 45)
                {
                    baseSkinName = idleSkin[(int)Direction.Front_L];
					curDirectionIdx = (int)Direction.Front_L;

				}
				else if(x < 45 && y >= 60)
                {
                    baseSkinName = idleSkin[(int)Direction.Back_L];
					curDirectionIdx = (int)Direction.Back_L;

				}

				skeletonAnimation.initialSkinName = baseSkinName;
				skeletonAnimation.Skeleton.SetSkin(baseSkinName);
				skeletonAnimation.Skeleton.SetSlotsToSetupPose();

				skeletonAnimation.AnimationName = idleAnim;
				skeletonAnimation.ApplyAnimation();
				break;

			case AnimState.Run:
				if (isBreath)
				{
					skeletonAnimation.AnimationName = runAnim;
					skeletonAnimation.ApplyAnimation();
					break;
				}

				if (x == 1 && y == 1)
                {
                    baseSkinName = runSkin[(int)Direction.Back_R];
					curDirectionIdx = (int)Direction.Back_R;
				}
                else if(x == 0 && y == 1)
                {
                    baseSkinName = runSkin[(int)Direction.Back];
					curDirectionIdx = (int)Direction.Back;
				}
                else if(x == 1 && y == 0)
                {
                    baseSkinName = runSkin[(int)Direction.Front_R];
					curDirectionIdx = (int)Direction.Front_R;
				}
                else if(x == 1 && y == -1)
                {
                    baseSkinName = runSkin[(int)Direction.Front_R];
					curDirectionIdx = (int)Direction.Front_R;
				}
                else if(x == 0 && y == 0)
                {
                    baseSkinName = runSkin[(int)Direction.Front];
					curDirectionIdx = (int)Direction.Front;
				}
                else if (x == 0 && y == -1)
                {
                    baseSkinName = runSkin[(int)Direction.Front];
					curDirectionIdx = (int)Direction.Front;
				}
                else if(x == -1 && y == -1)
                {
                    baseSkinName = runSkin[(int)Direction.Front_L];
					curDirectionIdx = (int)Direction.Front_L;
				}
                else if (x == -1 && y == 0)
                {
                    baseSkinName = runSkin[(int)Direction.Front_L];
					curDirectionIdx = (int)Direction.Front_L;
				}
                else if(x == -1 && y == 1)
                {
                    baseSkinName = runSkin[(int)Direction.Back_L];
					curDirectionIdx = (int)Direction.Back_L;
				}

                skeletonAnimation.initialSkinName = baseSkinName;
				skeletonAnimation.Skeleton.SetSkin(baseSkinName);
				skeletonAnimation.Skeleton.SetSlotsToSetupPose();

				skeletonAnimation.AnimationName = runAnim;
				skeletonAnimation.ApplyAnimation();

				break;
			case AnimState.Wave:
				if (x == 1 && y == 1)		baseSkinName = waveSkin[(int)Direction.Back_R];
				else if (x == 0 && y == 1)	baseSkinName = waveSkin[(int)Direction.Back];
				else if (x == 1 && y == 0)	baseSkinName = waveSkin[(int)Direction.Front_R];
				else if (x == 1 && y == -1) baseSkinName = waveSkin[(int)Direction.Front_R];
				else if (x == 0 && y == 0)	baseSkinName = waveSkin[(int)Direction.Front];
				else if (x == 0 && y == -1) baseSkinName = waveSkin[(int)Direction.Front];
				else if (x == -1 && y == -1)baseSkinName = waveSkin[(int)Direction.Front_L];
				else if (x == -1 && y == 0) baseSkinName = waveSkin[(int)Direction.Front_L];
				else if (x == -1 && y == 1) baseSkinName = waveSkin[(int)Direction.Back_L];

				skeletonAnimation.Skeleton.SetSkin(baseSkinName);
				skeletonAnimation.initialSkinName = baseSkinName;
				skeletonAnimation.Skeleton.SetSlotsToSetupPose();
				skeletonAnimation.AnimationName = waveAnim;
				skeletonAnimation.ApplyAnimation();
				break;
		}
	}

	[SpineAtlasRegion] public string[] idleFace;
	[SpineAtlasRegion] public string[] runFace;

	public void SetBreath(float x, float y)
    {
		if(Player.instance.stateMachine.currentState == Player.instance.idleState)
        {
			isBreath = true;
			attachments[0].slot = "IDLE_basicface";
			if (x == 1 && y == 1)		curDirectionIdx = (int)Direction.Back_R;
			else if (x == 0 && y == 1)	curDirectionIdx = (int)Direction.Back;
			else if (x == 1 && y == 0)	curDirectionIdx = (int)Direction.Front_R;
			else if (x == 1 && y == -1) curDirectionIdx = (int)Direction.Front_R;
			else if (x == 0 && y == 0)	curDirectionIdx = (int)Direction.Front;
			else if (x == 0 && y == -1) curDirectionIdx = (int)Direction.Front;
			else if (x == -1 && y == -1)curDirectionIdx = (int)Direction.Front_L;
			else if (x == -1 && y == 0) curDirectionIdx = (int)Direction.Front_L;
			else if (x == -1 && y == 1) curDirectionIdx = (int)Direction.Back_L;
			attachments[0].region = idleFace[curDirectionIdx];
		}
		else if(Player.instance.stateMachine.currentState == Player.instance.moveState)
        {
			isBreath = true;
			attachments[0].slot = "RUN_basicface";

			if (x == 1 && y == 1) curDirectionIdx = (int)Direction.Back_R;
			else if (x == 0 && y == 1) curDirectionIdx = (int)Direction.Back;
			else if (x == 1 && y == 0) curDirectionIdx = (int)Direction.Front_R;
			else if (x == 1 && y == -1) curDirectionIdx = (int)Direction.Front_R;
			else if (x == 0 && y == 0) curDirectionIdx = (int)Direction.Front;
			else if (x == 0 && y == -1) curDirectionIdx = (int)Direction.Front;
			else if (x == -1 && y == -1) curDirectionIdx = (int)Direction.Front_L;
			else if (x == -1 && y == 0) curDirectionIdx = (int)Direction.Front_L;
			else if (x == -1 && y == 1) curDirectionIdx = (int)Direction.Back_L;
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


