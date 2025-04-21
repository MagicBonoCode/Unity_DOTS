using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if(_instance == null)
                {
                    GameObject gameObject = new GameObject("GameManager", typeof(GameManager));
                    _instance = gameObject.GetComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private GameObject allyPrefab;
    public GameObject AllyCenter { get; private set; }

    private void Start()
    {
        if(AllyCenter == null)
        {
            AllyCenter = GameObject.Instantiate(allyPrefab);
            AllyCenter.transform.position = new Vector3(0f, 0f, 50.0f);
        }
    }

    public float AllyDir { get; private set; }

    [SerializeField]
    private GameObject allyModifierItemGroupPrefab;
    [SerializeField]
    private float spawnInterval = 5.0f;

    private float _timer;

    private void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            AllyDir = -1.0f;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            AllyDir = 1.0f;
        }
        else
        {
            AllyDir = 0.0f;
        }

        _timer = Mathf.Clamp(_timer + Time.deltaTime, 0.0f, spawnInterval);

        if(_timer >= spawnInterval)
        {
            GameObject itemGroup = Instantiate(allyModifierItemGroupPrefab);
            itemGroup.transform.position = new Vector3(0f, 0f, 120.0f);
            _timer = 0.0f;
        }
    }
}
