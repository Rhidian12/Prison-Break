using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseWeapon : MonoBehaviour
{
    [HideInInspector] public bool m_HasShotBullet = false;

    [SerializeField] protected Transform m_BulletSpawnPoint;
    [SerializeField] protected GameObject m_BulletPrefab;
    [SerializeField] protected float m_FireRate = 0f;
    [SerializeField] protected int m_ClipSize = 0;

    protected int m_CurrentAmountOfBullets = 0;

    private float m_FireTimer = 0f;

    private void Start()
    {
        Assert.IsNotNull(m_BulletPrefab);
        Assert.IsNotNull(m_BulletSpawnPoint);
        Assert.IsNotNull(m_BulletPrefab.GetComponent<BulletMovement>());

        m_CurrentAmountOfBullets = m_ClipSize;
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
            bullet.GetComponentInChildren<BulletMovement>().Velocity = m_BulletSpawnPoint.forward;

            m_FireTimer += 1f / m_FireRate;

            m_CurrentAmountOfBullets = Mathf.Clamp(m_CurrentAmountOfBullets - 1, 0, m_ClipSize);
        }
    }
}
