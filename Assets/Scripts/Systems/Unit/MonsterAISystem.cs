using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MonsterAISystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        // �Ʊ� ��ġ ĳ��
        var allyData = new NativeList<(Entity entity, float3 position)>(Allocator.Temp);
        foreach(var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<AllyTag>().WithEntityAccess())
        {
            allyData.Add((entity, transform.ValueRO.Position));
        }

        // ���� �̵� �� Ÿ�� Ž��
        foreach(var (transform, ai, unitState) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MonsterAIComponent>, RefRW<UnitStateComponent>>().WithAll<MonsterTag>())
        {
            float3 myPos = transform.ValueRO.Position;
            float3 targetDir = new float3(0, 0, -1); // �⺻������ �Ʒ�(z-) ����
            float closestDistance = float.MaxValue;
            bool shouldMove = true;
            UnitState myState = UnitState.Move;
            Entity targetEntity = Entity.Null;

            foreach(var (entity, allyPos) in allyData)
            {
                float distance = math.distance(myPos, allyPos);

                if(distance < ai.ValueRO.DetectRange && distance < closestDistance)
                {
                    closestDistance = distance;

                    if(distance <= ai.ValueRO.StopDistance)
                    {
                        shouldMove = false; // ����
                        targetDir = float3.zero;
                        myState = UnitState.Attack;
                        targetEntity = entity; // Ÿ�� ��ƼƼ ����
                        break;
                    }
                    else
                    {
                        targetDir = math.normalize(allyPos - myPos); // ���� ���� ��ȯ
                        shouldMove = true;
                    }
                }
            }

            if(shouldMove && !targetDir.Equals(float3.zero))
            {
                transform.ValueRW.Position += targetDir * ai.ValueRO.MoveSpeed * deltaTime;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(targetDir, math.up());
            }

            unitState.ValueRW.State = myState;

            // Ÿ�� ��ƼƼ�� Health ����
            if(myState == UnitState.Attack && targetEntity != Entity.Null)
            {
                if(state.EntityManager.HasComponent<HealthComponent>(targetEntity))
                {
                    var health = state.EntityManager.GetComponentData<HealthComponent>(targetEntity);

                    // ���� �ð� ��������
                    double currentTime = SystemAPI.Time.ElapsedTime;

                    // 1�� �������� ü�� ����
                    if(currentTime - ai.ValueRO.LastAttackTime >= 1.0f)
                    {
                        health.HealthAmount -= (int)ai.ValueRO.AttackDamage; // ������ ������ ����
                        state.EntityManager.SetComponentData(targetEntity, health);

                        // ������ ���� �ð� ������Ʈ
                        ai.ValueRW.LastAttackTime = currentTime;
                    }
                }
            }
        }

        allyData.Dispose();
    }
}
