using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButton : MonoBehaviour
{
    public List<Button> buttons = new List<Button>();
    public Button desiredButton;

    public void ChangeColor() 
    {
        foreach (Button button in buttons)
        {
            if (button == desiredButton) button.GetComponent<Image>().color = new Color32(135, 128, 128, 255);
            else button.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }
}
