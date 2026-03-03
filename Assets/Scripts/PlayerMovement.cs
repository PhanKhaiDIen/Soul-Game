using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movespeed = 10f;
    public float mouseSensitivity = 2f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2.5f;   // Độ cao cú nhảy
    public float gravity = -19.62f;  // Trọng lực (Sekiro rơi nhanh nên để -19.62 thay vì -9.81)

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;        // Vận tốc rơi tự do
    private bool isGrounded;         // Kiểm tra chạm đất

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. KIỂM TRA CHẠM ĐẤT
        // CharacterController có sẵn thuộc tính isGrounded rất tiện lợi
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ép nhẹ nhân vật xuống đất để giữ ổn định
        }

        // 2. DI CHUYỂN WASD
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveY;
        controller.Move(move * movespeed * Time.deltaTime);

        // 3. NHẢY (Bấm phím Space)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Công thức vật lý để tính lực nhảy dựa trên độ cao: v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 4. ÁP DỤNG TRỌNG LỰC (Rơi tự do)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. XOAY CAMERA
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (Camera.main != null)
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}