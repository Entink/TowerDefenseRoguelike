using UnityEngine;

public class CameraControll : MonoBehaviour
{
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

        var pan = Camera.main.GetComponent<CameraDragPan>();

        pan.SetClamp(minX, maxX);
    }

    



}
