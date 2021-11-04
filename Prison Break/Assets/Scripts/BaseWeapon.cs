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

    private Text m_CurrentAmmoText = null;
    private Text m_TotalAmmoText = null;
    private bool m_HasShotBullet = false;
    private float m_FireTimer = 0f;

    private void Start()
    {
        /* Check if there were clips added in the editor, since Awake() / Start() does not get called on them */
        foreach (Clip clip in m_Clips)
            if (!clip.isActiveAndEnabled)
                clip.Start();

        /* Put AimPoint at Reticle's position */
        var test = GetComponentInChildren<Image>().transform.position;
        test.z += 50f; // Make sure the aim point is far in front of our player
        m_AimPoint.position = Camera.main.ScreenToWorldPoint(test);

        /* Get the Text objects */
        Text[] textObjects = GetComponentsInChildren<Text>();
        m_CurrentAmmoText = textObjects[0];
        m_TotalAmmoText = textObjects[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FireTimer >= 0f)
            m_FireTimer -= Time.deltaTime;

        if (m_FireTimer < 0f)
            FireBullet();

        m_HasShotBullet = false;

        UpdateBulletTextUI();
    }

    public void AddClip(Clip clip)
    {
        if (clip.GetClipType == m_ClipType)
            m_Clips.Add(clip);
    }

    public void Reload()
    {
        // do we have any clips to reload with?
        if (m_Clips.Count <= 1)
            return;

        /* Our clip is already fully loaded */
        if (m_Clips[0].AmountOfRemainingBullets == m_Clips[0].GetClipSize)
            return;

        Clip clip = m_Clips[0];
        m_Clips.RemoveAt(0);

        if (clip.AmountOfRemainingBullets >= 0)
            m_Clips.Add(clip);

        /* Make sure the front clip is filled */
        if (m_Clips[0].AmountOfRemainingBullets <= m_Clips[0].GetClipSize)
        {
            if (m_Clips.Count > 1) /* We can only try to fill it if there are any other remaining clips */
            {
                int neededBullets = m_Clips[0].GetClipSize - m_Clips[0].AmountOfRemainingBullets;
                for (int i = 1; i < m_Clips.Count; ++i)
                {
                    /* Does the clip have bullets? */
                    if (m_Clips[i].AmountOfRemainingBullets > 0)
                    {
                        /* Only give as many bullets are needed */
                        if (m_Clips[i].AmountOfRemainingBullets >= neededBullets)
                        {
                            m_Clips[0].AmountOfRemainingBullets = neededBullets + m_Clips[0].AmountOfRemainingBullets;
                            m_Clips[i].AmountOfRemainingBullets = m_Clips[i].AmountOfRemainingBullets - neededBullets;
                        }
                        else /* The clip doesn't have enough ammo, so give it all the ammo it has remaining */
                        {
                            m_Clips[0].AmountOfRemainingBullets = m_Clips[i].AmountOfRemainingBullets + m_Clips[0].AmountOfRemainingBullets;
                            neededBullets -= m_Clips[i].AmountOfRemainingBullets; /* Decrement the amount of bullets needed */
                            m_Clips[i].AmountOfRemainingBullets = 0;
                        }
                    }

                    /* Check if the clip has been filled */
                    if (m_Clips[0].AmountOfRemainingBullets == m_Clips[0].GetClipSize)
                        break;
                }
            }
        }

        /* Check if any clip has empty ammo, if it does, remove it */
        m_Clips.RemoveAll( clip => clip.AmountOfRemainingBullets == 0 );
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

    private void UpdateBulletTextUI()
    {
        m_CurrentAmmoText.text = m_Clips[0].AmountOfRemainingBullets.ToString();

        int totalAmmo = 0;
        for (int i = 1; i < m_Clips.Count; ++i)
            totalAmmo += m_Clips[i].AmountOfRemainingBullets;

        m_TotalAmmoText.text = totalAmmo.ToString();
    }
}
