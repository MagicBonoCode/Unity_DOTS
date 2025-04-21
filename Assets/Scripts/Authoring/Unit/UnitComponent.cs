using System;
using Unity.Entities;
using UnityEngine;

public class UnitGameObjectPrefabComponent : IComponentData
{
    public GameObject Prefab;
}

public class UnitGameObjectInstance : IComponentData, IDisposable
{
    public GameObject Instance;

    public void Dispose()
    {
        UnityEngine.Object.DestroyImmediate(Instance);
    }
}

public struct UnitStateComponent : IComponentData
{
    public UnitState State;
}

public enum UnitState
{
    Idle,
    Move,
    Attack,
}
