using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Randoimizer : MonoBehaviour
{
    [SerializeField] UnitModelSelector selector;

    string code = "156329874562";

    int model;

    int move1;
    int move2;
    int move3;
    int move4;

    int bonusHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Split();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string Scan()
    {
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            code += Input.inputString;
        }
        Split();
        return code;
    }

    public void Split()
    {
        if (long.Parse(code) > 99999999999 && long.Parse(code) < 999999999999)
        {
            model = int.Parse(code.Substring(0, 2));

            move1 = int.Parse(code.Substring(2, 2));
            move2 = int.Parse(code.Substring(4, 2));
            move3 = int.Parse(code.Substring(6, 2));
            move4 = int.Parse(code.Substring(8, 2));

            bonusHealth = int.Parse(code.Substring(10, 2));
        }

        else if (int.Parse(code) > 99999999 && int.Parse(code) < 999999999)
        {
            model = int.Parse(code.Substring(3, 2));

            move1 = int.Parse(code.Substring(5, 2));
            move2 = int.Parse(code.Substring(7, 2));
            string rev = "";

            for (int i = 8; i > 2; i--)
            {
                rev += code.Substring(i, 1);
            }
            
            move3 = int.Parse(rev.Substring(0, 2));
            move4 = int.Parse(rev.Substring(2, 2));

            bonusHealth = int.Parse(rev.Substring(4, 2));
        }

        Pick();

        Debug.Log(model);
        Debug.Log(move1);
        Debug.Log(move2);
        Debug.Log(move3);
        Debug.Log(move4);
        Debug.Log(bonusHealth);
    }

    public void Pick()
    {
        if (model%3 == 0)
        {
            selector.selectedType = UnitModelSelector.UnitType.Rock;
        }
        else if (model%3 == 1)
        {
            selector.selectedType = UnitModelSelector.UnitType.Paper;
        }
        else if (model%3 == 2)
        {
            selector.selectedType = UnitModelSelector.UnitType.Scissors;
        }


    }
}
