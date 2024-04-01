using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    #region Components
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;
    private string animBoolName;
    #endregion

    #region Inputs
    protected static float xInput;
    protected static float yInput;
    #endregion

    protected float stateTimer;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
    }

    public virtual void Update()
    {
        if (player.isInteraction) return;

        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("xVelocity", rb.velocity.x);
        player.anim.SetFloat("yVelocity", rb.velocity.y);

        Vector2 mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        mouseDir.Normalize();
        //Debug.Log(mouseDir);
        player.anim.SetFloat("xDirection", mouseDir.x);
        player.anim.SetFloat("yDirection", mouseDir.y);
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }
}
