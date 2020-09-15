using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaluatingPanel : MonoBehaviour
{
    public Image accuracyBarImage;

    public Text accuracyText;

    public Text rewardMoneyText;

    public void StartEvaluating(float accuracy)
    {
        StartCoroutine(ScheduleAccuracyBarImageFillAmout(accuracy));
    }

    IEnumerator ScheduleAccuracyBarImageFillAmout(float accuracy)
    {        
        float currentAccuracy = 0f;
        float increaseSpeed = 0.3f;

        while (currentAccuracy < accuracy)
        {
            currentAccuracy = Mathf.Min(currentAccuracy + increaseSpeed * Time.deltaTime, accuracy + 0.00001f);

            accuracyBarImage.fillAmount = currentAccuracy;
            accuracyText.text = "Accuracy: " + Mathf.RoundToInt(currentAccuracy * 100f).ToString() + "%";

            rewardMoneyText.text = "+ $" + Mathf.RoundToInt(currentAccuracy * 500f).ToString();

            yield return null;
        }     
    }
}
