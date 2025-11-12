using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerScanManager : MonoBehaviour
{
    [Header("Testing")]
    [Tooltip("If true, will auto-scan players for testing in the editor")]
    public bool autoScanForTesting = false;

    public TMP_Text player1Label;
    public TMP_Text player2Label;
    public Button startGameButton;

    public static bool Player1Scanned = false;
    public static bool Player2Scanned = false;

    private bool scanLocked = false; // prevents duplicate scans within one frame

    private void Start()
    {
        Player1Scanned = false;
        Player2Scanned = false;

        startGameButton.interactable = false;

        player1Label.text = "Player 1: Waiting for scan...";
        player2Label.text = "Player 2: Waiting for scan...";

        // Auto-scan players if testing
        if (autoScanForTesting)
        {
            Player1Scanned = true;
            Player2Scanned = true;
            player1Label.text = "Player 1: Ready";
            player2Label.text = "Player 2: Ready";
            startGameButton.interactable = true;
        }
    }

    public void OnBarcodeScanned(string scannedCode)
    {
        if (scanLocked) return;
        scanLocked = true;

        Debug.Log("Scanned: " + scannedCode);

        if (!Player1Scanned)
        {
            Player1Scanned = true;
            player1Label.text = "Player 1: Ready";
        }
        else if (!Player2Scanned)
        {
            Player2Scanned = true;
            player2Label.text = "Player 2: Ready";
        }

        CheckReady();

        // Unlock to allow next scan after short delay
        Invoke(nameof(UnlockScan), 0.4f);
    }

    void UnlockScan()
    {
        scanLocked = false;
    }

    void CheckReady()
    {
        if (Player1Scanned && Player2Scanned)
            startGameButton.interactable = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
