﻿using System.Collections;
using Cinemachine;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameplayStartup : MonoBehaviour
    {
        [SerializeField]
        private CharacterController characterController;
        
        [SerializeField]
        private HeartController heartController;

        [SerializeField]
        private Battle _battle;

        [SerializeField]
        private DialogViewModel _dialog;

        [SerializeField]
        private MonologViewModel _monolog;
        
        [SerializeField]
        private GameObject _input;

        [SerializeField]
        private UseButton _useButton;

        [SerializeField]
        private Joystick _joystick;

        [SerializeField]
        private SelectViewModel _select;

        [SerializeField]
        private CinemachineConfiner2D _cinemachineConfiner;

        [SerializeField]
        private CinemachineVirtualCamera _cinemachineVirtualCamera;
        
        [SerializeField]
        private GameObject _introduction;
        
        [SerializeField]
        private LocationsManager _locationsManager;
        
        [SerializeField]
        private TMP_Text _saveText;
        
        [SerializeField]
        private GameOverScreen _gameOver;
        
        [SerializeField]
        private CommandManager _commandManager;

        [SerializeField]
        private CompanionsManager _companionsManager;
        
        [SerializeField]
        private EndingsManager _endingsManager;

        [SerializeField]
        private SaverTimer _saverTimer;

        [SerializeField]
        private MMF_Player _impulseMMFPlayer;

        [SerializeField]
        private InputManager _inputManager;

        [SerializeField]
        private TransitionScreen _transitionScreen;
        
        private void Awake()
        {
            GameData.CharacterController = characterController;
            GameData.HeartController = heartController;
            GameData.Battle = _battle;
            GameData.Dialog = _dialog;
            GameData.Monolog = _monolog;
            GameData.Select = _select;
            GameData.UseButton = _useButton;
            GameData.Joystick = _joystick;
            GameData.CinemachineConfiner = _cinemachineConfiner;
            GameData.LocationsManager = _locationsManager;
            GameData.Introduction = _introduction;
            GameData.SaveText = _saveText;
            GameData.GameOver = _gameOver;
            GameData.CommandManager = _commandManager;
            GameData.CompanionsManager = _companionsManager;
            GameData.EndingsManager = _endingsManager;
            GameData.CinemachineVirtualCamera = _cinemachineVirtualCamera;
            GameData.SaverTimer = _saverTimer;
            GameData.InputManager = _inputManager;
            GameData.ImpulseMMFPlayer = _impulseMMFPlayer;
            GameData.TransitionScreen = _transitionScreen;
            
            _companionsManager.Register();
        }

        private void Start()
        {
            /*foreach (var saveLoad in FindObjectsByType<SaveLoadBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                saveLoad.Load();
            }*/

            //YandexGame.savesData.IsNotIntroduction = true;
            //Lua.Run("Value[\"FUN\"] = 1");

            /*if (!YandexGame.savesData.IsNotIntroduction)
            {
                _introduction.SetActive(true);
                YandexGame.savesData.IsNotFirstPlay = true;
            }
            else
            {*/
                _input.SetActive(true);
                GameData.Joystick.gameObject.SetActive(true);
                GameData.CharacterController.enabled = true;
                GameData.CharacterController.gameObject.SetActive(true);

                GameData.SaveLoadManager.LoadLevel();
                
                var testLoad = new TestLoad();
                //testLoad.Load();
                
                //GameData.LocationsManager.SwitchLocation(YandexGame.savesData.LocationName, YandexGame.savesData.PointIndex);
                //GameData.CharacterController.transform.position = GameData.Saver.LoadPosition();
            //}

            StartCoroutine(Await());
        }

        private void OnDestroy()
        {
            //Lua.UnregisterFunction(nameof(IsWin));
            //Lua.UnregisterFunction(nameof(IsUseCylinder));
        }
        
        private IEnumerator Await()
        {
            yield return new WaitForSeconds(0.5f);
            GameData.IsCanStartBattle = true;
        }
    }
}