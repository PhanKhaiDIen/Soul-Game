using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float mouseSensitivity = 2f;

    [Header("Dodge Settings")]
    public float dodgeForce = 15f;
    public float dodgeDuration = 0.2f;
    private bool isDodging = false;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2.5f;
    public float gravity = -19.62f;

    private CharacterController controller;
    private PlayerStats stats;
    private Vector3 velocity;
    private bool isGrounded;
    private Animator anim;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();

        // Luôn khóa chuột khi bắt đầu game
        Cursor.lockState = CursorLockMode.Locked;

        // Tìm Animator ở hiệp sĩ KNIGHTT (con của Cylinder)
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. Lấy Input thô từ bàn phím (A/D là X, W/S là Y)
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (isDodging) return;

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // 2. GỬI DỮ LIỆU VÀO ANIMATOR (SỬA LỖI HƯỚNG TẠI ĐÂY)
        if (anim != null)
        {
            // MoveX nhận giá trị từ A(-1) và D(1)
            // MoveY nhận giá trị từ S(-1) và W(1)
            // 0.1f là Damp Time giúp chuyển dáng mượt, tạo cảm giác có đà
            anim.SetFloat("MoveX", inputX, 0.1f, Time.deltaTime);
            anim.SetFloat("MoveY", inputY, 0.1f, Time.deltaTime);
        }

        HandleMovement(inputX, inputY); // Truyền input vào hàm di chuyển
        HandleJump();
        HandleDodge();
        ApplyGravity();
        HandleCamera();
    }

    void HandleMovement(float x, float y)
    {
        // Lấy hướng tiến và hướng ngang chuẩn của nhân vật
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Đảm bảo hướng di chuyển luôn nằm trên mặt phẳng nằm ngang (loại bỏ Y)
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Tính toán hướng di chuyển cuối cùng
        Vector3 moveDir = (forward * y + right * x).normalized;

        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && moveDir.magnitude > 0.1f)
        {
            if (stats != null && stats.currentStamina > 0)
            {
                speed = runSpeed;
                stats.UseStamina(stats.staminaDrainRate * Time.deltaTime);
            }
        }

        // Di chuyển nhân vật
        controller.Move(moveDir * speed * Time.deltaTime);
    }

    // --- CÁC HÀM CÒN LẠI GIỮ NGUYÊN LOGIC CỦA BẠN ---

    void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && !isDodging)
        {
            if (stats != null && stats.UseStamina(20f))
            {
                StartCoroutine(DodgeRoutine());
            }
        }
    }

    IEnumerator DodgeRoutine()
    {
        isDodging = true;
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 dodgeDir = transform.right * x + transform.forward * y;
        if (dodgeDir.magnitude < 0.1f) dodgeDir = -transform.forward;

        float startTime = Time.time;
        while (Time.time < startTime + dodgeDuration)
        {
            controller.Move(dodgeDir.normalized * dodgeForce * Time.deltaTime);
            yield return null;
        }
        isDodging = false;
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (stats != null && stats.UseStamina(15f))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Xoay thân nhân vật theo trục ngang (Chuột trái/phải)
        transform.Rotate(Vector3.up * mouseX);

        // Xoay Camera lên xuống (Chuột lên/xuống)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}