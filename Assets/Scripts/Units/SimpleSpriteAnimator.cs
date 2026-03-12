using UnityEngine;

public class SimpleSpriteAnimator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Move")]
    [SerializeField] Sprite[] moveFrames;
    [SerializeField] float moveFps = 8f;

    [Header("Attack")]
    [SerializeField] Sprite[] attackFrames;
    [SerializeField] float attackFps = 12f;

    [Header("Stun")]
    [SerializeField] Sprite stunnedSprite;

    float moveSpeedMul = 1f;
    float attackSpeedMul = 1f;

    bool moving;
    bool stunned;
    bool attackPlaying;
    int moveIdx, attackIdx;
    float moveTimer, attackTimer;

    void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (moveFrames != null && moveFrames.Length > 0 && spriteRenderer && !stunned)
            spriteRenderer.sprite = moveFrames[0];
    }

    public void SetMoving(bool v)
    {
        moving = v && !stunned && !attackPlaying;
    }

    public void SetStunned(bool v)
    {
        stunned = v;
        if (stunned)
        {
            attackPlaying = false;
            if (spriteRenderer && stunnedSprite) spriteRenderer.sprite = stunnedSprite;
        }
    }

    public void PlayAttackOnce()
    {
        if (stunned) return;
        if (attackFrames == null || attackFrames.Length == 0) return;
        attackPlaying = true;
        attackIdx = 0;
        attackTimer = 0f;
        if (spriteRenderer) spriteRenderer.sprite = attackFrames[0];
    }

    public void SetAttackDuration(float seconds)
    {
        if (seconds <= 0f) return;
        float frames = (attackFrames != null) ? Mathf.Max(1, attackFrames.Length) : 1f;
        float mul = (frames / (attackFps * seconds));
        attackSpeedMul = Mathf.Clamp(mul, 0.25f, 4f);
    }

    public void SetMoveSpeedMul(float mul)
    {
        moveSpeedMul = Mathf.Clamp(mul, 0.25f, 3f);
    }

    void Update()
    {
        if (!spriteRenderer) return;

        if (stunned)
        {
            if (stunnedSprite) spriteRenderer.sprite = stunnedSprite;
            return;
        }

        if (attackPlaying)
        {
            if (attackFrames == null || attackFrames.Length == 0) { attackPlaying = false; return; }
            attackTimer += Time.deltaTime;
            float frameDur = 1f / Mathf.Max(1f, attackFps * attackSpeedMul);
            while (attackTimer >= frameDur)
            {
                attackTimer -= frameDur;
                attackIdx++;
                if (attackIdx >= attackFrames.Length)
                {
                    attackPlaying = false;
                    break;
                }
                spriteRenderer.sprite = attackFrames[attackIdx];
            }
            if (!attackPlaying && moveFrames != null && moveFrames.Length > 0 && moving)
                spriteRenderer.sprite = moveFrames[moveIdx];
            return;
        }

        if (moving && moveFrames != null && moveFrames.Length > 0)
        {
            moveTimer += Time.deltaTime;
            float frameDur = 1f / Mathf.Max(1f, moveFps * moveSpeedMul);
            while (moveTimer >= frameDur)
            {
                moveTimer -= frameDur;
                moveIdx = (moveIdx + 1) % moveFrames.Length;
                spriteRenderer.sprite = moveFrames[moveIdx];
            }
        }
        else
        {
            if (moveFrames != null && moveFrames.Length > 0)
                spriteRenderer.sprite = moveFrames[0];
        }
    }
}
