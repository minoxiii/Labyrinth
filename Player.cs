using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public PlayerInput playerInput;
    public Vector2 moveInput;
    public int facingDirection=1;

    void Update(){
        Flip();
    }

    void FixedUpdate(){
        float targetSpeed = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed,rb.linearVelocity.y);
    }

    void Flip()
    {
        if(moveInput.x>0.1f){
    facingDirection=1;
    }
    else if(moveInput.x<-0.1f){
        facingDirection=-1;
    }
    transform.localScale=new Vector3(facingDirection,1,1);
    }

    public void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
    }
}
