using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class AllyModifierItem : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public int IncreaseCount { get; private set; }

    public void SetIncreaseCount(int count)
    {
        IncreaseCount = count;
        _text.text = $"+{count}";
    }

    private EntityManager _entityManager;
    private Entity _allyPrefab;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // AllyPrefabSingleton에서 프리팹 Entity 가져오기
        var query = _entityManager.CreateEntityQuery(typeof(AllyPrefabSingleton));
        if(query.CalculateEntityCount() == 0)
        {
            return;
        }

        _allyPrefab = _entityManager.GetComponentData<AllyPrefabSingleton>(query.GetSingletonEntity()).Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_entityManager == null || _allyPrefab == Entity.Null || GameManager.Instance.AllyCenter == null)
            return;

        if(IncreaseCount > 0)
        {
            for(int i = 0; i < IncreaseCount; i++)
            {
                Vector2 circle = UnityEngine.Random.insideUnitCircle * 4.0f;
                Vector3 spawnPos = GameManager.Instance.AllyCenter.transform.position + new Vector3(circle.x, 0.0f, circle.y);

                Entity newAlly = _entityManager.Instantiate(_allyPrefab);
                _entityManager.SetComponentData(newAlly, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = quaternion.identity,
                    Scale = 1.0f
                });
            }
        }

        Destroy(gameObject); // 자기 자신 제거
    }
}
