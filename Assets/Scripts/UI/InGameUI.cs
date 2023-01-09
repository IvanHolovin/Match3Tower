using System;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Update()
    {
        _scoreText.text = GameManager.Instance.Score.ToString();
    }
}
