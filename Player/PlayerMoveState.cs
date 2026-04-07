using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
    }



    public override void Update()
    {
        base.Update();

        if (JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
        else if (Mathf.Abs(MoveInput.x) < 0.1f)
        {
            player.ChangeState(player.idleState);
        }
        else if (player.isGrounded && RunPressed && MoveInput.y <-.1f)
        {
            player.ChangeState(player.slideState);
        }
        else
        {
            anim.SetBool("isWalking", !RunPressed);
            anim.SetBool("isRunning", RunPressed);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        float speed = RunPressed ? player.runSpeed : player.walkSpeed;
        player.rb.linearVelocity = new Vector2(speed * player.facingDirection, player.rb.linearVelocity.y);
        player.Move();
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("isRunning", false);
        anim.SetBool("isWalking", false);
    }
}
