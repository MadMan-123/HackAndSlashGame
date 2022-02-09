using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{

    [SerializeField] float MaxHealth = 100;
    [SerializeField] float Health = 0;
    private TextMeshPro HealthText;
    private GameObject Player;

    void Start()
    {
        Health = MaxHealth;
        HealthText = GetComponentInChildren<TextMeshPro>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        HealthText.text = Health.ToString();
        HealthText.transform.LookAt((Player.transform));
    }

    public void TakeDamage(float fDamage)
    {
        Health -= fDamage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

}
