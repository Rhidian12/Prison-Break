using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseWeapon : MonoBehaviour
{

    [SerializeField] private Transform m_BulletSpawnPoint;
    [SerializeField] private float m_FireRate = 0f;
    [SerializeField] private Transform m_AimPoint;
    [SerializeField] private List<Clip> m_Clips = new List<Clip>();
    [SerializeField] private Clip.ClipType m_ClipType;

    private bool m_HasShotBullet = false;
    private float m_FireTimer = 0f;

    private void Start()
    {
        /* Check if there were clips added in the editor, since Awake() / Start() does not get called on them */
        foreach (Clip clip in m_Clips)
            if (!clip.isActiveAndEnabled)
                clip.Start();
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

    public void AddClip(Clip clip)
    {
        if (clip.GetClipType == m_ClipType)
            m_Clips.Add(clip);
    }

    public void Reload()
    {
        // do we have any clips to reload with?
        if (m_Clips.Count == 0)
            return;

        // reload
        m_Clips.Add(new Clip(m_ClipType));

        Clip clip = m_Clips[0];
        m_Clips.RemoveAt(0);
        // do we have any bullets remaining in the clip we're about to get rid of?
        if (clip.AmountOfRemainingBullets > 0)
            m_Clips.Add(new Clip(clip.AmountOfRemainingBullets, m_ClipType)); // get a new clip with the remaining bullets
    }

    public void Fire()
    {
        m_HasShotBullet = true;
    }

    private void FireBullet()
    {
        // Check if we can fire the gun
        if (m_Clips.Count == 0)
            return;

        Clip clip = m_Clips[0];

        // If the player has pressed the trigger
        if (m_HasShotBullet)
        {
            if (clip.CanFire())
            {
                /* Check if we hit something */
                if (Physics.Raycast(m_BulletSpawnPoint.position, (m_AimPoint.position - m_BulletSpawnPoint.position).normalized, out RaycastHit raycastHit))
                {
                    /* Are we hitting the enemy? Or is the Player getting hit? */
                    if (raycastHit.collider.gameObject.CompareTag("Enemy") || raycastHit.collider.gameObject.CompareTag("Player"))
                    {
                        /* Hurt the Target */
                        raycastHit.collider.gameObject.GetComponent<HealthScript>().RemoveHealth(clip.GetDamage);
                    }
                }

                m_FireTimer += 1f / m_FireRate;

                clip.Fire();
            }
        }
    }
}
