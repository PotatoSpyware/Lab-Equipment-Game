using UnityEngine;
using UnityEngine.UI;
using TMPro; // ✅ Required for TMPro

public class UIController : MonoBehaviour
{
    public UIAnimator uiAnimator;
    public Button[] moveButtons;
    public GameObject movesContainer;   
    
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

    public void ShowMoves(Move[] moves, System.Action<int> callback)
    {
        movesContainer.SetActive(true);

        for (int i = 0; i < moveButtons.Length; i++)
        {
            moveButtons[i].interactable = true;  // ✅ ensure re-enabled

            moveButtons[i].onClick.RemoveAllListeners();

            if (i < moves.Length)
            {
                moveButtons[i].gameObject.SetActive(true);
                TMP_Text label = moveButtons[i].GetComponentInChildren<TMP_Text>();
                label.text = moves[i].moveName;

                int index = i;
                moveButtons[i].onClick.AddListener(() =>
                {
                    moveButtons[index].interactable = false;  // ✅ prevent double click
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
        movesContainer.SetActive(false);  // hides all move buttons
    }

    public void ShowTurn(string message)
    {
        uiAnimator.PunchTurnBanner(message);
    }

    // UIController
    public void ShowWinner(Unit winner)
    {
        HideMoves(); // disable buttons
        uiAnimator.ShowWinner(winner); // pass the Unit
    }

}