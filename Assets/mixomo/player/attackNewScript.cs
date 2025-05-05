using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class attackNewScript : MonoBehaviour
{
    [Header("Animator Reference")]
    [SerializeField] private Animator playerAnimator;

    [Header("Button References")]
    public Button attackButton;
    public Button upAttackButton;
    public Button gildeButton;
    public Button frAttackButton;
    public Button kick;
    public Button frattack;
    public Button DubbleAttack;

    private bool isAnimationPlaying = false;
    private playerStutus playerStatus;

    private void Awake()
    {
        // Try to get components if not assigned
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("Animator component not found!", this);
            }
        }

        playerStatus = GetComponent<playerStutus>();
        if (playerStatus == null)
        {
            //Debug.LogError("playerStutus component not found!", this);
        }
    }

    private void Start()
    {
        // Safe button assignments
        attackButton?.onClick.AddListener(Attack);
        upAttackButton?.onClick.AddListener(UpAttack);
        gildeButton?.onClick.AddListener(Gilde);
        frAttackButton?.onClick.AddListener(FrAttack);
        kick?.onClick.AddListener(Kick);
        DubbleAttack?.onClick.AddListener(dubbleAttack1);
    }

    private IEnumerator ResetAnimationFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAnimationPlaying = false;
    }

    private void PlayAnimation(string debugLog, string animationTrigger, float velocityY, float velocityX)
    {
        // Early exit checks
        if (isAnimationPlaying) return;
        if (playerStatus != null && playerStatus.isDead) return;
        if (playerAnimator == null)
        {
            Debug.LogWarning("Animator not available for: " + debugLog);
            return;
        }

        isAnimationPlaying = true;

        try
        {
            playerAnimator.SetTrigger(animationTrigger);
            playerAnimator.SetFloat("velocityY", velocityY);
            playerAnimator.SetFloat("velocityX", velocityX);
            StartCoroutine(ResetAnimationFlag(1f));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Animation error: {e.Message}");
            isAnimationPlaying = false;
        }
    }

    // Animation methods remain the same...
    private void Attack() => PlayAnimation("Attack", "isWalking", 1.5f, 0f);
    private void UpAttack() => PlayAnimation("UpAttack", "isWalking", 0f, -0.5f);
    private void Gilde() => PlayAnimation("Gilde", "isWalking", 0f, 0f);
    private void FrAttack() => PlayAnimation("FrAttack", "isWalking", 0.5f, 0f);
    private void Kick() => PlayAnimation("Kick", "isWalking", 0f, 0.5f);
    private void dubbleAttack1() => PlayAnimation("DubbleAttack", "isWalking", 1f, 0f);
}