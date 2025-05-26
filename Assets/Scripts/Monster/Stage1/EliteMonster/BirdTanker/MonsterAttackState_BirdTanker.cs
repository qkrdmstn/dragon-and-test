using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterAttackState_BirdTanker : MonsterAttackStateBase
{
    protected new BirdTanker monster;
    private MonsterAnimController monsterAnimController;

    ContactFilter2D filter;
    public MonsterAttackState_BirdTanker(MonsterStateMachine _stateMachine, Player _player, BirdTanker _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;

        if(monster.haveAnim)
            monsterAnimController = monster.monsterAnimController;
    }

    public override void Enter()
    {
        base.Enter();

        monster.SetSpeed(0.0f);
        attackCoroutine = monster.StartCoroutine(Attack());
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

    IEnumerator Attack()
    {
        //Dash Start
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.owlWing);
        monster.monsterAnimController.SetAnim(TankerAnimState.Attack1, monster.CheckDir());
        yield return new WaitForSeconds(monster.dashReadyDuration);


        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.owlDash);
        Vector3 dir = player.transform.position - monster.transform.position;
        dir.Normalize();
        monster.monsterAnimController.SetAnim(TankerAnimState.Attack2, monster.CheckDir());
        monster.rb.velocity = (dir * monster.dashSpeed);
        float dashDuration = monster.dashDist / monster.dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        //Dash end
        monster.SetSpeed(0.0f);
        monster.monsterAnimController.SetAnim(TankerAnimState.Attack3, monster.CheckDir());

        //Shoot
        for (int i = 0; i < monster.waveNum; i++)
        {
            SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.owlAttack);
            for (int j = 0; j < monster.bulletNumPerWave; j++)
            {
                var bulletGo = MonsterPool.instance.pool.Get();
                var bulletComponent = bulletGo.GetComponent<MonsterBullet>();
                bulletGo.transform.position = monster.transform.position;

                Vector2 shootDir = Quaternion.AngleAxis((360.0f / (float)monster.bulletNumPerWave) * j + (180 / monster.bulletNumPerWave) * (i % 2), Vector3.forward) * Vector3.right;
                bulletComponent.BulletInitialize(shootDir, monster.bulletSpeed);
            }
            yield return new WaitForSeconds(monster.waveInterval);
        }
        yield return new WaitForSeconds(monster.reloadDelay);

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);
        if (dist < monster.chaseDist)
            stateMachine.ChangeState(monster.chaseState);
        else
            stateMachine.ChangeState(monster.idleState);
    }
}
