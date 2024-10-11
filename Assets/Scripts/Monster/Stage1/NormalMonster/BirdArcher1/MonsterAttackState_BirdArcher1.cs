using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterAttackState_BirdArcher1 : MonsterState
{
    protected new BirdArcher1 monster;
    private MonsterAnimController monsterAnimController;

    ContactFilter2D filter;
    public MonsterAttackState_BirdArcher1(MonsterStateMachine _stateMachine, Player _player, BirdArcher1 _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;

        if(monster.haveAnim)
            monsterAnimController = monster.monsterAnimController;
    }

    public override void Enter()
    {
        base.Enter();

        monster.SetSpeed(0.0f);
        monster.StartCoroutine(Shoot());
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
            SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.farAttack);

            Vector3 dir = player.transform.position - monster.transform.position;
            
            //Create Bullet
            var bulletGo = MonsterPool.instance.pool.Get();
            var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
            bulletGo.transform.position = monster.transform.position;
            bulletComponent.BulletInitialize(Quaternion.AngleAxis(Random.Range(-monster.shootOffsetAngle, monster.shootOffsetAngle), Vector3.forward) * dir, monster.bulletSpeed);

            yield return new WaitForSeconds(monster.shootDelay);
        }
        yield return new WaitForSeconds(monster.reloadDelay);

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.chaseDist)
            stateMachine.ChangeState(monster.chaseState);
        else
            stateMachine.ChangeState(monster.idleState);
    }
}
