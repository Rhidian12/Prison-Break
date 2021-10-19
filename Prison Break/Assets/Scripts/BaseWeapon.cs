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
    [SerializeField] protected Transform m_AimPoint;
    [SerializeField] protected Queue<Clip> m_Clips = new Queue<Clip>();

    public Queue<Clip> AmountOfClips
    {
        get => m_Clips;
    }

    //private Image m_Reticle;
    //private Camera m_Camera;
    private float m_FireTimer = 0f;

    private void Start()
    {
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

    public void Reload()
    {
        // do we have any clips to reload with?
        if (m_Clips.Count == 0)
            return;

        // reload
        m_Clips.Enqueue(new Clip());

        Clip clip = m_Clips.Dequeue();
        // do we have any bullets remaining in the clip we're about to get rid of?
        if (clip.AmountOfRemainingBullets > 0)
            m_Clips.Enqueue(new Clip(clip.AmountOfRemainingBullets)); // get a new clip with the remaining bullets
    }

    private void FireBullet()
    {
        // Check if we can fire the gun
        if (m_Clips.Count == 0)
            return;

        Clip clip = m_Clips.Peek();

        // If the player has pressed the trigger
        if (m_HasShotBullet)
        {
            if (clip.CanFire())
            {
                GameObject bullet = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.position, Quaternion.identity);
                bullet.GetComponentInChildren<BulletMovement>().Velocity = (m_AimPoint.position - m_BulletSpawnPoint.position).normalized;

                m_FireTimer += 1f / m_FireRate;

                clip.Fire();
            }
        }
    }
}
