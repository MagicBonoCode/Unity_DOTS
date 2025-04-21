using Unity.Entities;
using UnityEngine;

public struct MonsterSpawnerComponent : IComponentData
{
    public float Timer;
    public float SpawnIntervalMin;
    public float SpawnIntervalMax;
    public float CurrentInterval;

    public float SpawnXMin;
    public float SpawnXMax;
}

public struct MonsterPrefabComponent : IComponentData
{
    public Entity Prefab;
}

public class MonsterSpawnPointAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject monsterPrefab;
    [SerializeField]
    private Vector2 xRange = new Vector2(-10.0f, 10.0f);
    [SerializeField]
    private Vector2 intervalRange = new Vector2(0.5f, 1.0f);

    public class Baker : Baker<MonsterSpawnPointAuthoring>
    {
        public override void Bake(MonsterSpawnPointAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var monsterEntity = GetEntity(authoring.monsterPrefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new MonsterSpawnerComponent
            {
                Timer = 0.0f,
                SpawnIntervalMin = authoring.intervalRange.x,
                SpawnIntervalMax = authoring.intervalRange.y,
                CurrentInterval = UnityEngine.Random.Range(authoring.intervalRange.x, authoring.intervalRange.y),
                SpawnXMin = authoring.xRange.x,
                SpawnXMax = authoring.xRange.y
            });

            AddComponent(entity, new MonsterPrefabComponent
            {
                Prefab = monsterEntity
            });
        }
    }
}
