using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void Enter()
    {
        player.anim.SetBool("isIdle", true);
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (JumpPressed)
        {
            JumpPressed = false;
            player.ChangeState(player.jumpState);
        }
        else if (Mathf.Abs(player.moveInput.x) > 0.1f)
        {
            player.ChangeState(player.moveState);
        }
        else if (MoveInput.y < -0.1f)
        {
            player.ChangeState(player.crouchState);
        }
    }

    public override void Exit()
    {
        player.anim.SetBool("isIdle", false);
    }
}