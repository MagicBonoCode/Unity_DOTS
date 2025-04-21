using Unity.Entities;
using UnityEngine;

public struct MonsterTag : IComponentData { }

public struct MonsterAIComponent : IComponentData
{
    public float MoveSpeed;
    public float DetectRange;
    public float StopDistance;
    public int AttackDamage;
    public double LastAttackTime;
}

public class MonsterAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private float moveSpeed = 1.5f;
    [SerializeField]
    private float detectRange = 10.0f;
    [SerializeField]
    private float stopDistance = 1.5f;
    [SerializeField]
    private int attackDamage = 1;

    public class Baker : Baker<MonsterAuthoring>
    {
        public override void Bake(MonsterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponentObject(entity, new UnitGameObjectPrefabComponent
            {
                Prefab = authoring.prefab
            });
            AddComponent<UnitStateComponent>(entity);

            AddComponent(entity, new MonsterTag());
            AddComponent(entity, new MonsterAIComponent
            {
                MoveSpeed = authoring.moveSpeed,
                DetectRange = authoring.detectRange,
                StopDistance = authoring.stopDistance,
                AttackDamage = authoring.attackDamage
            });
        }
    }
}
