using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach(var (transform, bullet, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Bullet>>().WithEntityAccess())
        {
            // 이동
            float newZ = transform.ValueRW.Position.z + bullet.ValueRO.MoveSpeed * deltaTime;
            transform.ValueRW.Position.z = newZ;

            // 거리 초과 제거
            float distanceTravelled = math.distance(bullet.ValueRO.SpawnPosition, transform.ValueRW.Position);
            if(distanceTravelled >= bullet.ValueRO.MaxDistance)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            // 몬스터 충돌 감지 (단순 거리 기반)
            foreach(var (monsterTransform, monsterEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<MonsterTag>().WithEntityAccess())
            {
                float dist = math.distance(transform.ValueRW.Position, monsterTransform.ValueRO.Position);
                if(dist < 1.5f) // 충돌 판정 거리 (반지름 기준)
                {
                    // 대미지 적용 (HealthComponent가 있다면)
                    if(state.EntityManager.HasComponent<HealthComponent>(monsterEntity))
                    {
                        var health = state.EntityManager.GetComponentData<HealthComponent>(monsterEntity);
                        health.HealthAmount -= bullet.ValueRO.Damage;
                        health.OnHealthChanged = true;
                        state.EntityManager.SetComponentData(monsterEntity, health);

                        UnityEngine.Debug.Log($"Monster hit! Remaining Health: {health.HealthAmount}");
                    }

                    entityCommandBuffer.DestroyEntity(entity); // 총알 제거
                    break;
                }
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }
}
