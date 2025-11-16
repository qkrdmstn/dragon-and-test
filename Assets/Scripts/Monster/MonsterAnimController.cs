using System;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine.UIElements;

public enum MonsterAnimState
{
    Run, Attack, Spawn, Death
}

// spine animation state : https://ko.esotericsoftware.com/spine-unity-events

public class MonsterAnimController : AnimController
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetAnim()
    {   // 생성
        baseSkinName = skins[0].skin[(int)Direction.FRONT];
        curAnim = anims[(int)MonsterAnimState.Spawn];

        base.SetAnim();
    }

    public override void SetAnim<T>(T _animState, Direction direction, bool isLoop = false)
    {  
        MonsterAnimState animState = Enum.Parse<MonsterAnimState>(_animState.ToString());
        
        if (animState == MonsterAnimState.Run || isLoop) skeletonAnimation.loop = true;
        else skeletonAnimation.loop = false;

        baseSkinName = skins[0].skin[(int)direction];
        curAnim = anims[(int)animState];

        base.SetAnim();
    }

    public Direction FindDirToPlayer(Vector3 direction)
    {   // 방향이 바뀔 때마다 6방향 애니메이션을 틀어야함
        if(direction.y > dirMaxY)
        {   // back
            if (direction.x > dirMaxX ) curDirectionIdx = Direction.BACK_R;
            else if (direction.x > dirMinX) curDirectionIdx = Direction.BACK;
            else    curDirectionIdx = Direction.BACK_L;
        }
        else
        {   // front
            if (direction.x > dirMaxX) curDirectionIdx = Direction.FRONT_R;
            else if (direction.x > dirMinX) curDirectionIdx = Direction.FRONT;
            else  curDirectionIdx = Direction.FRONT_L;
        }
        return curDirectionIdx;
    }
}
