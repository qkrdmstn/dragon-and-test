using System;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

public enum PlayerAnimState
{
	Idle, Run, Wave, Breath, knockBack, Fall
}

public class PlayerAnimController : AnimController
{
    public bool isBreath = false;
	public float mouseDirMinX = -0.2f;
	public float mouseDirMaxX = 0.2f;
	public float mouseDirMinY = -0.2f;
	public float mouseDirMaxY = 0.2f;

	private void Awake()
    {
        base.Awake();
    }

    public override void SetAnim<T>(T _animState, float x = 0, float y = 0) 
	{
		PlayerAnimState animState = Enum.Parse<PlayerAnimState>(_animState.ToString());
		int animIdx = (int)animState;

		if (animState == PlayerAnimState.Idle)
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
		else if (animState == PlayerAnimState.Run || animState == PlayerAnimState.Wave)
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

		baseSkinName = skins[animIdx].skin[(int)curBaseSkinIdx];
		curAnim = anims[animIdx];

		skeletonAnimation.skeleton.SetSkin(baseSkinName);
		skeletonAnimation.Skeleton.SetSlotsToSetupPose();

		if (animState != PlayerAnimState.Wave && (isBreath || animState == PlayerAnimState.Run))
		{   // 총을 쏘거나 마우스 이동의 얼굴 방향 및 표정 스킨 갱신 (단 대시중에는 변경 금지)
			ChangeSkinSlot(SetMouseDirection(Player.instance.stateMachine.currentState.mouseDir.x, Player.instance.stateMachine.currentState.mouseDir.y));
		}

		skeletonAnimation.AnimationName = curAnim;
		skeletonAnimation.ApplyAnimation();
	}

    protected override void ChangeSkinSlot(Direction _region)
    {
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
}
