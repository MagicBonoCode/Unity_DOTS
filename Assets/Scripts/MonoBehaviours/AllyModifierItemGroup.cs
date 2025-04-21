using UnityEngine;

public class AllyModifierItemGroup : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float lifeTime = 60.0f;

    [SerializeField]
    private AllyModifierItem[] items;

    public void Start()
    {
        for(int i = 0; i < items.Length; i++)
        {
            items[i].SetIncreaseCount(Random.Range(1, 100));
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }
}
