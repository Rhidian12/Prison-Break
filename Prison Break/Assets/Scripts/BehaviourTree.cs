using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourState
{
    Failure,
    Running,
    Success
}

public abstract class IBehaviour
{
    protected BehaviourState m_CurrentState = BehaviourState.Failure;

    public abstract BehaviourState Execute(Blackboard blackboard);
}

public abstract class BehaviourComposite : IBehaviour
{
    protected List<IBehaviour> m_Behaviours;

    public BehaviourComposite(List<IBehaviour> behaviours)
    {
        m_Behaviours = behaviours;
    }

    public override abstract BehaviourState Execute(Blackboard blackboard);
}

public sealed class BehaviourSelector : BehaviourComposite
{
    public BehaviourSelector(List<IBehaviour> behaviours)
        : base(behaviours)
    { }

    public override BehaviourState Execute(Blackboard blackboard)
    {
        foreach (IBehaviour behaviour in m_Behaviours)
        {
            m_CurrentState = behaviour.Execute(blackboard);

            switch (m_CurrentState)
            {
                case BehaviourState.Failure:
                    {
                        continue;
                    }
                case BehaviourState.Success:
                    {
                        return m_CurrentState;
                    }
                case BehaviourState.Running:
                    {
                        return m_CurrentState;
                    }
            }
        }
        return BehaviourState.Failure;
    }
}

public sealed class BehaviourSequence : BehaviourComposite
{
    public BehaviourSequence(List<IBehaviour> behaviours)
        : base(behaviours)
    { }

    public override BehaviourState Execute(Blackboard blackboard)
    {
        foreach (IBehaviour behaviour in m_Behaviours)
        {
            m_CurrentState = behaviour.Execute(blackboard);

            switch (m_CurrentState)
            {
                case BehaviourState.Failure:
                    {
                        return m_CurrentState;
                    }
                case BehaviourState.Running:
                    {
                        return m_CurrentState;
                    }
                case BehaviourState.Success:
                    {
                        continue;
                    }
            }
        }
        return m_CurrentState = BehaviourState.Success;
    }
}

public sealed class BehaviourConditional : IBehaviour
{
    private System.Func<Blackboard, bool> m_Condition;

    public BehaviourConditional(System.Func<Blackboard, bool> condition)
    {
        m_Condition = condition;
    }

    public override BehaviourState Execute(Blackboard blackboard)
    {
        if (m_Condition(blackboard))
            return m_CurrentState = BehaviourState.Success;
        else
            return m_CurrentState = BehaviourState.Failure;
    }
}

public sealed class BehaviourInvertedConditional : IBehaviour
{
    private System.Func<Blackboard, bool> m_Condition;

    public BehaviourInvertedConditional(System.Func<Blackboard, bool> condition)
    {
        m_Condition = condition;
    }

    public override BehaviourState Execute(Blackboard blackboard)
    {
        if (!m_Condition(blackboard))
            return m_CurrentState = BehaviourState.Success;
        else
            return m_CurrentState = BehaviourState.Failure;
    }
}

public sealed class BehaviourAction : IBehaviour
{
    private System.Func<Blackboard, BehaviourState> m_Action;

    public BehaviourAction(System.Func<Blackboard, BehaviourState> action)
    {
        m_Action = action;
    }

    public override BehaviourState Execute(Blackboard blackboard)
    {
        return m_CurrentState = m_Action(blackboard);
    }
}

public sealed class BehaviourTree
{
    private BehaviourState m_CurrentState = BehaviourState.Failure;
    private Blackboard m_Blackboard = null;
    private IBehaviour m_RootBehaviour = null;

    public Blackboard GetBlackboard
    {
        get => m_Blackboard;
    }

    public BehaviourTree(Blackboard blackboard, IBehaviour rootBehaviour)
    {
        m_Blackboard = blackboard;
        m_RootBehaviour = rootBehaviour;
    }

    public void Update()
    {
        m_CurrentState = m_RootBehaviour.Execute(m_Blackboard);
    }
}