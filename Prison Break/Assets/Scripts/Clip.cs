using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clip : MonoBehaviour
{
    [SerializeField] private int m_ClipSize = 0;
    [SerializeField] private int m_Damage = 0;
    [SerializeField] private ClipType m_ClipType;
    
    private int m_CurrentAmountOfBullets = 0;

    public int GetDamage
    {
        get => m_Damage;
    }
    public int AmountOfRemainingBullets
    {
        get => m_CurrentAmountOfBullets;
        set
        {
            if (value <= m_ClipSize)
                m_CurrentAmountOfBullets = value;
        }
    }
    public ClipType GetClipType
    {
        get => m_ClipType;
    }
    public int GetClipSize
    {
        get => m_ClipSize;
    }
    public enum ClipType
    {
        None,
        Pistol
    }

    public Clip(ClipType clipType)
    {
        m_CurrentAmountOfBullets = m_ClipSize;
        m_ClipType = clipType;
    }

    public Clip(int amountOfBullets, ClipType clipType)
    {
        m_CurrentAmountOfBullets = amountOfBullets;
        m_ClipType = clipType;
    }

    public void Start()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponentInChildren<BaseWeapon>().AddClip(this);

            Destroy(gameObject);
        }
    }

    public bool CanFire()
    {
        return m_CurrentAmountOfBullets > 0;
    }
}
