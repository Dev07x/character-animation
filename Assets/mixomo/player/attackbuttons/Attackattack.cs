using UnityEngine;
using UnityEngine.UI;

public class Attackattack : MonoBehaviour
{
    public Animator playerAnimator;
    public Button attackattack;

    private void Start()
    {
        //playerAnimator = GetComponent<Animator>();
        if (attackattack != null)
        {
            attackattack.onClick.AddListener(aattackattack);
        }
    }

    private void aattackattack()
    {
        Debug.Log("aatackaatakc");
        playerAnimator.SetTrigger("isWalking");

        if (playerAnimator != null)
        {
            Debug.Log("ho");
            playerAnimator.SetFloat("velocityY", 0f);
            playerAnimator.SetFloat("velocityX", 0.5f);
        }
    }
}
