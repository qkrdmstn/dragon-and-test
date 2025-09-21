using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrossbowmanAnimState
{
    Run, Attack, Spawn, Death, Idle, Idle2
}

public class CrossbowmanAnimationController : MonsterAnimController
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetAnim()
    {   // 생성
        baseSkinName = skins[0].skin[(int)Direction.FRONT];
        curAnim = anims[(int)CrossbowmanAnimState.Spawn];

        if (!GetAnimLoop())
            skeletonAnimation.state.GetCurrent(0).TrackTime = 0;

        skeletonAnimation.skeleton.SetSkin(baseSkinName);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();

        skeletonAnimation.AnimationName = curAnim;
        skeletonAnimation.ApplyAnimation();
    }

    public override void SetAnim<T>(T _animState, Direction direction, bool isLoop = false)
    {
        CrossbowmanAnimState animState = Enum.Parse<CrossbowmanAnimState>(_animState.ToString());

        if (animState == CrossbowmanAnimState.Run) skeletonAnimation.loop = true;
        else skeletonAnimation.loop = false;

        baseSkinName = skins[0].skin[(int)direction];
        curAnim = anims[(int)animState];

        if (!GetAnimLoop())
            skeletonAnimation.state.GetCurrent(0).TrackTime = 0;

        skeletonAnimation.skeleton.SetSkin(baseSkinName);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();

        skeletonAnimation.AnimationName = curAnim;
        skeletonAnimation.ApplyAnimation();
    }
}
