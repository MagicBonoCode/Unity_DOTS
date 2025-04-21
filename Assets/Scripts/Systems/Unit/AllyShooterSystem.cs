using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AllyShooterSystem : ISystem
{
    private Entity _bulletPrefab;

    public void OnCreate(ref SystemState state)
    {
        // Load bullet prefab (Runtime 설정 또는 Baker를 통해 저장 가능)
        state.RequireForUpdate<BulletPrefabSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var bulletPrefab = SystemAPI.GetSingleton<BulletPrefabSingleton>().Value;
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach(var (transform, cooldown, entity) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<AllyFireComponent>>().WithEntityAccess()
                          .WithAll<AllyTag>())
        {
            cooldown.ValueRW.Timer += deltaTime;

            if(cooldown.ValueRW.Timer >= cooldown.ValueRW.FireRate)
            {
                cooldown.ValueRW.Timer = 0f;

                float3 spawnPos = transform.ValueRO.Position + transform.ValueRO.Forward() * 1.0f;

                Entity bullet = entityCommandBuffer.Instantiate(bulletPrefab);
                entityCommandBuffer.SetComponent(bullet, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = transform.ValueRO.Rotation,
                    Scale = 1f
                });
                entityCommandBuffer.SetComponent(bullet, new Bullet
                {
                    MoveSpeed = 40,
                    MaxDistance = 20f,
                    SpawnPosition = spawnPos,
                    Damage = 1
                });
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }
}
