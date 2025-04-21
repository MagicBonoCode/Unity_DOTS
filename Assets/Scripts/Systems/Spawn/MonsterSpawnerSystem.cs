using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MonsterSpawnerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach(var (spawner, prefabRef, transform, entity) in SystemAPI.Query<RefRW<MonsterSpawnerComponent>, RefRO<MonsterPrefabComponent>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            spawner.ValueRW.Timer += deltaTime;

            if(spawner.ValueRW.Timer >= spawner.ValueRW.CurrentInterval)
            {
                spawner.ValueRW.Timer = 0.0f;

                float next = UnityEngine.Random.Range(spawner.ValueRO.SpawnIntervalMin, spawner.ValueRO.SpawnIntervalMax);
                spawner.ValueRW.CurrentInterval = next;

                float spawnX = UnityEngine.Random.Range(spawner.ValueRO.SpawnXMin, spawner.ValueRO.SpawnXMax);
                float3 spawnPos = new float3(spawnX, transform.ValueRO.Position.y, transform.ValueRO.Position.z);

                Entity monster = entityCommandBuffer.Instantiate(prefabRef.ValueRO.Prefab);
                entityCommandBuffer.SetComponent(monster, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = quaternion.identity,
                    Scale = 1.0f
                });
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }
}
