using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startButton;

    [Header("Win")]
    [SerializeField] private GameObject WinUI;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button rePlayLevelButton;

    [Header("Lost")]
    [SerializeField] private GameObject LostUI;
    [SerializeField] private Button rePlayLevelButton1;

    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartGameClick);

        nextLevelButton.onClick.AddListener(PLayNextLevel);

        rePlayLevelButton.onClick.AddListener(RePlayLevel);
        rePlayLevelButton1.onClick.AddListener(RePlayLevel);
    }
    public void StartGameClick()
    {
        ManagerController.Instance.LoadGameData();
        startButton.gameObject.SetActive(false);
    }

    public void PLayNextLevel()
    {
        LevelManager.Instance.IncreaseLevel();
        ManagerController.Instance.LoadGameData();
        WinUI.gameObject.SetActive(false);
    }

    public void RePlayLevel()
    {
        ManagerController.Instance.LoadGameData();
        WinUI.gameObject.SetActive(false);
        LostUI.gameObject.SetActive(false);
    }

    public void ShowWinUI()
    {
        StartCoroutine(DelayShowUI(WinUI, 0.1f));
    }

    public void ShowLostUI(float time)
    {
        StartCoroutine(DelayShowUI(LostUI, time));
    }

    IEnumerator DelayShowUI(GameObject ui, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        ui.SetActive(true);
        ui.transform.localScale = Vector3.zero;
        ui.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
}
