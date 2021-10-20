using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private GameObject m_Player;
    private BaseWeapon m_PlayerWeapon;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = gameObject;
        m_PlayerWeapon = gameObject.GetComponentInChildren<BaseWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
