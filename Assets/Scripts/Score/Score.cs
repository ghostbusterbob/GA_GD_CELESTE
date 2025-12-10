using UnityEngine;
using TMPro;


public class Score : MonoBehaviour
//zorgt ervoor dat de score omhoog gaat als de speler een aardbei pakt
{
    public TMP_Text strawberryText;
    [SerializeField] private int strawberryCount = 3;

    public void AddStrawberry()
    {
        strawberryCount++;
        strawberryText.text = "X " + strawberryCount.ToString();
    }
}
