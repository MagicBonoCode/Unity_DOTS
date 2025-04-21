using Unity.Entities;
using UnityEngine;

public struct AllyPrefabSingleton : IComponentData
{
    public Entity Value;
}

public class AllyPrefabAuthoring : MonoBehaviour
{
    public GameObject allyPrefab;

    public class Baker : Baker<AllyPrefabAuthoring>
    {
        public override void Bake(AllyPrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var prefabEntity = GetEntity(authoring.allyPrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new AllyPrefabSingleton
            {
                Value = prefabEntity
            });
        }
    }
}
