using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIAnimator : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI turnBanner;
    public CanvasGroup winnerCanvasGroup;  // contains darkener + text + buttons
    public Button[] moveButtons;
    public Button playAgainButton;
    public TextMeshProUGUI winnerText;
    void Start()
    {
        HideWinPanel();
    }

    public void PunchTurnBanner(string text)
    {
        turnBanner.gameObject.SetActive(true);
        turnBanner.text = text;
        StartCoroutine(ScalePunch(turnBanner.transform));
    }

    IEnumerator ScalePunch(Transform target)
    {
        Vector3 startScale = Vector3.one;
        Vector3 punchScale = new Vector3(1.3f, 1.3f, 1f);

        float time = 0f;
        float duration = 0.2f;

        while (time < duration)
        {
            target.localScale = Vector3.Lerp(startScale, punchScale, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        target.localScale = punchScale;

        time = 0f;
        while (time < duration)
        {
            target.localScale = Vector3.Lerp(punchScale, startScale, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        target.localScale = startScale;
    }

    // UIAnimator
    public void ShowWinner(Unit winner)
    {
        // disable move buttons
        foreach (var btn in moveButtons)
            btn.interactable = false;

        // Update text immediately
        winnerText.text = $"{winner.unitName} WINS!";

        // Activate winner UI
        winnerCanvasGroup.gameObject.SetActive(true);
        winnerCanvasGroup.alpha = 0f;

        // Fade in
        StartCoroutine(FadeCanvasGroup(winnerCanvasGroup, 0f, 1f, 0.5f));
    }


    public void HideWinPanel()
    {
        winnerCanvasGroup.alpha = 0f;
        winnerCanvasGroup.gameObject.SetActive(false);

        // Re-enable move buttons
        foreach (var btn in moveButtons)
            btn.interactable = true;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        cg.alpha = to;
    }
}
