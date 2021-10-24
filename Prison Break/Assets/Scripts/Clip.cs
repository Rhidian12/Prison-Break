using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clip : MonoBehaviour
{
    [SerializeField] private int m_ClipSize = 0;
    [SerializeField] private ClipType m_ClipType;
    
    private int m_CurrentAmountOfBullets = 0;

    public enum ClipType
    {
        None,
        Pistol
    }

    public int AmountOfRemainingBullets
    {
        get => m_CurrentAmountOfBullets;
    }
    public ClipType GetClipType
    {
        get => m_ClipType;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<BaseWeapon>().AddClip(this);

            Destroy(gameObject);
        }
    }

    public bool CanFire()
    {
        return m_CurrentAmountOfBullets > 0;
    }
}
