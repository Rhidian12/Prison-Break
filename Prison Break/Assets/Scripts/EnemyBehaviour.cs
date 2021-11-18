using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviour
{
    public abstract void Initialize();

    public virtual void FixedUpdate() {}
    public virtual void Update() {}
}