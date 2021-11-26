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
    private string m_MouseXAxis = "MouseX";
    private string m_MouseYAxis = "MouseY";
    private string m_InfraredScannerAxis = "Scanning";
    private string m_ReloadingAxis = "Reloading";

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
        HandleReloadingInput();
    }

    private void HandlePlayerMovementInput()
    {
        m_PlayerMovementScript.DesiredVelocity = Input.GetAxisRaw(m_HorizontalKeyboardAxis) * m_PlayerMovementScript.Rigidbody.transform.right
            + Input.GetAxisRaw(m_VerticalKeyboardAxis) * m_PlayerMovementScript.Rigidbody.transform.forward;
    }

    private void HandlePlayerFiringInput()
    {
        if (Input.GetAxisRaw(m_PlayerFiringAxis) > 0f)
            m_BaseWeaponScript.FireBullet(~LayerMask.GetMask("Player"));
    }

    private void HandlePlayerAimInput()
    {
        m_PlayerAimScript.CurrentRotation = new Vector2(Input.GetAxisRaw(m_MouseXAxis), Input.GetAxisRaw(m_MouseYAxis));
    }

    private void HandleInfraredScannerInput()
    {
        if (m_InfraredScannerScript != null && Input.GetAxisRaw(m_InfraredScannerAxis) > 0f)
            m_InfraredScannerScript.ScanAhead();
    }

    private void HandleReloadingInput()
    {
        if (Input.GetAxisRaw(m_ReloadingAxis) > 0f)
            m_BaseWeaponScript.Reload();
    }
}
