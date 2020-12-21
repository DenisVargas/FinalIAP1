using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image foregroundImage = null;
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
}
