using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    [SerializeField] private Clip.ClipType m_ClipType;

    private bool m_IsPickedUp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BaseWeapon weapon = other.GetComponentInChildren<BaseWeapon>();

            weapon.AddClip(new Clip(m_ClipType));
        }
    }
}
