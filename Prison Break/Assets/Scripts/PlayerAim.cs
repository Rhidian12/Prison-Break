using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{

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
        const float maxVerticalAngle = 89f;
        const float rotationSpeed = 200f;

        // Rotate the Root of the player over its local Y axis
        m_PlayerTransform.transform.Rotate(0f, m_CurrentRotation.x * rotationSpeed * 0.01f, 0f, Space.Self);

        // Increase the Vertical Camera Angle
        m_CameraVerticalAngle += m_CurrentRotation.y * -1f * 0.01f * rotationSpeed;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -maxVerticalAngle, maxVerticalAngle);

        // Set the Camera its local X angle
        m_Camera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);
    }
}
