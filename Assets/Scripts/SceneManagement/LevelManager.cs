using DesignPatterns.Generics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{

    [SerializeField] GameObject loaderCanvas;
    [SerializeField] Image progressBar;
    [SerializeField] Image fadePanel;
    [SerializeField] float fadeSpeed;
    [SerializeField] List<string> userTips;
    [SerializeField] TextMeshProUGUI userTipText;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(
            PreLoadFadeIn(
                () => StartCoroutine(
                    LoadSceneAsync(sceneName, LoadSceneMode.Single,
                    () =>
                    {
                        loaderCanvas.SetActive(true);
                        progressBar.fillAmount = 0;
                        string tip = userTips[UnityEngine.Random.Range(0, userTips.Count)];
                        userTipText.text = tip;
                    }, 
                    () =>
                    {
                        loaderCanvas.SetActive(false);
                        progressBar.fillAmount = 0;
                        StartCoroutine(PostLoadFadeOut(null));
                    }))));
    }

    public void AddScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive,
            () => loaderCanvas.SetActive(false), null));
    }

    private IEnumerator PreLoadFadeIn(Action onPreLoadEnd)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0);
        while (true)
        {
            yield return new WaitForEndOfFrame();
            fadePanel.color = new Color(
                fadePanel.color.r, 
                fadePanel.color.g,
                fadePanel.color.b, 
                Time.deltaTime * fadeSpeed + fadePanel.color.a);

            if (fadePanel.color.a >= 1)
            {
                fadePanel.color = new Color(
                    fadePanel.color.r,
                    fadePanel.color.g,
                    fadePanel.color.b,
                    1);

                break;
            }
        }

        onPreLoadEnd?.Invoke();
        fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator LoadSceneAsync(
        string sceneName, LoadSceneMode loadMode, Action onLoadSceneStart, Action onLoadSceneEnd)
    {
        onLoadSceneStart?.Invoke();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            progressBar.fillAmount = Mathf.Clamp01(operation.progress / 0.9f);
            yield return new WaitForEndOfFrame();
            if (progressBar.fillAmount == 1)
            {
                // aggiungere delay per dare più una sensazione di ritardo
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }
        }
        onLoadSceneEnd?.Invoke();
    }

    private IEnumerator PostLoadFadeOut(Action onPostLoadEnd)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1);
        while (true)
        {
            yield return new WaitForEndOfFrame();
            fadePanel.color = new Color(
                fadePanel.color.r,
                fadePanel.color.g,
                fadePanel.color.b,
                fadePanel.color.a - Time.deltaTime * fadeSpeed);

            if (fadePanel.color.a <= 0)
            {
                fadePanel.color = new Color(
                    fadePanel.color.r,
                    fadePanel.color.g,
                    fadePanel.color.b,
                    0);

                break;
            }
        }
        
        onPostLoadEnd?.Invoke();
        fadePanel.gameObject.SetActive(false);
    }

}
