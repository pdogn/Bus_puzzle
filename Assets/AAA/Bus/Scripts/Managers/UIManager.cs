using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startButton;

    [Header("Win")]
    [SerializeField] private GameObject WinUI;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button rePlayLevelButton;

    private void Start()
    {
        StartGameClick();

        PLayNextLevel();
    }

    public void StartGameClick()
    {
        startButton.onClick.AddListener(() =>
        {
            ManagerController.Instance.LoadGameData();
            startButton.gameObject.SetActive(false);
        });
    }

    public void PLayNextLevel()
    {
        nextLevelButton.onClick.AddListener(() =>
        {
            LevelManager.Instance.IncreaseLevel();
            ManagerController.Instance.LoadGameData();
            WinUI.gameObject.SetActive(false);
        });
    }

}
