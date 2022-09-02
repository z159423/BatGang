using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TimeScaler : MonoBehaviour
{
    public static TimeScaler instance;

    private Coroutine timeScaleCoroutine;
    public TextMeshProUGUI text;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        text.text = Time.timeScale.ToString();
    }

    public void ChangeTimeScale(float scale)
    {
        //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, scale, 0.5f).SetEase(Ease.OutExpo).SetUpdate(true).OnComplete(() => comeback());

        Time.timeScale = scale;
        

        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 1.5f).SetEase(Ease.InCubic).SetUpdate(true);

    }

    private void comeback()
    {
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 1.5f).SetEase(Ease.InCubic).SetUpdate(true);
    }
}