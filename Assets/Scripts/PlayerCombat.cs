using UnityEngine;

public partial class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;      // Kéo Player vào đây
    public GameObject sword;       // Kéo khối Cube "kiếm" vào đây
    public float attackStamina = 15f;
    public float attackDuration = 0.3f;
    private bool isAttacking = false;

    void Start()
    {
        sword.SetActive(false); // Lúc đầu giấu kiếm đi
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        // Kiểm tra đủ Stamina mới cho đánh
        if (stats.currentStamina >= attackStamina)
        {
            stats.UseStamina(attackStamina);
            StartCoroutine(PerformAttack());
        }
    }

    System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        sword.SetActive(true); // Hiện kiếm ra

        // Giả lập vung kiếm (xoay nhẹ hoặc đẩy ra trước)
        yield return new WaitForSeconds(attackDuration);

        sword.SetActive(false); // Ẩn kiếm sau khi chém xong
        isAttacking = false;
    }
}