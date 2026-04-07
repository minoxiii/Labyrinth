using UnityEngine;

public class PlayerSlideState : PlayerState
{
    private float slideTimer;
    private float slideStopTime;

    public PlayerSlideState(Player player) : base(player) { }   

    public override void Enter()
    {
        base.Enter();
        slideTimer = player.slideDuration;
        slideStopTime = 0;  

        player.SetColliderSlide();
        player.anim.SetBool("isSliding", true);
    }

    public override void Update()
    {
        base.Update();

        if (slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;
        }
        else if (slideStopTime <= 0)
        {
            slideStopTime = player.slideStopDuration;
        }
        else 
        {
            slideStopTime -= Time.deltaTime;

            if (slideStopTime <= 0)
            {
                if (player.CheckForCeiling() || MoveInput.y <= -.1f)
                    player.ChangeState(player.crouchState);
                else
                    player.ChangeState(player.idleState);
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (slideTimer > 0)
        {
            rb.linearVelocity = new Vector2(player.slideSpeed * player.facingDirection, rb.linearVelocity.y);
        }
        else 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.SetColliderNormal();
        anim.SetBool("isSliding", false);
    }
}
