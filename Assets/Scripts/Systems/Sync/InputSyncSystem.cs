using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class InputSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dir = GameManager.Instance.AllyDir;

        if(!SystemAPI.HasSingleton<InputComponent>())
        {
            var entity = EntityManager.CreateEntity(typeof(InputComponent));
            EntityManager.SetComponentData(entity, new InputComponent { Dir = dir });
        }
        else
        {
            SystemAPI.SetSingleton(new InputComponent { Dir = dir });
        }
    }
}
