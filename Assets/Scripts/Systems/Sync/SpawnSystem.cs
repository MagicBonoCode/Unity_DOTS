using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
public partial struct SpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var query = SystemAPI.QueryBuilder().WithAll<UnitGameObjectPrefabComponent>().Build();
        var entities = query.ToEntityArray(Allocator.Temp);

        foreach(var entity in entities)
        {
            var unitGameObjectPrefab = state.EntityManager.GetComponentData<UnitGameObjectPrefabComponent>(entity);
            var instance = GameObject.Instantiate(unitGameObjectPrefab.Prefab);
            instance.hideFlags |= HideFlags.HideAndDontSave;

            // Entity ¿¬°á
            var controller = instance.GetComponent<UnitAnimatorController>();
            if(controller != null)
            { 
                controller.Entity = entity;
            }

            state.EntityManager.AddComponentObject(entity, instance.GetComponent<Transform>());
            state.EntityManager.AddComponentObject(entity, instance.GetComponent<Animator>());
            state.EntityManager.AddComponentData(entity, new UnitGameObjectInstance { Instance = instance });
            state.EntityManager.RemoveComponent<UnitGameObjectPrefabComponent>(entity);
        }
    }
}
