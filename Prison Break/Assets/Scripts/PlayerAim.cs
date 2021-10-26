using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private float m_RotationSpeed = 150f;
    [SerializeField] private float m_MaxCameraXAngle = 89f;
    [SerializeField] private Camera m_Camera;

    private float m_CameraVerticalAngle = 0f;
    private Transform m_PlayerTransform;
    private Vector2 m_CurrentRotation;

    public Vector2 CurrentRotation
    {
        get => m_CurrentRotation;
        set
        {
            m_CurrentRotation = value;
        }
    }

    private void Awake()
    {
        m_PlayerTransform = gameObject.transform.parent.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Rotate the Root of the player over its local Y axis
        m_PlayerTransform.transform.Rotate(0f, m_CurrentRotation.x * m_RotationSpeed * 0.01f, 0f, Space.Self);

        // Increase the Vertical Camera Angle
        m_CameraVerticalAngle += m_CurrentRotation.y * -1f * 0.01f * m_RotationSpeed;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -m_MaxCameraXAngle, m_MaxCameraXAngle);

        // Set the Camera its local X angle
        m_Camera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);
    }
}
