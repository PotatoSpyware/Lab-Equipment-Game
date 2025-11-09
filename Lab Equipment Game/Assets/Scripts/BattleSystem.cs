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

    void Start()
    {
        player1.currentHP = player1.maxHP;
        player2.currentHP = player2.maxHP;

        ui.UpdateUI(player1, player2);
        ui.ShowMoves(player1.moves, OnMoveSelected);

        ui.uiAnimator.PunchTurnBanner(
            $"{(isPlayer1Turn ? player1.unitName : player2.unitName)}'s Turn"
        );
        ui.uiAnimator.playAgainButton.onClick.AddListener(PlayAgain);
    }

    void OnMoveSelected(int moveIndex)
    {
        if (isBusy) return;
        isBusy = true;
        
        Unit attacker = isPlayer1Turn ? player1 : player2;
        Unit defender = isPlayer1Turn ? player2 : player1;

        Move move = attacker.moves[moveIndex];

        // ✅ Run coroutine that handles animation → damage → UI update → turn swap
        StartCoroutine(AttackFlow(attacker, defender, move));
    }

    IEnumerator AttackFlow(Unit attacker, Unit defender, Move move)
    {
        yield return PlayAttackAnimation(attacker, defender);

        defender.TakeDamage(move.damage);

        ui.UpdateUI(player1, player2);

        // Check win condition
        // BattleSystem
        if (defender.currentHP <= 0)
        {
            ui.HideMoves();
            ui.ShowWinner(attacker); // pass the Unit, not a string
            yield break;
        }


        // Switch turn
        isPlayer1Turn = !isPlayer1Turn;

        ui.ShowMoves(isPlayer1Turn ? player1.moves : player2.moves, OnMoveSelected);

        ui.uiAnimator.PunchTurnBanner(
            $"{(isPlayer1Turn ? player1.unitName : player2.unitName)}'s Turn"
        );

        isBusy = false;
    }


    IEnumerator PlayAttackAnimation(Unit attacker, Unit defender)
    {
        Transform a = attacker.model;
        Transform d = defender.model;

        Rigidbody attackerRb = attacker.GetComponent<Rigidbody>();
        Rigidbody defenderRb = defender.GetComponent<Rigidbody>();

        if (!attackerRb || !defenderRb)
            yield break;

        // ✅ Freeze defender so it can't launch into space
        defenderRb.isKinematic = true;

        Vector3 originalWorldPos = attacker.transform.position;
        Quaternion originalRot = a.localRotation;

        attacker.hasCollided = false;

        // --- STEP 1: Wind-up animation (raise model) ---
        float windUp = 0.2f;
        float t = 0f;

        while (t < windUp)
        {
            a.localRotation = Quaternion.Euler(Mathf.Lerp(0, 45, t / windUp), 0, 0);  // tilt forward
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // --- STEP 2: Smash (attacker uses physics force) ---
        Vector3 attackDir = (defender.transform.position - attacker.transform.position).normalized;
        attackerRb.AddForce(attackDir * attacker.attackForce, ForceMode.VelocityChange);

        float timeout = 0.6f;
        float elapsed = 0f;

        // Wait until attacker collides OR timeout
        while (!attacker.hasCollided && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Stop attacker movement
        attackerRb.linearVelocity = Vector3.zero;
        attackerRb.angularVelocity = Vector3.zero;

        // --- STEP 3: Defender shake animation ---
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

        // ✅ Allow defender physics again
        defenderRb.isKinematic = false;

        // --- STEP 4: Return attacker to original position ---
        float returnSpeed = 6f;
        while (Vector3.Distance(attacker.transform.position, originalWorldPos) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(
                attacker.transform.position,
                originalWorldPos,
                Time.deltaTime * returnSpeed
            );
            yield return null;
        }

        attacker.transform.position = originalWorldPos;
        a.localRotation = originalRot;
    }
    
    public void PlayAgain()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
