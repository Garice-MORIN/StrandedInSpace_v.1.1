using UnityEngine;
using UnityEngine.UI;

public class RowBehaviour : MonoBehaviour
{
    public Text name;
    public Text money;
    public Text death;

    public void WriteData(string name, int money, int death)
    {
        this.name.text = name;
        this.money.text = money.ToString();
        this.death.text = death.ToString();
    }
}
