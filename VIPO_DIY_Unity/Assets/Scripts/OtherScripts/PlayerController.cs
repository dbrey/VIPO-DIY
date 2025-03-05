using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Parametros basicos jugador
    public float moveSpeed = 10.0f;
    public float smoothTurnTime = 0.1f;
    public float smoothTurnVel = 0.1f;
    [SerializeField] Transform cam;
    public void assignCam(Transform newCam) { cam = newCam; }

    public float jumpForce = 2.0f;
    public float gravity = -10;
    public float pushPower = 2.0f;     // this script pushes all rigidbodies that the character touches
    Vector3 velocity;

    public float jumpCooldown = 2.0f;

    private CharacterController cc;
    private Animator animator;

    private bool canJump = true;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {

        // Documentation later
        #region Movement
        
        // Get the player's input
        Vector3 moveDirection;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        // Move the player
        if (moveDirection.sqrMagnitude >= 0.1f)
        {
            animator.SetBool("IsMoving", true);
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothTurnVel, smoothTurnTime);


            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            cc.Move(moveDir * moveSpeed * Time.deltaTime);

        }
        else
            animator.SetBool("IsMoving", false);


        velocity.y += gravity * Time.deltaTime;
        #endregion

        // Jump
        #region
        // Jump if the player presses the jump button and the cooldown is over
        if (canJump && (Input.GetButtonDown("Jump")))
        {
            animator.SetBool("IsJumping", true);

            velocity.y = Mathf.Sqrt(jumpForce * 2 * -gravity);

            canJump = false;
        }

        cc.Move(velocity * Time.deltaTime);

        if (cc.isGrounded)
        {
            canJump = true;
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsJumping", false);

        }
        else
            animator.SetBool("IsGrounded", false);
        #endregion
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // If you want your character to do something specific when it hits an specific object, do it here
        switch (hit.gameObject.name)
        {
            case "RaidToWhoCube(Clone)":
                hit.gameObject.GetComponent<AssignRaidToCube>().collisionWithPlayer();
                break;
        }

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.linearVelocity = pushDir * pushPower;
    }
}
