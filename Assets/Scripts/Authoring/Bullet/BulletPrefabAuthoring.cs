using Unity.Entities;
using UnityEngine;

public struct BulletPrefabSingleton : IComponentData
{
    public Entity Value;
}

public class BulletPrefabAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;

    public class Baker : Baker<BulletPrefabAuthoring>
    {
        public override void Bake(BulletPrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var bulletEntity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new BulletPrefabSingleton
            {
                Value = bulletEntity
            });
        }
    }
}
