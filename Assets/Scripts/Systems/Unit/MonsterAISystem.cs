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

        // 아군 위치 캐싱
        var allyData = new NativeList<(Entity entity, float3 position)>(Allocator.Temp);
        foreach(var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<AllyTag>().WithEntityAccess())
        {
            allyData.Add((entity, transform.ValueRO.Position));
        }

        // 몬스터 이동 및 타겟 탐색
        foreach(var (transform, ai, unitState) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MonsterAIComponent>, RefRW<UnitStateComponent>>().WithAll<MonsterTag>())
        {
            float3 myPos = transform.ValueRO.Position;
            float3 targetDir = new float3(0, 0, -1); // 기본적으로 아래(z-) 방향
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
                        shouldMove = false; // 멈춤
                        targetDir = float3.zero;
                        myState = UnitState.Attack;
                        targetEntity = entity; // 타겟 엔티티 설정
                        break;
                    }
                    else
                    {
                        targetDir = math.normalize(allyPos - myPos); // 추적 방향 전환
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

            // 타겟 엔티티의 Health 감소
            if(myState == UnitState.Attack && targetEntity != Entity.Null)
            {
                if(state.EntityManager.HasComponent<HealthComponent>(targetEntity))
                {
                    var health = state.EntityManager.GetComponentData<HealthComponent>(targetEntity);

                    // 현재 시간 가져오기
                    double currentTime = SystemAPI.Time.ElapsedTime;

                    // 1초 간격으로 체력 감소
                    if(currentTime - ai.ValueRO.LastAttackTime >= 1.0f)
                    {
                        health.HealthAmount -= (int)ai.ValueRO.AttackDamage; // 정수형 데미지 감소
                        state.EntityManager.SetComponentData(targetEntity, health);

                        // 마지막 공격 시간 업데이트
                        ai.ValueRW.LastAttackTime = currentTime;
                    }
                }
            }
        }

        allyData.Dispose();
    }
}
