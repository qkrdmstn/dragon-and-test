using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterAttackState_BirdWarrior1 : MonsterState
{
    protected BirdWarrior1 monster;

    public MonsterAttackState_BirdWarrior1(MonsterStateMachine _stateMachine, Player _player, BirdWarrior1 _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;
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
        monster.monsterAnimController.SetAnim(MonsterAnimState.Attack, monster.CheckDir());
        for (int i = 0; i < monster.shootNum; i++)
        {
            Vector3 dir = player.transform.position - monster.transform.position;

            GameObject aura = GameObject.Instantiate(monster.swordAura, monster.transform.position, Quaternion.identity);
            MonsterBullet auraScript = aura.GetComponent<MonsterBullet>();
            SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.nearAttack);
            auraScript.BulletInitialize(dir);
            yield return new WaitForSeconds(monster.shootDelay);
        }

        stateMachine.ChangeState(monster.chaseState);
    }
}
