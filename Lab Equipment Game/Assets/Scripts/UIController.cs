using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIController : MonoBehaviour
{
    public UIAnimator uiAnimator;
    public Button[] moveButtons;
    public GameObject movesContainer;

    public GameObject damagePopupPrefab;
    
    [Header("UI Text (TMP)")]
    public TMP_Text player1HP;
    public TMP_Text player2HP;
    public TMP_Text turnText;
    public GameObject winnerPanel;
    public TextMeshProUGUI winnerText;
    public Button playAgainButton;
    public Button quitButton;

    public void UpdateUI(Unit p1, Unit p2)
    {
        player1HP.text = $"{p1.unitName}: {p1.currentHP} HP";
        player2HP.text = $"{p2.unitName}: {p2.currentHP} HP";
    }

    public void ShowMoves(Move[] moves, System.Action<int> callback, int playerTurn)
    {
        movesContainer.SetActive(true);

        for (int i = 0; i < moveButtons.Length; i++)
        {
            moveButtons[i].onClick.RemoveAllListeners();

            if (i < moves.Length)
            {
                moveButtons[i].gameObject.SetActive(true);
                TMP_Text label = moveButtons[i].GetComponentInChildren<TMP_Text>();
                label.text = $"{moves[i].moveName} ({moves[i].damage})";

                int index = i;

                // Disable button if still on cooldown
                moveButtons[i].interactable = (playerTurn - moves[index].lastUsedByPlayerTurn) > moves[index].cooldownTurns;

                moveButtons[i].onClick.AddListener(() =>
                {
                    moves[index].lastUsedByPlayerTurn = playerTurn; // mark move as used
                    moveButtons[index].interactable = false;
                    callback(index);
                });
            }
            else
            {
                moveButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideMoves()
    {
        movesContainer.SetActive(false);
    }

    public void ShowTurn(string message)
    {
        uiAnimator.PunchTurnBanner(message);
    }
    
    public void ShowDamagePopup(Vector3 worldPosition, int damage, bool bonus)
    {
        GameObject popup = Instantiate(damagePopupPrefab, transform); // parent to canvas
        TMP_Text text = popup.GetComponent<TMP_Text>();

        text.text = damage.ToString();  // use actual final damage
        text.color = bonus ? Color.yellow : Color.white;

        // Convert world to screen point
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        popup.transform.position = screenPos;

        StartCoroutine(FadeAndMoveUp(popup));
    }

    private IEnumerator FadeAndMoveUp(GameObject popup)
    {
        TMP_Text text = popup.GetComponent<TMP_Text>();
        float duration = 0.8f;
        float elapsed = 0f;
        Vector3 startPos = popup.transform.position;

        while (elapsed < duration)
        {
            popup.transform.position = startPos + Vector3.up * (elapsed * 50f); // move up
            text.alpha = Mathf.Lerp(1, 0, elapsed / duration); // fade out
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(popup);
    }

    public void ShowWinner(Unit winner)
    {
        HideMoves();
        uiAnimator.ShowWinner(winner);
    }
}
