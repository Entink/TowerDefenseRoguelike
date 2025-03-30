using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    private float minX;
    private float maxX;
    private float camHalfWidth;

    public FightData testFight;


    private void Start()
    {
        Camera cam = Camera.main;

        camHalfWidth = cam.orthographicSize * cam.aspect;

        FightData fight = BattleDataCarrier.selectedFight != null ? BattleDataCarrier.selectedFight : testFight;

        if (fight == null)
        {
            Debug.LogWarning("Brak danych o walce! - kamera nie ustawi granic.");
            return;
        }

        float halfLength = fight.levelLenght / 2f;
        minX = -halfLength + camHalfWidth;
        maxX = halfLength - camHalfWidth;
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
            Vector3 newPos = transform.position + new Vector3(horizontal, 0f, 0f) * moveSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            transform.position = newPos;
        }
    }



}
