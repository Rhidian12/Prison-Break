using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBlackboardData
{};

public sealed class BlackboardData<T> : IBlackboardData
{
	private T m_Data;

	public T Data
    {
		get => m_Data;
        set
        {
			if (value.GetType().Equals(typeof(T)))
				m_Data = value;
        }
    }

	public BlackboardData(T data)
	{
		m_Data = data;
	}
};

public sealed class Blackboard
{
	private Dictionary<string, IBlackboardData> m_pData = new Dictionary<string, IBlackboardData>();

	public void AddData<T>(string id, T data)
	{
		if (!m_pData.ContainsKey(id))
			m_pData.Add(id, new BlackboardData<T>(data));
	}

	public void ChangeData<T>(string id, T data)
	{
		if (m_pData.TryGetValue(id, out IBlackboardData value))
			((BlackboardData<T>)value).Data = data;
	}

	public T GetData<T>(string id)
	{
		if (m_pData.TryGetValue(id, out IBlackboardData value))
			return ((BlackboardData<T>)value).Data;
		else
			throw new System.Exception();
	}
};
