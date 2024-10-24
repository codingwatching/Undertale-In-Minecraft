﻿using UnityEngine;

namespace Game
{
    public class Plate : MonoBehaviour
    {
        [SerializeField] 
        private Sprite _activeSprite;

        [SerializeField] 
        private Sprite _deactivateSprite;
        
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField] 
        private PlaySoundEffect _playSound;
        
        public bool IsActive;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out CharacterController character) && !IsActive)
            {
                Activate();
                _playSound.Play();
            }
        }

        public void Activate()
        {
            _spriteRenderer.sprite = _activeSprite;
            IsActive = true;
        }

        public void Deactivate()
        {
            _spriteRenderer.sprite = _deactivateSprite;
            IsActive = false;
        }
    }
}