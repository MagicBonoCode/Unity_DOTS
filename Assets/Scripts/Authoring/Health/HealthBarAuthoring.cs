using Unity.Entities;
using UnityEngine;

public struct HealthBar : IComponentData
{
    public Entity BarVisualEntity;
    public Entity HealthEntity;

}

public class HealthBarAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject barVisualGameObject;
    [SerializeField]
    private GameObject healthGameObject;

    public class Baker : Baker<HealthBarAuthoring>
    {
        public override void Bake(HealthBarAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthBar
            {
                BarVisualEntity = GetEntity(authoring.barVisualGameObject, TransformUsageFlags.NonUniformScale),
                HealthEntity = GetEntity(authoring.healthGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }
}
