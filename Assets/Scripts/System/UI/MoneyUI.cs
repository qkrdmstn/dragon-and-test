using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI txt;
    void Awake()
    {
        txt = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        txt.text = Player.instance?.money.ToString();
    }
}