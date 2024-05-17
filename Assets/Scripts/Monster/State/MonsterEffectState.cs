using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEffectState : MonsterState
{
    private MonsterBase monster;
    public float effectTime;
    public MonsterEffectState(MonsterStateMachine _stateMachine, GameObject _player, MonsterBase _monster) : base(_stateMachine, _player)
    {
        monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("속박됨");
        monster.inEffect = true;
        monster.rigidBody.velocity = Vector3.zero;
        monster.rigidBody.angularVelocity = 0;
        monster.spriteRenderer.color = Color.cyan;
        effectTime = 2.0f;
    }

    public override void Exit()
    {
        Debug.Log("속박해제");
        monster.inEffect = false;
        monster.spriteRenderer.color = Color.white;
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        effectTime -= Time.deltaTime;
        if (effectTime <= 0.0f) stateMachine.ChangeState(monster.tempState);

        /*
        switch ()
        {
            case @@@@:
                monster.rigidBody.velocity = Vector3.zero;
                monster.rigidBody.angularVelocity = 0;
                break;
        }
        */
    }
}