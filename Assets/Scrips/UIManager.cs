using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField]
    TextMeshProUGUI _AILevel;
    [SerializeField]
    TextMeshProUGUI _inputFeedback;
    [SerializeField]
    TextMeshProUGUI _AIText;
    [SerializeField]
    TextMeshProUGUI _playerText;
    [SerializeField]
    float _fadeInDuration = 1f;

    private Vector3 originalPositionPlayer;
    private Vector3 originalPositionIA;

    [Header("Heal Bars")]
    [SerializeField]
    private float _fadeDuration = 1f;
    [SerializeField]
    Slider _playerHealBar;
    [SerializeField]
    Slider _AIHealBar;

    float _timerFB = 0;
    private void Start()
    {
        GameManager.Instance.StartGame(this);

        originalPositionPlayer = _playerText.rectTransform.localPosition;
        originalPositionIA = _AIText.rectTransform.localPosition;

        _playerText.alpha = 0f;
        _AIText.alpha = 0f;
    }
    //Handle Feedback of Turn
    public void SetTurn(GameState state)
    {
        switch (state)
        {
            case GameState.MoveAI:
                _playerText.enabled = false;
                _AIText.enabled = true;
                _AIText.text = "IAn IS MOVING";
                StartCoroutine(AnimateText(_AIText, originalPositionIA));
                break;

            case GameState.MovePlayer:
                _AIText.enabled = false;
                _playerText.enabled = true;
                _playerText.text = "YOU MOVE";
                StartCoroutine(AnimateText(_playerText, originalPositionPlayer));
                break;

            case GameState.FireAI:
                _playerText.enabled = false;
                _AIText.enabled = true;
                _AIText.text = "IAn IS FIRING";
                StartCoroutine(AnimateText(_AIText, originalPositionIA));
                break;

            case GameState.FirePlayer:
                _AIText.enabled = false;
                _playerText.enabled = true;
                _playerText.text = "YOU FIRE";
                StartCoroutine(AnimateText(_playerText, originalPositionPlayer));
                break;

            default:
                Debug.LogWarning("Unknown state");
                break;
        }
    }

    private IEnumerator AnimateText(TextMeshProUGUI _TurnText, Vector3 originalPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = originalPosition + new Vector3(0, 50, 0);

        while (elapsedTime < _fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            _TurnText.alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeInDuration);
            _TurnText.rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / _fadeInDuration);
            yield return null;
        }

        _TurnText.alpha = 1f;
        _TurnText.rectTransform.localPosition = originalPosition;
    }


    //Handle Life Bars
    public void UpdateLife(TankType type, float targetProgress)
    {
        if (type == TankType.Player)
        {
            StartCoroutine(UpdateHealthBar(_playerHealBar, targetProgress));
        }
        else
        {
            StartCoroutine(UpdateHealthBar(_AIHealBar, targetProgress));
        }
    }

    private IEnumerator UpdateHealthBar(Slider healthBar, float targetProgress)
    {
        float initialProgress = healthBar.value;
        float elapsedTime = 0f;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            healthBar.value = Mathf.Lerp(initialProgress, targetProgress, elapsedTime / _fadeDuration);

            yield return null;
        }

        healthBar.value = targetProgress;
    }

    //Handle feedback if outside
    public void ShowMessage(string message)
    {
        if(!_inputFeedback.gameObject.activeSelf)
        {
            _inputFeedback.gameObject.SetActive(true);
            _inputFeedback.text = message;
            _timerFB = 5;
            StartCoroutine(FadeLoop());
        }
    }

    IEnumerator FadeLoop()
    {
        for (int i = 0; i < 3; i++)
        {
            // Fade In (aparecer)
            yield return StartCoroutine(FadeText(0f, 1f, 1f));

            // Fade Out (desaparecer)
            yield return StartCoroutine(FadeText(1f, 0f, 1f));
        }
    }

    IEnumerator FadeText(float startAlpha, float endAlpha, float fadeDuration)
    {
        Color originalColor = _inputFeedback.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            _inputFeedback.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;  
        }
        _inputFeedback.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    //Handle level of AI

    public void SetAILevel(string level)
    {
        _AILevel.text = "AI Level: " + level;
    }
}
