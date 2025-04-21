using Unity.Entities;
using UnityEngine;

public struct HealthComponent : IComponentData
{
    public int HealthAmount;
    public int MaxHealthAmount;
    public bool OnHealthChanged;
    public bool OnDead;
    public bool OnTookDamage;
}

public class HealthAuthoring : MonoBehaviour
{
    [SerializeField]
    private int maxHealthAmount = 10;

    public class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthComponent
            {
                HealthAmount = authoring.maxHealthAmount,
                MaxHealthAmount = authoring.maxHealthAmount,
                OnHealthChanged = true,
            });
        }

    }
}
