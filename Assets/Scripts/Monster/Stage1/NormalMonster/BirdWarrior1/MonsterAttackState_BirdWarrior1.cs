using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterAttackState_BirdWarrior1 : MonsterAttackStateBase
{
    protected new BirdWarrior1 monster;

    public MonsterAttackState_BirdWarrior1(MonsterStateMachine _stateMachine, Player _player, BirdWarrior1 _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;
    }

    public override void Enter()
    {
        base.Enter();

        monster.SetSpeed(0.0f);
        attackCoroutine = monster.StartCoroutine(Shoot());
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        monster.SetSpeed(monster.moveSpeed);
    }

    IEnumerator Shoot()
    {
        for (int i = 0; i < monster.shootNum; i++)
        {
            monster.monsterAnimController.SetAnim(MonsterAnimState.Attack, monster.CheckDir());
            Vector3 dir = player.transform.position - monster.transform.position;

            GameObject aura = GameObject.Instantiate(monster.swordAura, monster.transform.position, Quaternion.identity);
            MonsterBullet auraScript = aura.GetComponent<MonsterBullet>();
            SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.nearAttack);
            auraScript.BulletInitialize(dir);
            yield return new WaitForSeconds(monster.shootDelay);
        }

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.chaseDist)
            stateMachine.ChangeState(monster.chaseState);
        else
            stateMachine.ChangeState(monster.idleState);
    }
}
