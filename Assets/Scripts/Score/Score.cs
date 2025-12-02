using UnityEngine;
using TMPro;


public class Score : MonoBehaviour
{
    public TMP_Text strawberryText;
    [SerializeField] private int strawberryCount = 3;

    public void AddStrawberry()
    {
        strawberryCount++;
        strawberryText.text = "X " + strawberryCount.ToString();
    }
}
