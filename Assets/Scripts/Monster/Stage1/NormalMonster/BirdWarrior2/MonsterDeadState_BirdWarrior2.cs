using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterDeadState_BirdWarrior2 : MonsterDeadStateBase
{
    protected new BirdWarrior2 monster;
    public MonsterDeadState_BirdWarrior2(MonsterStateMachine _stateMachine, Player _player, BirdWarrior2 _monster) : base(_stateMachine, _player, _monster)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected override void Dead()
    {
        monster.warningObject.SetActive(false);
        base.Dead();
    }


}
