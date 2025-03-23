using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [Header("Movement")]
    public float cameraSpeed;

    [Header("Offsets")]
    public float minX;
    public float maxX;

    [SerializeField] private Transform enemyBase ;
    [SerializeField] private Transform allyBase;
    [SerializeField] private float margin = 3f;

    private void Awake()
    {
        enemyBase = GameObject.Find("EnemyBase").transform;
        allyBase = GameObject.Find("AllyBase").transform;

        if (enemyBase != null)
        {
            minX = enemyBase.position.x - margin;

        }

        if(allyBase != null)
        {
            maxX = allyBase.position.x + margin;

        }
    }

    private void FixedUpdate()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
        }

        if (horizontal != 0f)
        {
            Vector3 newPos = transform.position + new Vector3(horizontal, 0f, 0f) * cameraSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            transform.position = newPos;
        }
    }



}
