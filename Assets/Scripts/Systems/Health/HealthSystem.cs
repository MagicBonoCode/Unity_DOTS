using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<HealthComponent> health, Entity entity) in SystemAPI.Query<RefRW<HealthComponent>>().WithEntityAccess())
        {
            if(health.ValueRO.HealthAmount <= 0)
            {
                health.ValueRW.OnDead = true;
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
