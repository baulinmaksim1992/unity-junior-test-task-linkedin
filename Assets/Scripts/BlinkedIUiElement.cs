using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlinkedIUiElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image _panelImage;
    [SerializeField] Color _blinkColor;
    Color _originalColor;
    Coroutine _blinckCO;

    void Awake()
    {
        _originalColor = _panelImage.color;
    }

    public void UpdateText(string newText)
    {
        _text.text = newText;

        if (_blinckCO != null)
        {
            StopCoroutine(_blinckCO);
        }

        _blinckCO = StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        // Установите цвет Image на красный
        _panelImage.color = _blinkColor;

        // Плавный возврат цвета к исходному в течение 0.25 секунды
        float duration = 0.25f; // Длительность анимации
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _panelImage.color = Color.Lerp(_blinkColor, _originalColor, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедитесь, что цвет окончательно установлен в исходный
        _panelImage.color = _originalColor;
    }
}
