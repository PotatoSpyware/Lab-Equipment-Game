using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    public Unit player1;
    public Unit player2;
    public UIController ui;

    private bool isPlayer1Turn = true;
    private bool isBusy = false;
    private int turnCount = 0; // counts full rounds

    public Randoimizer p1Randomizer;
    public Randoimizer p2Randomizer;

    void Start()
    {
        // Assign units using barcode/randomizer
        p1Randomizer.Split();
        p2Randomizer.Split();

        player1.currentHP = player1.maxHP;
        player2.currentHP = player2.maxHP;

        ui.UpdateUI(player1, player2);
        ui.ShowMoves(player1.moves, OnMoveSelected, turnCount);

        ui.uiAnimator.PunchTurnBanner($"{(isPlayer1Turn ? player1.unitName : player2.unitName)}'s Turn");

        ui.playAgainButton.onClick.AddListener(PlayAgain);
    }

    void OnMoveSelected(int moveIndex)
    {
        if (isBusy) return;
        isBusy = true;

        Unit attacker = isPlayer1Turn ? player1 : player2;
        Unit defender = isPlayer1Turn ? player2 : player1;

        Move move = attacker.moves[moveIndex];

        bool hasAdvantage = CheckElementAdvantage(attacker, defender);
        int finalDamage = move.damage + (hasAdvantage ? 20 : 0);

        // Mark move as used immediately for cooldowns
        move.lastUsedByPlayerTurn = turnCount;

        // Start attack animation with damage popup on contact
        StartCoroutine(AttackFlow(attacker, defender, move, finalDamage, hasAdvantage));
    }

    IEnumerator AttackFlow(Unit attacker, Unit defender, Move move, int damage, bool bonus)
    {
        yield return PlayAttackAnimation(attacker, defender, damage, bonus);

        // Apply damage
        defender.TakeDamage(damage);
        ui.UpdateUI(player1, player2);

        // Check win condition
        if (defender.currentHP <= 0)
        {
            ui.HideMoves();
            ui.ShowWinner(attacker);
            yield break;
        }

        // Switch turn
        isPlayer1Turn = !isPlayer1Turn;

        // Increment turnCount only after both players have moved (full round)
        if (isPlayer1Turn) turnCount++;

        ui.ShowMoves(isPlayer1Turn ? player1.moves : player2.moves, OnMoveSelected, turnCount);
        ui.uiAnimator.PunchTurnBanner($"{(isPlayer1Turn ? player1.unitName : player2.unitName)}'s Turn");

        isBusy = false;
    }

    private bool CheckElementAdvantage(Unit attacker, Unit defender)
    {
        var aType = attacker.modelSelector.selectedType;
        var dType = defender.modelSelector.selectedType;

        // Reversed advantage
        return (aType == UnitModelSelector.UnitType.Rock && dType == UnitModelSelector.UnitType.Paper) ||
               (aType == UnitModelSelector.UnitType.Scissors && dType == UnitModelSelector.UnitType.Rock) ||
               (aType == UnitModelSelector.UnitType.Paper && dType == UnitModelSelector.UnitType.Scissors);
    }

    IEnumerator PlayAttackAnimation(Unit attacker, Unit defender, int damage, bool bonus)
    {
        Transform a = attacker.model;
        Transform d = defender.model;

        Rigidbody attackerRb = attacker.GetComponent<Rigidbody>();
        Rigidbody defenderRb = defender.GetComponent<Rigidbody>();

        if (!attackerRb || !defenderRb) yield break;

        defenderRb.isKinematic = true;

        Vector3 originalWorldPos = attacker.transform.position;
        Quaternion originalRot = a.localRotation;

        attacker.hasCollided = false;

        float windUp = 0.2f;
        float t = 0f;

        // Wind-up animation
        while (t < windUp)
        {
            a.localRotation = Quaternion.Euler(Mathf.Lerp(0, 45, t / windUp), 0, 0);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // Smash animation
        Vector3 attackDir = (defender.transform.position - attacker.transform.position).normalized;
        attackerRb.AddForce(attackDir * attacker.attackForce, ForceMode.VelocityChange);

        float timeout = 0.6f;
        float elapsed = 0f;

        // Wait for collision or timeout
        while (!attacker.hasCollided && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // STOP MOVEMENT
        attackerRb.linearVelocity = Vector3.zero;
        attackerRb.angularVelocity = Vector3.zero;

        // SPAWN DAMAGE POPUP ON CONTACT
        ui.ShowDamagePopup(defender.transform.position, damage, bonus);

        // Defender shake
        Vector3 defenderStart = d.localPosition;
        float shakeDuration = 0.25f;
        elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            d.localPosition = defenderStart + Random.insideUnitSphere * 0.05f;
            elapsed += Time.deltaTime;
            yield return null;
        }
        d.localPosition = defenderStart;

        defenderRb.isKinematic = false;

        // Return attacker to original position
        float returnSpeed = 6f;
        while (Vector3.Distance(attacker.transform.position, originalWorldPos) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, originalWorldPos, Time.deltaTime * returnSpeed);
            yield return null;
        }

        attacker.transform.position = originalWorldPos;
        a.localRotation = originalRot;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
