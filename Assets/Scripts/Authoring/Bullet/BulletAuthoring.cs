using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Bullet : IComponentData
{
    public float MoveSpeed;
    public float MaxDistance;
    public int Damage;

    public float3 SpawnPosition;
}

public class BulletAuthoring : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private int damage;

    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet
            {
                MoveSpeed = authoring.moveSpeed,
                MaxDistance = authoring.maxDistance,
                Damage = authoring.damage,
            });
        }
    }
}
