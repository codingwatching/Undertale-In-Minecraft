﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using YG;
using Random = UnityEngine.Random;

namespace Game
{
    public class Battle : MonoBehaviour
    {
        [SerializeField]
        private AttackBase _attackTutorial;

        [SerializeField]
        private BlackPanel _blackPanel;

        [SerializeField]
        private BattleMessageBox _enemyMessageBox;

        [SerializeField]
        private BattleMessageBox _messageBox;
        
        [SerializeField]
        private SelectActManager _selectActManager;

        [SerializeField]
        private SpriteRenderer _arena;

        [SerializeField]
        private Transform[] _points;

        [SerializeField]
        private PlaySound _startBattlePlaySound;

        [SerializeField]
        private PlaySound _levelUpPlaySound;

        [SerializeField]
        private PlaySound _sparePlaySound;
        
        [Header("Variables")]
        [SerializeField]
        private float _speedPlacement;

        [SerializeField]
        private LocalizedString _winReplica;

        [SerializeField]
        private LocalizedString _winReplicaCheat;

        [SerializeField]
        private TMP_Text _addProgressLabel;

        [SerializeField]
        private AddProgressData _addProgressData;

        [SerializeField]
        private Transform _actScreenContainer;
        
        private Label _healthLabel;
        private Label _enemyHealthLabel;
        private Vector2 _normalWorldCharacterPosition;
        private Coroutine _coroutine;
        private AudioClip _previousSound;
        private Vector2 _enemyStartPosition;
        private AttackBase[] _attacks;
        private int _attackIndex;
        private bool _isSecondRound;

        public BlackPanel BlackPanel => _blackPanel;
        public GameObject Arena => _arena.gameObject;
        public SelectActManager SelectActManager => _selectActManager;
        public Transform ActScreenContainer => _actScreenContainer;
        public BattleMessageBox EnemyMessageBox => _enemyMessageBox;
        public TMP_Text AddProgressLabel => _addProgressLabel;
        public AddProgressData AddProgressData => _addProgressData;

        public int? AddProgress = null;

        private void OnDisable()
        {
            EventBus.Death = null;
            EventBus.Damage = null;
            
            if (GameData.EnemyData != null)
            {
                if (GameData.EnemyData.GameObject != null && GameData.EnemyData.StartBattleTrigger != null)
                    GameData.EnemyData.GameObject.transform.SetParent(GameData.EnemyData.StartBattleTrigger.transform);

                GameData.EnemyData.StartBattleTrigger = null;
            }
        }

        public void StartBattle()
        {
            _normalWorldCharacterPosition = GameData.CharacterController.transform.position;
            GameData.CharacterController.enabled = false;
            GameData.HeartController.enabled = false;
            GameData.HeartController.transform.position = _arena.transform.position;
            _previousSound = GameData.MusicPlayer.Clip;
            GameData.Saver.IsSavingPosition = false;
            GameData.InputManager.Show();
            GameData.CompanionsManager.SetMove(false);

            _isSecondRound = false;
            gameObject.SetActive(true);

            transform.position = 
                Camera.main.transform.position.SetZ(0).AddY(-3.5f) 
                + (Vector3) GameData.EnemyData.StartBattleTrigger.Offset;
            
            GameData.EnemyData.GameObject.transform.SetParent(GameData.EnemyPoint);

            if (GameData.EnemyData.GameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.flipX = true;

            var character = GameData.CharacterController;
            character.GetComponent<Collider2D>().isTrigger = true;
            character.View.Flip(false);
            character.View.SetOrderInLayer(11);
            
            _arena.size = Vector2.zero;
            GameData.HeartController.gameObject.SetActive(false);

            YandexGame.savesData.Health = YandexGame.savesData.MaxHealth;
            EventBus.HealthChange.Invoke(YandexGame.savesData.MaxHealth, YandexGame.savesData.Health);
            
            GameData.BattleProgress = 0;
            EventBus.BattleProgressChange?.Invoke(0);
            
            _attackIndex = 0;

            EventBus.Damage += OnDamage;
            EventBus.Death += OnDeath;
            
            _attacks = GameData.EnemyData.EnemyConfig.Attacks;

            var commands = new List<CommandBase>()
            {
                new IntroCommand(_startBattlePlaySound, _points),
                //new SkipIntroCommand(_points),
                new DelayCommand(1f),
                new StartEnemyTurnCommand(),
            };
            
            GameData.CommandManager.StartCommands(commands);
        }

        public void Turn(BaseActConfig act = null)
        {
            var commands = new List<CommandBase>();

            /*if (act != null)
            {
                commands.Add(new MessageCommand(_messageBox, act.Description));
                commands.Add(new MessageCommand(_enemyMessageBox, act.Reaction));
                commands.Add(new AddProgressCommand(act.Progress, _addProgressLabel, _addProgressData));
            }*/
            
            //commands.Add(new CheckEndBattleCommand());
            
            //if (!_isSecondRound && YandexGame.savesData.IsTutorialComplited && _attacks[_attackIndex].Messages != null && _attacks[_attackIndex].Messages.Length != 0) 
            //    commands.Add(new MessageCommand(_enemyMessageBox, _attacks[_attackIndex].Messages));

            //commands.Add(new ShowArenaCommand(_arena, _blackPanel));
            
            //if (!YandexGame.savesData.IsTutorialComplited)
            //    commands.Add(new EnemyAttackCommand(_attackTutorial, _blackPanel, _arena.gameObject)); 
            
            //commands.Add(new EnemyAttackCommand(_attacks[_attackIndex], _blackPanel, _arena.gameObject));
            //commands.Add(new CheckEndBattleCommand());
            //commands.Add(new HideArenaCommand(_arena, _blackPanel));
            commands.Add(new StartCharacterTurnCommand());

            GameData.CommandManager.StartCommands(commands);
            GetIndex();
        }

        public void EndBattle()
        {
            GameData.CharacterController.View.SetOrderInLayer(0);
            GameData.CompanionsManager.SetMove(true);
            var winReplica = YandexGame.savesData.IsCheat ? _winReplicaCheat : _winReplica;
            
            var commands = new List<CommandBase>();
            
            commands.Add(new DelayCommand(1f));
            commands.Add(new MessageCommand(_enemyMessageBox, GameData.EnemyData.EnemyConfig.EndReplicas));
            commands.Add(new ExitCommand(gameObject, _sparePlaySound, _levelUpPlaySound, _previousSound,
                _normalWorldCharacterPosition, _speedPlacement, winReplica));
            
            GameData.CommandManager.StartCommands(commands);
        }

        public void StartCharacterTurn()
        {
            //GameData.CharacterController.View.Dance();
            _selectActManager.Activate(true);
        }

        private void GetIndex()
        {
            if (YandexGame.savesData.IsTutorialComplited)
                _attackIndex++;

            if (_attackIndex >= _attacks.Length)
            {
                _isSecondRound = true;
                _attackIndex = Random.Range(0, _attacks.Length);
            }
        }

        private void OnDeath()
        {
            StopCoroutine(_coroutine);
        }

        private void OnDamage(int value)
        {
            GameData.CharacterController.View.Damage();
        }

        [ContextMenu("Progress_100")]
        private void Progress_100()
        {
            GameData.BattleProgress = 100;
        }

        public AttackBase CreateAttack(AttackBase attackPrefab)
        {
            return Instantiate(attackPrefab.gameObject, transform).GetComponent<AttackBase>();
        }
    }
}