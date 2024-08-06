using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

public enum AnimState
{
	Idle, Run, Wave, Breath, knockBack, Fall
}

public enum Direction
{
	BACK, BACK_L, BACK_R,
	FRONT, FRONT_L, FRONT_R
}

public class AnimController : MonoBehaviour
{
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
	Direction curDirectionIdx, curBaseSkinIdx;

	[System.Serializable]
	public class SlotRegionPair
	{
		[SpineSlot] public string slot;
		[SpineAtlasRegion] public string region;
	}

	[SerializeField] SpineAtlasAsset atlasAsset;
	[SerializeField] List<SlotRegionPair> attachments = new List<SlotRegionPair>();

	Atlas atlas;
	SkeletonAnimation skeletonAnimation;
	SkeletonRenderer skeletonRenderer;	// 피격 시 깜빡임

	float mouseDirMinX = -0.2f;
	float mouseDirMaxX = 0.2f;
	float mouseDirMinY = -0.2f;
	float mouseDirMaxY = 0.2f;

	void Awake()
	{
		skeletonRenderer = GetComponent<SkeletonRenderer>();
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	public void SetAnim(AnimState animState, float x = 0f, float y = 0f)
	{
		if (animState == AnimState.Idle)
        {
			if (x >= mouseDirMaxX && y >= mouseDirMaxY) curBaseSkinIdx = Direction.BACK_R;
			else if (x >= mouseDirMinX && x < mouseDirMaxX && y >= mouseDirMaxY) curBaseSkinIdx = Direction.BACK;
			else if (x >= mouseDirMaxX && y < mouseDirMaxY && y >= mouseDirMinY) curBaseSkinIdx = Direction.FRONT_R;
			else if (x >= mouseDirMaxX && y < mouseDirMinY) curBaseSkinIdx = Direction.FRONT_R;
			else if (x >= mouseDirMinX && x < mouseDirMaxX && y < mouseDirMaxY) curBaseSkinIdx = Direction.FRONT;
			else if (x < mouseDirMinX && y < mouseDirMaxY && y >= mouseDirMinY) curBaseSkinIdx = Direction.FRONT_L;
			else if (x < mouseDirMinX && y < mouseDirMinY) curBaseSkinIdx = Direction.FRONT_L;
			else if (x < mouseDirMinX && y >= mouseDirMaxY) curBaseSkinIdx = Direction.BACK_L;
		}
		else if (animState == AnimState.Run || animState == AnimState.Wave)
        {
			if (x == 1 && y == 1) curBaseSkinIdx = Direction.BACK_R;
			else if (x == 0 && y == 1) curBaseSkinIdx = Direction.BACK;
			else if (x == 1 && y == 0) curBaseSkinIdx = Direction.FRONT_R;
			else if (x == 1 && y == -1) curBaseSkinIdx = Direction.FRONT_R;
			else if (x == 0 && y == 0) curBaseSkinIdx = Direction.FRONT;
			else if (x == 0 && y == -1) curBaseSkinIdx = Direction.FRONT;
			else if (x == -1 && y == -1) curBaseSkinIdx = Direction.FRONT_L;
			else if (x == -1 && y == 0) curBaseSkinIdx = Direction.FRONT_L;
			else if (x == -1 && y == 1) curBaseSkinIdx = Direction.BACK_L;
		}

		switch (animState)
		{
			case AnimState.Idle:
				baseSkinName = idleSkin[(int)curBaseSkinIdx];
				curAnim = idleAnim;
				break;

			case AnimState.Run:
				baseSkinName = runSkin[(int)curBaseSkinIdx];
				curAnim = runAnim;
				break;

			case AnimState.Wave:
				baseSkinName = waveSkin[(int)curBaseSkinIdx];
				curAnim = waveAnim;
				break;
		}

		skeletonAnimation.skeleton.SetSkin(baseSkinName);
		skeletonAnimation.Skeleton.SetSlotsToSetupPose();

		if (isBreath || animState == AnimState.Run)
        {	// 총을 쏘거나 마우스 이동의 얼굴 방향 및 표정 스킨 갱신
			ChangeSkinSlot(SetMouseDirection(Player.instance.stateMachine.currentState.mouseDir.x, Player.instance.stateMachine.currentState.mouseDir.y));
		}

		skeletonAnimation.AnimationName = curAnim;
		skeletonAnimation.ApplyAnimation();
	}

	void ChangeSkinSlot(Direction _region)
    {
		atlas = atlasAsset.GetAtlas();
		float scale = skeletonRenderer.skeletonDataAsset.scale;

		Slot slot;
		AtlasRegion region;

		if (isBreath)
		{
			if (Player.instance.stateMachine.currentState == Player.instance.idleState)
			{
				attachments[0].slot = "IDLE_basicface";
			}
			else if (Player.instance.stateMachine.currentState == Player.instance.moveState)
			{
				attachments[0].slot = "RUN_basicface";
			}

			slot = skeletonRenderer.skeleton.FindSlot(attachments[0].slot);
			region = atlas.FindRegion(_region.ToString() + "/Face_Bress");
		}
		else
		{
			slot = skeletonRenderer.skeleton.FindSlot(attachments[1].slot);

			switch (_region)
			{
				case Direction.FRONT_R:
					region = atlas.FindRegion(_region.ToString() + "/FACE");
					break;
				default:
					region = atlas.FindRegion(_region.ToString() + "/Face");
					break;
			}
		}

		Attachment originAttachment = slot.Attachment;
		slot.Attachment = originAttachment.GetRemappedClone(region, true, true, scale);
	}

	Direction SetMouseDirection(float x, float y)
    {
		if (x >= mouseDirMaxX && y >= mouseDirMaxY)
			curDirectionIdx = Direction.BACK_R;

		else if (x >= mouseDirMinX && x < mouseDirMaxX && y >= mouseDirMaxY)
			curDirectionIdx = Direction.BACK;

		else if (x >= mouseDirMaxX && y < mouseDirMaxY)
			curDirectionIdx = Direction.FRONT_R;

		else if (x >= mouseDirMinX && x < mouseDirMaxX && y < mouseDirMaxY)
			curDirectionIdx = Direction.FRONT;

		else if (x < mouseDirMinX && y < mouseDirMaxY && y >= mouseDirMinY)
			curDirectionIdx = Direction.FRONT_L;
		else if (x < mouseDirMinX && y < mouseDirMinY)
			curDirectionIdx = Direction.FRONT_L;

		else if (x < mouseDirMinX && y >= mouseDirMaxY)
			curDirectionIdx = Direction.BACK_L;

		return curDirectionIdx;
	}

	public void SetMaterialColor(Color color)
    {
		skeletonAnimation.skeleton.SetColor(color);
	}
}


