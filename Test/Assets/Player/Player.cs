using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ItemPickup))]
public class Player : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float MaxHealth = 100f;
    [SerializeField] float Health = 0f;
    [SerializeField] float HealthRegen = 0.5f;
    [SerializeField] float HealthRegenDelay = 1f;
    [Header("UI")]
    [SerializeField] Canvas PlayerCanvas;
    private Scrollbar HealthBar;
    private bool bIsDead = false;

    void Start()
    {
        Health = MaxHealth;
        HealthBar = PlayerCanvas.GetComponentInChildren<Scrollbar>();
        UpdatePlayerHealthBar();

    }

    void Update()
    {
        if (!bIsDead)
        {
            if (Health < MaxHealth)
            {
                StartCoroutine(RegenHealth());
            }
            StartCoroutine(RegenHealth());

        }

    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            OnPlayerDeath();
        }
        UpdatePlayerHealthBar();

    }

    void UpdatePlayerHealthBar()
    {
        HealthBar.size = (Health / MaxHealth);
    }
    IEnumerator RegenHealth()
    {
        while (Health < MaxHealth)
        {
            yield return new WaitForSeconds(HealthRegenDelay);
            Health += HealthRegen;
            UpdatePlayerHealthBar();

        }
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        yield return null;

    }


    void OnPlayerDeath()
    {
        Time.timeScale = 0;
        //TODO: Game over screen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }
}
