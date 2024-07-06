using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject _helpPanel;
    [SerializeField] TextMeshProUGUI _enemyScoreText;
    [SerializeField] TextMeshProUGUI _playerScoreText;
    [SerializeField] BlinkedIUiElement _rotateAndShotText;
    [SerializeField] BlinkedIUiElement _doubleShotText;
    [SerializeField] BlinkedIUiElement _roundText;
    [SerializeField] TextMeshProUGUI _specialFireTimerText;
    [SerializeField] Image _specialFireImage;

    string _originalSpecialPanelText;
    Color _originalSpecialPanelColor;
    Coroutine _specialFireCooldownCO;

    void Awake()
    {
        _originalSpecialPanelText = _specialFireTimerText.text;
        _originalSpecialPanelColor = _specialFireImage.color;
    }

    public void StartCooldownPanelAnimation(float time)
    {
        if (_specialFireCooldownCO != null)
        {
            StopCoroutine(_specialFireCooldownCO);
            _specialFireCooldownCO = null;
        }

        _specialFireCooldownCO = StartCoroutine(SpecialShotCooldownCO(time));
    }

    IEnumerator SpecialShotCooldownCO(float time)
    {
        _originalSpecialPanelColor = _specialFireImage.color;
        _originalSpecialPanelText = _specialFireTimerText.text;

        _specialFireImage.color = Color.red;
        _specialFireTimerText.text = time.ToString();

        while (time >= 0)
        {
            time--;
            yield return new WaitForSeconds(1f);
            _specialFireTimerText.text = time.ToString();
        }

        _specialFireImage.color = _originalSpecialPanelColor;
        _specialFireTimerText.text = _originalSpecialPanelText;
    }

    public void OnGameResetHandler()
    {
        if (_specialFireCooldownCO != null)
        {
            StopCoroutine(_specialFireCooldownCO);
            _specialFireCooldownCO = null;
        }

        _specialFireImage.color = _originalSpecialPanelColor;
        _specialFireTimerText.text = _originalSpecialPanelText;
    }

    public void UpdateEnemyScoreText(int score)
    {
        _enemyScoreText.text = score.ToString();
    }

    public void UpdatePlayerScoreText(int score)
    {
        _playerScoreText.text = score.ToString();
    }

    public void UpdateRotateAndShotText(int count)
    {
        _rotateAndShotText.UpdateText("Rotate & Shot x" + count);
    }

    public void UpdateDoubleShotText(int count)
    {
        _doubleShotText.UpdateText("Double Shot x" + count);
    }

    public void UpdateRoundText(int round)
    {
        _roundText.UpdateText("Round " + round);
    }

    public void HelpPanelIsActive(bool status)
    {
        _helpPanel.SetActive(status);
    }
}
