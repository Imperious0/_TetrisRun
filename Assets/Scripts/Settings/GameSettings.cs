using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Settings/Game Settings")]
public class GameSettings : ScriptableObject
{
    //General Settings
    [Header("Player Settings")]
    [Range(0f, 100f), SerializeField]
    float _playerSpeed = 1f;
    [Range(0.01f, 100f), SerializeField]
    float _playerRewindSpeedMultiplier = 1f;

    [Header("Block Settings")]
    [Range(0.01f, 10f), SerializeField]
    float _blockTimeLapse = 1f;
    [Range(1, 360), SerializeField]
    int _maxFlipCount = 1;
    [Range(0.01f, 100), SerializeField]
    float _blockForce = 50f;

    [Header("Game Settings")]
    [SerializeField]
    float _gamePlaySpeed = 1f;
    [Range(1, 100), SerializeField]
    int _gameMaxDifficulty = 1;

    public float PlayerSpeed { get => _playerSpeed; }
    public float BlockTimeLapse { get => _blockTimeLapse; }
    public float GamePlaySpeed { get => _gamePlaySpeed; }
    public int MaxFlipCount { get => _maxFlipCount; }
    public int GameMaxDifficulty { get => _gameMaxDifficulty; }
    public float BlockForce { get => _blockForce; }
    public float PlayerRewindSpeedMultiplier { get => _playerRewindSpeedMultiplier; }
}
