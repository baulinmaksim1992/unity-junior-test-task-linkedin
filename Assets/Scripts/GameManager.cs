using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIController _ui;
    [SerializeField] PlayerController _player;
    [SerializeField] EnemyController _enemiePrefab;
    [SerializeField] float _helpPanelShowingTime;
    [SerializeField] float _maxEnemiesCount;
    [SerializeField] float _enemySpeedBase;
    [SerializeField] float _enemySpeedIncreaseByRound;
    [HideInInspector] public event Action OnRestart;
    [HideInInspector] public event Action OnGameReset;
    [HideInInspector] public List<EnemyController> SpawnedEnemies;

    int _playerScore;
    int _enemyScore;
    int _doubleShot;
    int _rotateAndShot;
    int _round;

    void Start()
    {
        SpawnedEnemies = new List<EnemyController>();
        ResetGame();
    }

    void AddEnemyOnScene()
    {
        var position = new Vector3(transform.position.x, 0, transform.position.z - (_round * 4));
        var enemy = Instantiate(_enemiePrefab, position, Quaternion.identity);
        SpawnedEnemies.Add(enemy);

        var increaseSpeed = _round <= _maxEnemiesCount ? _round * _enemySpeedIncreaseByRound : 0;

        enemy.SetSpeed(_enemySpeedBase + increaseSpeed);
    }

    public void StartNextRaund()
    {
        _round++;

        //переместить на стартовые позици игрока и врага
        _player.SetSpawnPosition();

        if (SpawnedEnemies != null && SpawnedEnemies.Any())
        {
            for (int i = 0; i < SpawnedEnemies.Count(); i++)
            {
                if (SpawnedEnemies[i] != null)
                {
                    Destroy(SpawnedEnemies[i].gameObject);
                }
            }
        }

        SpawnedEnemies.Clear();

        for (int i = 0; i < _round; i++)
        {
            if (i <= _maxEnemiesCount)
            {
                AddEnemyOnScene();
            }
        }

        UpdateUiElements();

        //показать временно хелп панель

        if (_round == 1)
        {
            StartCoroutine(ShowHelpPanelTemporaryCO());
        }
        
        if (OnGameReset != null)
        {
            OnGameReset.Invoke();
        }
    }

    public void ResetGame()
    {
        //сбросить счета
        _playerScore = 0;
        _enemyScore = 0;
        _doubleShot = 0;
        _rotateAndShot = 0;
        _round = 0;

        StartNextRaund();
    }

    void UpdateUiElements()
    {
        _ui.UpdateEnemyScoreText(_enemyScore);
        _ui.UpdatePlayerScoreText(_playerScore);
        _ui.UpdateDoubleShotText(_doubleShot);
        _ui.UpdateRotateAndShotText(_rotateAndShot);
        _ui.UpdateRoundText(_round);
    }

    IEnumerator ShowHelpPanelTemporaryCO()
    {
        _ui.HelpPanelIsActive(true);
        yield return new WaitForSeconds(_helpPanelShowingTime);
        _ui.HelpPanelIsActive(false);
    }

    public void PlayerGetShotHandler()
    {
        AddEnemyScore(1);
        StartNextRaund();
    }

    public void EnemyGetShotHandler()
    {
        AddPlayerScore(1);
    }

    public void AddPlayerScore(int score)
    {
        _playerScore += score;
        UpdateUiElements();
    }

    public void AddEnemyScore(int score)
    {
        _enemyScore += score;
        UpdateUiElements();
    }

    public void AddDoubleShotStat()
    {
        _doubleShot++;
        _ui.UpdateDoubleShotText(_doubleShot);
    }

    public void AddRotateAndShotStat()
    {
        _rotateAndShot++;
        _ui.UpdateRotateAndShotText(_rotateAndShot);
    }
}
