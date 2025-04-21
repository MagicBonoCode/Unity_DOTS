using UnityEngine;

public class AllyCenter : MonoBehaviour
{
    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * 10.0f * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * 10.0f * Time.deltaTime;
        }
    }
}
