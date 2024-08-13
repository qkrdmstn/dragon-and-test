using System;

using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

public enum MonsterAnimState
{
    Run, Attack, Spawn, Death
}

public class MonsterAnimController : AnimController
{
    private void Awake()
    {
        base.Awake();
    }

    public override void SetAnim<T>(T _animState, float x = 0, float y = 0)
    {   // x = direction
        // directions = { 0, 45, 90, 180, 270, 315, 360}
        MonsterAnimState animState = Enum.Parse<MonsterAnimState>(_animState.ToString());

        if(animState == MonsterAnimState.Attack)
        {

        }
    }

}
