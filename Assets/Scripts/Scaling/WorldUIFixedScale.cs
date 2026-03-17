using UnityEngine;

public class WorldUIFixedScale : MonoBehaviour
{
    [SerializeField] private Transform targetToCompensate;
    [SerializeField] private bool updateEveryFrame = true;
    [SerializeField] private bool autoFindTarget = true;

    private void Awake()
    {
        ApplyScaleCompensation();
        TryAutoAssignTarget();
    }

    private void LateUpdate()
    {
        if (!updateEveryFrame)
            return;

        if (targetToCompensate == null && autoFindTarget)
            TryAutoAssignTarget();

        ApplyScaleCompensation();
    }

    public void ApplyScaleCompensation()
    {
        if (targetToCompensate == null)
            return;

        Vector3 lossy = targetToCompensate.lossyScale;

        float x = Mathf.Approximately(lossy.x, 0f) ? 1f : 1f / lossy.x;
        float y = Mathf.Approximately(lossy.y, 0f) ? 1f : 1f / lossy.y;
        float z = Mathf.Approximately(lossy.z, 0f) ? 1f : 1f / lossy.z;

        transform.localScale = new Vector3(x, y, z);

    }

    public void TryAutoAssignTarget()
    {
        if (targetToCompensate != null)
            return;

        UnitController unitController = GetComponentInParent<UnitController>();

        if(unitController != null)
        {
            targetToCompensate = unitController.transform;
            return;
        }

        if (transform.root != null && transform.root != transform)
            targetToCompensate = transform.root;

    }
}
