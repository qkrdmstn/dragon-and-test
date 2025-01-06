using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterDeadStateBase : MonsterState
{
    public MonsterDeadStateBase(MonsterStateMachine _stateMachine, Player _player, MonsterBase _monster) : base(_stateMachine, _player, _monster)
    {
    }

    public override void Enter()
    {
        base.Enter();
        monster.isStateChangeable = false;
        monster.isDead = true;
        monster.SetSpeed(0.0f);
        monster.StopAllCoroutines();
        Dead();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }

    protected virtual void Dead()
    {
        monster.StartCoroutine(DeadCoroutine());
    }

    IEnumerator DeadCoroutine()
    {
        if(monster.haveAnim)
            monster.monsterAnimController.SetAnim(MonsterAnimState.Death, monster.CheckDir());

        float sec = Mathf.Clamp(monster.deadDuration, 0f, 0.7f);
        yield return new WaitForSeconds(sec);

        if (monster.IsEffectSpawner())
        {
            monster.spawner.DeathCount();
            monster.ItemDrop();
        }
        yield return new WaitForSeconds(0.7f - sec);

        GameObject.Destroy(monster.gameObject);
    }
}
