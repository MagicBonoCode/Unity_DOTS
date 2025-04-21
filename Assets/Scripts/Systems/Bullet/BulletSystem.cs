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
            // �̵�
            float newZ = transform.ValueRW.Position.z + bullet.ValueRO.MoveSpeed * deltaTime;
            transform.ValueRW.Position.z = newZ;

            // �Ÿ� �ʰ� ����
            float distanceTravelled = math.distance(bullet.ValueRO.SpawnPosition, transform.ValueRW.Position);
            if(distanceTravelled >= bullet.ValueRO.MaxDistance)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            // ���� �浹 ���� (�ܼ� �Ÿ� ���)
            foreach(var (monsterTransform, monsterEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<MonsterTag>().WithEntityAccess())
            {
                float dist = math.distance(transform.ValueRW.Position, monsterTransform.ValueRO.Position);
                if(dist < 1.5f) // �浹 ���� �Ÿ� (������ ����)
                {
                    // ����� ���� (HealthComponent�� �ִٸ�)
                    if(state.EntityManager.HasComponent<HealthComponent>(monsterEntity))
                    {
                        var health = state.EntityManager.GetComponentData<HealthComponent>(monsterEntity);
                        health.HealthAmount -= bullet.ValueRO.Damage;
                        health.OnHealthChanged = true;
                        state.EntityManager.SetComponentData(monsterEntity, health);

                        UnityEngine.Debug.Log($"Monster hit! Remaining Health: {health.HealthAmount}");
                    }

                    entityCommandBuffer.DestroyEntity(entity); // �Ѿ� ����
                    break;
                }
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }
}
