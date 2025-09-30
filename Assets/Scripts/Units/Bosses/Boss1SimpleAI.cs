using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class Boss1SimpleAI : MonoBehaviour
{
    [SerializeField] private float retreatMoveSpeed = 1.5f;

    private UnitController ctrl;
    [SerializeField] private bool hasRetreatTarget = false;
    private float targetX;

    private void Awake()
    {
        ctrl = GetComponent<UnitController>();
    }

    public void RetreatTo(float baseX)
    {
        targetX = baseX;
        hasRetreatTarget = true;
    }

    private void FixedUpdate()
    {
        if (ctrl != null && ctrl.IsStunned) return;

        if(hasRetreatTarget)
        {
            float dx = targetX - transform.position.x;
            float step = Mathf.Sign(dx) * retreatMoveSpeed * Time.deltaTime;
            if (Mathf.Abs(step) > Mathf.Abs(dx)) step = dx;
            transform.position += new Vector3(step, 0f, 0f);

            if (Mathf.Abs(dx) <= 1f)
                hasRetreatTarget = false;
        }
    }

    
}