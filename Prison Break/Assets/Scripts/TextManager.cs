using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;

public class TextManager : MonoBehaviour
{
    [SerializeField] private List<TextMesh> m_Texts;
    [SerializeField] private List<Transform> m_PositionsToPass;
    [SerializeField] private GameObject m_Player;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_PositionsToPass.Count; ++i)
            if (!m_Texts[i].text.Equals(""))
                if (Vector3.SqrMagnitude(m_PositionsToPass[i].position - m_Player.transform.position) <= 10f)
                    m_Texts[i].text = "";
    }
}
