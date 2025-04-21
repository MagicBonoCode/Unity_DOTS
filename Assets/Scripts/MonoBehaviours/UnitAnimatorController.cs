using Unity.Entities;
using UnityEngine;

public class UnitAnimatorController : MonoBehaviour
{
    public Entity Entity; // 외부에서 연결 (SpawnSystem 등에서 할당)
    private EntityManager entityManager;

    private Animator animator;
    private UnitStateComponent lastState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        if(!entityManager.Exists(Entity)) return;
        if(!entityManager.HasComponent<UnitStateComponent>(Entity)) return;

        var state = entityManager.GetComponentData<UnitStateComponent>(Entity);
        if(state.State == lastState.State) return; // 상태 바뀔 때만 작동

        switch(state.State)
        {
            case UnitState.Idle:
                animator.CrossFade("Idle", 0.1f);
                break;
            case UnitState.Move:
                animator.CrossFade("Move", 0.1f);
                break;
            case UnitState.Attack:
                animator.CrossFade("Attack", 0.1f);
                break;
        }

        lastState = state;
    }
}
