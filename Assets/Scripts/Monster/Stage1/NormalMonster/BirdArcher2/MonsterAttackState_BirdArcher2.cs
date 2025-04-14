using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterAttackState_BirdArcher2 : MonsterAttackStateBase
{
    protected new BirdArcher2 monster;
    private MonsterAnimController monsterAnimController;

    ContactFilter2D filter;
    public MonsterAttackState_BirdArcher2(MonsterStateMachine _stateMachine, Player _player, BirdArcher2 _monster) : base(_stateMachine, _player, _monster)
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
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.farAttack);
        monster.monsterAnimController.SetAnim(MonsterAnimState.Attack, monster.CheckDir());

        Vector3 dir = player.transform.position - monster.transform.position;
        for (int i = 0; i < monster.shootNum; i++)
        {
            //Create Bullet
            var bulletObj = MonsterPool.instance.pool.Get();
            var bulletComponent = bulletObj.GetComponent<MonsterBullet>();
            bulletObj.transform.position = monster.transform.position;

            Vector3 shootDir = Quaternion.AngleAxis(Random.Range(-monster.shootAngleOffset, monster.shootAngleOffset), Vector3.forward) * dir;
            float bulletSpeed = Random.Range(monster.bulletSpeedRange.x, monster.bulletSpeedRange.y);
            bulletComponent.BulletInitialize(shootDir, bulletSpeed);
        }
        yield return new WaitForSeconds(monster.reloadDelay);

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.chaseDist)
            stateMachine.ChangeState(monster.chaseState);
        else
            stateMachine.ChangeState(monster.idleState);
    }
}
