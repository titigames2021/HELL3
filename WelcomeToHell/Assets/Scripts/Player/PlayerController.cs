using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    public float playerSpeed = 2.0f;
   public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;
    private PlayerInput _input;

    private Vector2 move_input_data;
    private Vector2 mouse_input_data;
    private bool check_input;
    private bool jump_input;

    [SerializeField] private float mouseSensitivity = 100f;
    private float verticalRotation = 0f;
    public float distance_;
    public float angle_;

    public LayerMask sensor_layer_;
    public LayerMask obstacles_layer_;
    public Collider detected_object_;
    public GameObject punch_proyectile;
    public Transform fist;
    public bool canJump;
    public bool canShoot;
    private int hits;



    private bool shoot_input;
    private void Awake()
    {

        _input = new PlayerInput();



        //Input shoot

        _input.GeneralMovement.Shoot.performed += shoot_performed =>
        {
            shoot_input = shoot_performed.ReadValueAsButton();
            Debug.Log("punch");
        };
        _input.GeneralMovement.Shoot.canceled += shoot_performed =>
        {
            shoot_input = shoot_performed.ReadValueAsButton();
            canShoot = true;
            
        };



        //Input mov

        _input.GeneralMovement.Move.performed += move_performed =>
        {

            move_input_data = move_performed.ReadValue<Vector2>();
            
        };


        _input.GeneralMovement.Move.canceled += move_performed =>
        {

            move_input_data = move_performed.ReadValue<Vector2>();
           
        };



        //Input aim 
        _input.GeneralMovement.Aim.performed += ctx =>
        {
            mouse_input_data = ctx.ReadValue<Vector2>() * mouseSensitivity;
        };


        _input.GeneralMovement.Aim.canceled += ctx =>
        {
            mouse_input_data = Vector2.zero;
        };


        //Input jump
        _input.GeneralMovement.Jump.performed += jump_performed =>
        {
            
                jump_input = jump_performed.ReadValueAsButton();
              
               
            
           

        };



        _input.GeneralMovement.Jump.canceled += jump_performed =>
        {

            jump_input = jump_performed.ReadValueAsButton();
           
            canJump = true;

        };









    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {


        // Get the direction the player is facing
        Vector3 direction = transform.forward;
        direction.y = 0f;
        direction = direction.normalized;

        // Calculate movement direction
        Vector3 moveDirection = direction * move_input_data.y + transform.right * move_input_data.x;
        moveDirection = moveDirection.normalized * playerSpeed;

        // Move the player
        controller.Move(moveDirection * Time.deltaTime);

        // Rotate the player based on mouse input
        float horizontalRotation = mouse_input_data.x;
        transform.Rotate(Vector3.up * horizontalRotation);

        verticalRotation -= mouse_input_data.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);



       





        //Detect that an enemy enters melee range. 


        Collider[] colliders = Physics.OverlapSphere(transform.position, distance_, sensor_layer_);

        detected_object_ = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider single_collider = colliders[i];

            Vector3 dir_to_collider = Vector3.Normalize(single_collider.bounds.center - transform.position);

            // Angle -> coste alto / alternativa Dot
            float angle_to_collider = Vector3.Angle(transform.forward, dir_to_collider);

            if (angle_to_collider < angle_)
            {
                if (!Physics.Linecast(transform.position, single_collider.bounds.center, out RaycastHit hit, obstacles_layer_))
                {
                    Debug.DrawLine(transform.position, single_collider.bounds.center, Color.green);
                    detected_object_ = single_collider;
                    break;
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                }
            }
        }


        if(shoot_input && detected_object_!= null)
        {

            if (canShoot)
            {
                Debug.Log("hits: "+hits);
                hits++;

                canShoot = false;

                EnemyController enemyController =detected_object_.GetComponent<EnemyController>();

                enemyController.PunchHit();
            }
           

           /*
            GameObject p_pro = Instantiate(punch_proyectile, fist.position, Quaternion.identity);

            Rigidbody PproRb = punch_proyectile.GetComponent<Rigidbody>();
            PproRb.AddForce(transform.forward * 1.3f, ForceMode.Impulse);
           */

        }




    }

    private void FixedUpdate()
    {

        // Jump, when input is canceled, the player can jump if is grounded 

        if (jump_input && groundedPlayer)
        {
            if (canJump) { playerVelocity.y = jumpHeight * -1.0f * gravityValue; canJump = false; }

        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;


        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, distance_);

        Vector3 right_dir = Quaternion.Euler(0.0f, angle_, 0.0f) * transform.forward;
        Gizmos.DrawRay(transform.position, right_dir * distance_);

        Vector3 left_dir = Quaternion.Euler(0.0f, -angle_, 0.0f) * transform.forward;
        Gizmos.DrawRay(transform.position, left_dir * distance_);
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }


   
}
