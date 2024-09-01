using System;
using Spine;
using Spine.Unity.AttachmentTools;

public enum PlayerAnimState
{
	Idle, Run, Wave, Breath, knockBack, Fall
}

public class PlayerAnimController : AnimController
{
    public bool isBreath = false;

	protected override void Awake()
    {
        base.Awake();
    }

    public override void SetAnim<T>(T _animState, float x = 0, float y = 0) 
	{
		PlayerAnimState animState = Enum.Parse<PlayerAnimState>(_animState.ToString());
		int animIdx = (int)animState;

		if (animState == PlayerAnimState.Idle)
		{
            if (x >= dirMaxX && y >= dirMaxY) curBaseSkinIdx = Direction.BACK_R;
			else if (x >= dirMinX && x < dirMaxX && y >= dirMaxY) curBaseSkinIdx = Direction.BACK;
			else if (x >= dirMaxX && y < dirMaxY && y >= dirMinY) curBaseSkinIdx = Direction.FRONT_R;
			else if (x >= dirMaxX && y < dirMinY) curBaseSkinIdx = Direction.FRONT_R;
			else if (x >= dirMinX && x < dirMaxX && y < dirMaxY) curBaseSkinIdx = Direction.FRONT;
			else if (x < dirMinX && y < dirMaxY && y >= dirMinY) curBaseSkinIdx = Direction.FRONT_L;
			else if (x < dirMinX && y < dirMinY) curBaseSkinIdx = Direction.FRONT_L;
			else if (x < dirMinX && y >= dirMaxY) curBaseSkinIdx = Direction.BACK_L;
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

        base.SetAnim();

        if (animState != PlayerAnimState.Wave && (animState == PlayerAnimState.Run || animState == PlayerAnimState.Idle))
		{   // 마우스 이동의 얼굴 방향 및 표정 스킨 갱신 (단 대시중에는 변경 금지)
			ChangeSkinSlot(SetMouseDirection(Player.instance.stateMachine.currentState.mouseDir.x, Player.instance.stateMachine.currentState.mouseDir.y));
		}
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
		slot.Attachment = originAttachment?.GetRemappedClone(region, true, true, scale);
	}

	Direction SetMouseDirection(float x, float y)
	{
		if (x >= dirMaxX && y >= dirMaxY)
			curDirectionIdx = Direction.BACK_R;

		else if (x >= dirMinX && x < dirMaxX && y >= dirMaxY)
			curDirectionIdx = Direction.BACK;

		else if (x >= dirMaxX && y < dirMaxY)
			curDirectionIdx = Direction.FRONT_R;

		else if (x >= dirMinX && x < dirMaxX && y < dirMaxY)
			curDirectionIdx = Direction.FRONT;

		else if (x < dirMinX && y < dirMaxY && y >= dirMinY)
			curDirectionIdx = Direction.FRONT_L;
		else if (x < dirMinX && y < dirMinY)
			curDirectionIdx = Direction.FRONT_L;

		else if (x < dirMinX && y >= dirMaxY)
			curDirectionIdx = Direction.BACK_L;

		return curDirectionIdx;
	}
}
