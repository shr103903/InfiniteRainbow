using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutUI : MonoBehaviour
{
    public static FadeInOutUI instance = null;

    public Action loadAction = null;
    public float timeDeltaTime = 0f;
    private IEnumerator fadeOutCor = null;
    private IEnumerator fadeInCor = null;

    [SerializeField]
    private Image fadeImage = null;

    [SerializeField]
    private float time;

    private Ease linearEase = Ease.Linear;

    private Tweener fadeTweener = null;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void StartFadeOut(Action func = null)
    {
        if (func != null)
        {
            loadAction = func;
        }
        else
        {
            loadAction = null;
        }

        FadeOut();
    }

    public void StartFadeIn()
    {
        FadeIn();
    }

    public void StartScene()
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        FadeIn();
    }

    private void FadeOut()
    {
        if (fadeTweener != null)
        {
            fadeTweener.Kill();
        }

        fadeImage.gameObject.SetActive(true);

        fadeTweener = fadeImage.DOFade(1.0f, time).SetEase(linearEase).OnComplete(() =>
        {
            fadeImage.color = new Color(0, 0, 0, 1);

            if (loadAction != null)
            {
                loadAction.Invoke();
            }

            //FadeIn();
            StopCoroutine(fadeOutCor);
            //yield return null;
        });

    }

    private void FadeIn()
    {
        if (fadeTweener != null)
        {
            fadeTweener.Kill();
        }

        float alpha = 1f;
        fadeImage.color = new Color(0, 0, 0, alpha);

        fadeTweener = fadeImage.DOFade(0.0f, time).SetEase(linearEase).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
            StopCoroutine(fadeInCor);
            //yield return null;
        });
    }
}
