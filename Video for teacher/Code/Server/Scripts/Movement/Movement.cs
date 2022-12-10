using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serverside
public class Movement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private User user;

    [Header("Stats")]
    public float storedMoveSpd = 0;
    public float speed = 10;
    public float jumpForce = 50;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Space]
    [Header("Bools")]
    public bool canMove;

    private bool didTeleport;
    private bool groundTouch;
    private bool[] inputs;

    private void OnValidate()
    {
        if (rb == null) { rb = GetComponent<Rigidbody2D>(); }
        if (user == null) { user = GetComponent<User>(); }
        if (controller == null) { controller = GetComponent<CharacterController>(); }
    }

    private void Start()
    {
        groundTouch = true;
        storedMoveSpd = speed;
    }

    public void SetInput(bool[] inputs)
    {
        this.inputs = inputs;
    }

    public void Teleport(Vector2 toPosition)
    {
        bool isEnabled = controller.enabled;
        controller.enabled = false;
        transform.position = toPosition;
        controller.enabled = isEnabled;

        didTeleport = true;
    }

    private void FixedUpdate()
    {
        Vector2 inputDirection = new Vector2(0f, rb.velocity.y);
        bool jump = false;

        if (inputs[0]) { inputDirection.x -= 1; }
        if (inputs[1]) { inputDirection.y += 1; }
        if (inputs[2]) { jump = true; }

        Move(inputDirection, jump);
    }

    private void Move(Vector2 dir, bool jump)
    {
        Vector2 moveDirection = new Vector2(dir.x, rb.velocity.y);

        if (dir.x > 0f) { transform.localScale = new Vector3(1f, 1f, 1f); } 
        else { transform.localScale = new Vector3(1f, 1f, -1f); }

        dir.x *= speed;
        rb.velocity = dir;
        
        if(jump & groundTouch)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.velocity += Vector2.up * jumpForce;

            groundTouch = false;
        } else if (jump & rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        } else if (!jump & rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        SendMovement();
    }

    #region Messages

    private void SendMovement()
    {
        if (NetworkManager.Singleton.CurrentTick % 2 != 0)
        {
            return;
        }
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerMovement);
        message.AddUShort(user.Id);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddBool(didTeleport);
        message.AddVector2(transform.position);
        message.AddVector2(transform.localScale);
        message.AddUShort(user.Id);
        NetworkManager.Singleton.Server.SendToAll(message);

        didTeleport = false;
    }

    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if(User.inGameList.TryGetValue(fromClientId, out User user))
        {
            user.GetComponent<Movement>().SetInput(message.GetBools());
        }   
    }

    #endregion
}
