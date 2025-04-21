using Unity.Entities;
using Unity.Transforms;

public partial struct SyncTransformSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (transform, gameObjectInstance) in SystemAPI.Query<RefRO<LocalTransform>, UnitGameObjectInstance>())
        {
            if(gameObjectInstance.Instance != null)
            {
                gameObjectInstance.Instance.transform.position = transform.ValueRO.Position;
                gameObjectInstance.Instance.transform.rotation = transform.ValueRO.Rotation;
            }
        }
    }
}
