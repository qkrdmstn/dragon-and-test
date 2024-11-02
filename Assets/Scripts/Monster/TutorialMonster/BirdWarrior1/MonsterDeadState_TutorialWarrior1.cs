using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterDeadState_TutorialWarrior1 : MonsterDeadStateBase
{
    protected new TutorialBirdWarrior1 monster;
    private TutorialInteraction tutorial;

    public MonsterDeadState_TutorialWarrior1(MonsterStateMachine _stateMachine, Player _player, TutorialBirdWarrior1 _monster, TutorialInteraction _tutorial) : base(_stateMachine, _player, _monster)
    {
        monster = _monster;
        tutorial = _tutorial;
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
        //if(monster.haveAnim)
        //    monster.monsterAnimController.SetAnim(MonsterAnimState.Death, monster.CheckDir());

        //float sec = Mathf.Clamp(monster.deadDuration, 0f, 0.7f);
        //yield return new WaitForSeconds(sec);

        //if (monster.IsEffectSpawner())
        //{
        //    monster.ItemDrop();
        //}
        //yield return new WaitForSeconds(0.7f - sec);

        //GameObject.Destroy(monster.gameObject);


        if (monster.myHwatuNum > 0)
            monster.HwatuObjectDrop();

        tutorial.monsters[(int)monster.myNum].isKilled = true;
        SoundManager.instance.SetEffectSound(SoundType.Monster, MonsterSfx.Dead);

        monster.gameObject.SetActive(false);
    }
}
