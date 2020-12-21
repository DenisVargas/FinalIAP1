using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image foregroundImage = null;
    [SerializeField] Image frontImage = null;
    [SerializeField] float FadeTime = 2f;
    [SerializeField] Canvas canvas = null;

    public float value
    {
        get => foregroundImage.fillAmount;
        set =>  foregroundImage.fillAmount = value;
    }

    private void Awake()
    {
        canvas.worldCamera = Camera.main;
        foregroundImage.fillAmount = 1f;
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
        //transform.Rotate(0, 180, 0);
    }

    public void FadeOut()
    {
        foregroundImage.CrossFadeAlpha(0, FadeTime, false);
        frontImage.CrossFadeAlpha(0, FadeTime, false);
    }
}
