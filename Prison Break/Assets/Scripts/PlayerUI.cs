using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private BaseWeapon m_PlayerWeapon;
    private Text m_CurrentAmmoText = null;
    private Text m_TotalAmmoText = null;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerWeapon = gameObject.GetComponentInChildren<BaseWeapon>();

        /* Get the Text objects */
        Text[] textObjects = GetComponentsInChildren<Text>();

        for (int i = 0; i < textObjects.Length; ++i)
        {
            if (textObjects[i].gameObject.name.Equals("CurrentAmmo"))
                m_CurrentAmmoText = textObjects[i];
            else if (textObjects[i].gameObject.name.Equals("TotalAmmo"))
                m_TotalAmmoText = textObjects[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBulletTextUI();
    }

    private void UpdateBulletTextUI()
    {
        m_CurrentAmmoText.text = m_PlayerWeapon.GetClips[0].AmountOfRemainingBullets.ToString();

        int totalAmmo = 0;
        for (int i = 1; i < m_PlayerWeapon.GetClips.Count; ++i)
            totalAmmo += m_PlayerWeapon.GetClips[i].AmountOfRemainingBullets;

        m_TotalAmmoText.text = totalAmmo.ToString();
    }
}
