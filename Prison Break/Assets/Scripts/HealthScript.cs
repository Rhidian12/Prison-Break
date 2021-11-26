using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    [SerializeField] private int m_MaxHealth;

    private int m_CurrentHealth = 0;

    public int GetMaxHealth
    {
        get => m_MaxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealth = m_MaxHealth;    
    }

    private void Update()
    {
        /* Are we dead? */
        if (m_CurrentHealth <= 0)
        {
            /* If the Death Message was Sent, We can just destroy the GameObject */
            Destroy(gameObject);
        }
    }

    public void RemoveHealth(int healthToRemove)
    {
        m_CurrentHealth -= healthToRemove;
    }
}
