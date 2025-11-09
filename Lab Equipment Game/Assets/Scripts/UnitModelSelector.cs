using UnityEngine;

public class UnitModelSelector : MonoBehaviour
{
    public enum UnitType
    {
        Rock,
        Paper,
        Scissors
    }

    [Header("Pick which model this unit should use")]
    public UnitType selectedType;

    [Header("Assign model objects")]
    public GameObject rockModel;
    public GameObject paperModel;
    public GameObject scissorsModel;

    void OnValidate()
    {
        ApplyModel();
    }

    void Start()
    {
        ApplyModel();
    }

    void ApplyModel()
    {
        if (rockModel) rockModel.SetActive(selectedType == UnitType.Rock);
        if (paperModel) paperModel.SetActive(selectedType == UnitType.Paper);
        if (scissorsModel) scissorsModel.SetActive(selectedType == UnitType.Scissors);
    }
}