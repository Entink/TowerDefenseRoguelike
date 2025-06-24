using UnityEngine;

public class MapCameraMover : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float leftLimit = -10f;
    public float rightLimit = 10f;

   

    private void Update()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move = -1f;
        if (Input.GetKey(KeyCode.D)) move = 1f;

        Vector3 newPos = transform.position + Vector3.right * move * moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, leftLimit, rightLimit);

        transform.position = newPos;
    }
}
