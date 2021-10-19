using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clip : MonoBehaviour
{
    [SerializeField] private int m_ClipSize = 0;
    
    private int m_CurrentAmountOfBullets = 0;

    public int AmountOfRemainingBullets
    {
        get => m_CurrentAmountOfBullets;
    }

    public Clip()
    {
        m_CurrentAmountOfBullets = m_ClipSize;
    }

    public Clip(int amountOfBullets)
    {
        m_CurrentAmountOfBullets = amountOfBullets;
    }

    private void Awake()
    {
        m_CurrentAmountOfBullets = m_ClipSize;
    }

    public void Fire()
    {
        // check if we can fire
        if (m_CurrentAmountOfBullets <= 0)
            return;

        // decrement how many bullets we have
        if (--m_CurrentAmountOfBullets <= 0)
            m_CurrentAmountOfBullets = 0; // we cant have negative bullets
    }

    public bool CanFire()
    {
        return m_CurrentAmountOfBullets > 0;
    }
}
