using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AllyTag : IComponentData { }

public struct InputComponent : IComponentData
{
    public float3 Position;
    public float Dir;
}

public struct AllyMoverComponent : IComponentData
{
    public float MoveSpeed;
}

public struct AllyFireComponent : IComponentData
{
    public float FireRate;
    public float Timer;
}

public class AllyAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private float fireRate = 1.0f;

    public class Baker : Baker<AllyAuthoring>
    {
        public override void Bake(AllyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponentObject(entity, new UnitGameObjectPrefabComponent
            {
                Prefab = authoring.prefab,
            });
            AddComponent<UnitStateComponent>(entity);

            AddComponent(entity, new AllyTag());
            AddComponent(entity, new AllyMoverComponent
            {
                MoveSpeed = authoring.moveSpeed,
            });
            AddComponent(entity, new AllyFireComponent
            {
                FireRate = authoring.fireRate,
            });
        }
    }
}
