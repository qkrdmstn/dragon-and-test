using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterAttackState_BirdCrossbowman : MonsterAttackStateBase
{
    protected new BirdCrossbowman monster;
    private MonsterAnimController monsterAnimController;

    ContactFilter2D filter;
    public MonsterAttackState_BirdCrossbowman(MonsterStateMachine _stateMachine, Player _player, BirdCrossbowman _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;

        if(monster.haveAnim)
            monsterAnimController = monster.monsterAnimController;
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
        int curbulletsOnce = monster.bulletNumPerWave;
        float theta = monster.attackRangeAngle / monster.bulletNumPerWave;

        for (int j = 0; j < monster.waveNum; j++)
        {
            Vector3 dir = player.transform.position - monster.transform.position;
            for (int i = 0; i < curbulletsOnce; i++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = monster.transform.position;

                float rotateAngle = (theta * i) + (theta * j * 0.5f) - (monster.attackRangeAngle / 2.0f);
                bulletComponent.BulletInitialize(Quaternion.AngleAxis(rotateAngle, Vector3.forward) * dir, monster.bulletSpeed);
            }
            curbulletsOnce--;
            yield return new WaitForSeconds(monster.waveInterval);
        }
        yield return new WaitForSeconds(monster.reloadDelay);

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.escapeDist)
            stateMachine.ChangeState(monster.escapeState);
        else if(dist < monster.chaseDist)
            stateMachine.ChangeState(monster.chaseState);
        else
            stateMachine.ChangeState(monster.idleState);
    }
}
