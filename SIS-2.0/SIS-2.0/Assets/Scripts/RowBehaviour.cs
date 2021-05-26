using UnityEngine;
using UnityEngine.UI;

public class RowBehaviour : MonoBehaviour
{
    public Text Name;
    public Text Money;
    public Text Death;

    public void WriteData(string name, int money, int death)
    {
        Name.text = name;
        Money.text = money.ToString();
        Death.text = death.ToString();
    }
}
