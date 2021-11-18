using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITransition
{
    public EnemyBehaviour m_From;
    public EnemyBehaviour m_To;
    public System.Func<bool> m_Predicate;

    public AITransition(EnemyBehaviour from, EnemyBehaviour to, System.Func<bool> predicate)
    {
        m_From = from;
        m_To = to;
        m_Predicate = predicate;
    }

    public EnemyBehaviour CheckTransition()
    {
        if (m_Predicate())
            return m_To;
        else
            return m_From;
    }
}
