using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || damageAmount <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damageAmount);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetBool("Death", true);
    }
}