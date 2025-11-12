using UnityEngine;

[System.Serializable]
public class Move
{
    public string moveName;
    public int damage;

    [HideInInspector] public int lastUsedByPlayerTurn = -2; 
    public int cooldownTurns = 1; 
}