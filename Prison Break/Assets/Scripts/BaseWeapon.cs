using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseWeapon : MonoBehaviour
{
    [HideInInspector] public bool m_HasShotBullet = false;

    [SerializeField] protected Transform m_BulletSpawnPoint;
    [SerializeField] protected GameObject m_BulletPrefab;
    [SerializeField] protected float m_FireRate = 0f;
    [SerializeField] protected int m_ClipSize = 0;
    [SerializeField] protected Transform m_AimPoint;

    protected int m_CurrentAmountOfBullets = 0;

    //private Image m_Reticle;
    //private Camera m_Camera;
    private float m_FireTimer = 0f;

    private void Start()
    {
        m_CurrentAmountOfBullets = m_ClipSize;

        //m_Reticle = GetComponentInChildren<Image>();
        //m_Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FireTimer >= 0f)
            m_FireTimer -= Time.deltaTime;

        if (m_FireTimer < 0f)
            FireBullet();

        m_HasShotBullet = false;
    }

    private void FireBullet()
    {
        if (m_CurrentAmountOfBullets <= 0)
            return;

        if (m_HasShotBullet)
        {
            GameObject bullet = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, Quaternion.identity);
            bullet.GetComponentInChildren<BulletMovement>().Velocity = (m_AimPoint.position - m_BulletSpawnPoint.position).normalized;

            m_FireTimer += 1f / m_FireRate;

            m_CurrentAmountOfBullets = Mathf.Clamp(m_CurrentAmountOfBullets - 1, 0, m_ClipSize);
        }
    }
}
