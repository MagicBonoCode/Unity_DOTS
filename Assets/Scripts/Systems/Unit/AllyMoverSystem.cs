using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AllyMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if(!SystemAPI.HasSingleton<InputComponent>())
        { 
            return;
        }

        float dir = SystemAPI.GetSingleton<InputComponent>().Dir;
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach(var (transform, mover) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<AllyMoverComponent>>())
        {
            float newX = transform.ValueRW.Position.x + dir * mover.ValueRO.MoveSpeed * deltaTime;
            if(newX > 10.0f)
            {
                newX = 10.0f;
            }
            else if(newX < -10.0f)
            {
                newX = -10.0f;
            }

            transform.ValueRW.Position.x = newX;
        }
    }
}
