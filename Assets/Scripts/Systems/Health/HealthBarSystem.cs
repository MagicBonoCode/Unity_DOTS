using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct HealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;
        if(Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }

        foreach((RefRW<LocalTransform> localTransform, RefRO<HealthBar> healthBar) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<HealthBar>>())
        {
            // HealthEntity 유효성 검사
            if(healthBar.ValueRO.HealthEntity == Entity.Null || !state.EntityManager.Exists(healthBar.ValueRO.HealthEntity))
            {
                continue;
            }

            // BarVisualEntity 유효성 검사
            if(healthBar.ValueRO.BarVisualEntity == Entity.Null || !state.EntityManager.Exists(healthBar.ValueRO.BarVisualEntity))
            {
                continue;
            }

            // HealthComponent 존재 여부 확인
            if(!SystemAPI.HasComponent<HealthComponent>(healthBar.ValueRO.HealthEntity))
            {
                continue;
            }

            HealthComponent health = SystemAPI.GetComponent<HealthComponent>(healthBar.ValueRO.HealthEntity);

            if(!health.OnHealthChanged)
            {
                continue;
            }

            // HealthBar 로직 처리
            float healthNormalized = (float)health.HealthAmount / health.MaxHealthAmount;

            if(healthNormalized == 1.0f)
            {
                localTransform.ValueRW.Scale = 0.0f;
            }
            else
            {
                localTransform.ValueRW.Scale = 1.0f;
            }

            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.BarVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1.0f, 1.0f);
        }
    }
}
