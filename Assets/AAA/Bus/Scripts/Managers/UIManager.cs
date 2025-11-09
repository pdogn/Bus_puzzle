using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        StartGameClick();


    }

    public void StartGameClick()
    {
        startButton.onClick.AddListener(() =>
        {
            ManagerController.Instance.LoadGameData();
            startButton.gameObject.SetActive(false);
        });
    }

}
