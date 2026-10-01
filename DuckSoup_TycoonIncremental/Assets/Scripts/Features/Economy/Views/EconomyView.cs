using TMPro;
using UnityEngine;

public class EconomyView : MonoBehaviour
{
    [SerializeField]private TMP_Text moneyText;

    public void SetMoney(string value)
    {
        moneyText.text = value;
    }
}