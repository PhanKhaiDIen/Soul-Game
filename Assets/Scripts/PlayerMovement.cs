using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;

    [Header("Dodge Settings")]
    public float dodgeForce = 20f;      // Lực lướt
    public float dodgeDuration = 0.6f;   // Thời gian diễn ra cú lướt
    public float dodgeCooldown = 0.6f;   // Thời gian nghỉ giữa 2 lần né
    private bool isDodging = false;
    private float lastDodgeTime;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2.5f;
    public float gravity = -35f;        // Trọng lực mạnh để chân bám đất tốt

    private CharacterController controller;
    private PlayerStats stats;          // Yêu cầu phải có script PlayerStats trên cùng Object
    private Vector3 velocity;
    private Vector3 moveDirection;
    private bool isGrounded;
    private Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponentInChildren<Animator>();

        // Khóa chuột
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Kiểm tra mặt đất
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Nếu đang né (Dodge) thì thoát Update, không cho phép di chuyển hay nhảy
        if (isDodging) return;

        // 3. Lấy Input di chuyển
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        // Tính hướng di chuyển theo hướng mặt nhân vật
        moveDirection = (transform.forward * inputY + transform.right * inputX).normalized;

        // 4. Cập nhật Animator cho Blend Tree (MoveX, MoveY)
        if (anim != null)
        {
            anim.SetFloat("MoveX", inputX, 0.1f, Time.deltaTime);
            anim.SetFloat("MoveY", inputY, 0.1f, Time.deltaTime);
            anim.SetBool("IsRunning", moveDirection.magnitude > 0.1f);
        }

        // 5. Gọi các hàm xử lý
        HandleMovement();
        HandleJump();
        HandleDodge();
        ApplyGravity();
    }

    void HandleMovement()
    {
        float speed = walkSpeed;

        // Nhấn Shift trái để chạy nhanh (tốn Stamina)
        if (Input.GetKey(KeyCode.LeftShift) && moveDirection.magnitude > 0.1f)
        {
            if (stats != null && stats.currentStamina > 0)
            {
                speed = runSpeed;
                stats.UseStamina(10f * Time.deltaTime);
            }
        }

        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    void HandleDodge()
    {
        // Bấm C để né (Chỉ khi chạm đất, không đang né, và hết Cooldown)
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && !isDodging && Time.time > lastDodgeTime + dodgeCooldown)
        {
            bool canDodge = (stats == null) || stats.UseStamina(20f);

            if (canDodge)
            {
                StartCoroutine(DodgeRoutine());
            }
        }
    }

    IEnumerator DodgeRoutine()
    {
        isDodging = true;
        if (anim != null) anim.SetTrigger("Dodge");

        // Chốt hướng né
        Vector3 finalDodgeDir = moveDirection;
        if (finalDodgeDir == Vector3.zero) finalDodgeDir = -transform.forward;

        float startTime = Time.time;

        while (Time.time < startTime + dodgeDuration)
        {
            // Tính tỷ lệ thời gian đã trôi qua (từ 0 đến 1)
            float t = (Time.time - startTime) / dodgeDuration;

            // Dùng đường cong nhẹ: lúc đầu nhanh, về sau chậm dần (giống như thực tế)
            float speedMultiplier = Mathf.Lerp(1.5f, 0.2f, t);

            controller.Move(finalDodgeDir * dodgeForce * speedMultiplier * Time.deltaTime);
            yield return null;
        }

        isDodging = false;
        lastDodgeTime = Time.time;
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            bool canJump = (stats == null) || stats.UseStamina(15f);

            if (canJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (anim != null) anim.SetTrigger("Jump");
            }
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}