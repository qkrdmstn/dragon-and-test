using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterAttackState_BirdWarrior2 : MonsterState
{
    protected new BirdWarrior2 monster;
    private MonsterAnimController monsterAnimController;
    private GameObject warningObj;

    public MonsterAttackState_BirdWarrior2(MonsterStateMachine _stateMachine, Player _player, BirdWarrior2 _monster) : base(_stateMachine, _player, _monster)
    {
        this.monster = _monster;
        monsterAnimController = monster.monsterAnimController;
        warningObj = monster.warningObject;
    }

    public override void Enter()
    {
        base.Enter();
        monster.SetSpeed(0.0f);

        monster.StartCoroutine(Dash());
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        monster.SetSpeed(0.0f);
    }

    IEnumerator Dash()
    {
        Vector3 dir = player.transform.position - monster.transform.position;
        dir.Normalize();

        // 차징 애니메이션
        monsterAnimController.SetAnim(MonsterAnimState.Attack, monster.CheckDir());

        //차징 로직
        float timer = 0.0f;
        warningObj.SetActive(true);
        while (timer < monster.chargingDelay)
        {
            dir = player.transform.position - monster.transform.position;
            dir.Normalize();
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;
            warningObj.transform.rotation = Quaternion.Euler(0, 0, theta);
            warningObj.transform.position = monster.transform.position + dir * monster.dashDist * 0.4f;

            yield return null;
            timer += Time.deltaTime;
        }
        warningObj.SetActive(false);

        // 공격 애니메이션
        monsterAnimController.SetAnim(MonsterAnimState.Run, monster.CheckDir());
        monsterAnimController.SetAnimSpeed(monster.dashRatio);
        
        //공격 사운드
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.dashAttack);

        //공격 로직
        monster.rb.velocity = (dir * monster.dashSpeed);
        float dashDuration = monster.dashDist / monster.dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        monster.SetSpeed(0.0f);
        monsterAnimController.SetAnimSpeed(1.0f);
        yield return new WaitForSeconds(monster.dashDelay);
        stateMachine.ChangeState(monster.chaseState);
    }
}
