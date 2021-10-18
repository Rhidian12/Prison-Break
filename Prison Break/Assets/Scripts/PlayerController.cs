using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement m_PlayerMovementScript;
    [SerializeField] private BaseWeapon m_BaseWeaponScript;
    [SerializeField] private PlayerAim m_PlayerAimScript;

    private InfraredScanner m_InfraredScannerScript;

    private string m_HorizontalKeyboardAxis = "KeyboardHorizontal";
    private string m_VerticalKeyboardAxis = "KeyboardVertical";
    private string m_PlayerFiringAxis = "Firing";
    private string m_MouseX = "MouseX";
    private string m_MouseY = "MouseY";
    private string m_InfraredScanner = "Scanning";

    public InfraredScanner SetInfraredScannerScript
    {
        set
        {
            m_InfraredScannerScript = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerMovementInput();
        HandlePlayerFiringInput();
        HandlePlayerAimInput();
        HandleInfraredScannerInput();
    }

    private void HandlePlayerMovementInput()
    {
        m_PlayerMovementScript.DesiredVelocity = Input.GetAxisRaw(m_HorizontalKeyboardAxis) * m_PlayerMovementScript.Rigidbody.transform.right
            + Input.GetAxisRaw(m_VerticalKeyboardAxis) * m_PlayerMovementScript.Rigidbody.transform.forward;
    }

    private void HandlePlayerFiringInput()
    {
        if (!m_BaseWeaponScript.m_HasShotBullet)
            if (Input.GetAxisRaw(m_PlayerFiringAxis) > 0f)
                m_BaseWeaponScript.m_HasShotBullet = true;
    }

    private void HandlePlayerAimInput()
    {
        m_PlayerAimScript.CurrentRotation = new Vector2(Input.GetAxisRaw(m_MouseX), Input.GetAxisRaw(m_MouseY));
    }

    private void HandleInfraredScannerInput()
    {
        if (Input.GetAxisRaw(m_InfraredScanner) > 0f)
            m_InfraredScannerScript.ScanAhead();
    }
}
