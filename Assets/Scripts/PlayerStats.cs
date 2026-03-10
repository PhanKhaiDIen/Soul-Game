using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 20f;
    public float staminaDrainRate = 30f; // Thêm dòng này để sửa lỗi số 1

    [Header("UI Reference")]
    public Image staminaFillImage;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        if (staminaFillImage != null)
        {
            staminaFillImage.fillAmount = currentStamina / maxStamina;
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }
}