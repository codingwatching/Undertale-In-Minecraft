using System.Collections;
using Cinemachine;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Game
{
    public class BedTeleport : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        [SerializeField]
        private CinemachineVirtualCamera _cinemachineVirtual;

        [SerializeField]
        private GenocideGameEnd _end;
        
        private Vector2 _startPosition;
        
        public void Use()
        {
            GameData.CoroutineRunner.StartCoroutine(AwaitUse());
        }

        private IEnumerator AwaitUse()
        {
            GameData.CompanionsManager.DeactivateAllCompanion();
            
            var characterTransform = GameData.CharacterController.transform;
            _startPosition = characterTransform.position;
            GameData.CharacterController.enabled = false;

            GameData.CharacterController.View.Flip(true);
            _spriteRenderer.GetComponent<BoxCollider2D>().enabled = false;
            
            _spriteRenderer.sortingOrder = -1;
            
            var characterJumpMoveToUpCommand = new MoveToPointCommand(characterTransform, characterTransform.position.AddY(1), 0.25f);
            yield return characterJumpMoveToUpCommand.Await();

            var rotationCommand = new RotationCommand(characterTransform, new Vector3(0, 0, -90), 0.5f);
            yield return rotationCommand.Await();
            
            var characterJumpToBedCommand = new MoveToPointCommand(characterTransform, transform.position.AddY(0.75f).AddX(-0.5f), 0.5f);
            yield return characterJumpToBedCommand.Await();
            
            GameData.EffectSoundPlayer.Play(GameData.AssetProvider.JumpSound);

            _cinemachineVirtual.gameObject.SetActive(true);
            float size = 8;
            
            do
            {
                size -= 4 * Time.deltaTime;
                _cinemachineVirtual.m_Lens.OrthographicSize = size;
                yield return null;
            } while (size > 0.5f);

            yield return new WaitForSeconds(2);
            
            GameData.CharacterController.View.Sleep();

            yield return new WaitForSeconds(2);

            if (Lua.IsTrue("Variable[\"KILLS\"] >= 5"))
            {
                characterTransform.eulerAngles = Vector3.zero;
                GameData.CharacterController.View.Reset();
                GameData.CharacterController.enabled = true;
                GameData.LocationsManager.SwitchLocation("FakeHerobrineHome", 1);   
            }
            else
                _end.Use();

            /*GameData.CharacterController.View.Reset();
            
            yield return new WaitForSeconds(1);
            
            do
            {
                size += 4 * Time.deltaTime;
                _cinemachineVirtual.m_Lens.OrthographicSize = size;
                yield return null;
            } while (size < 8.0f);
            
            var characterJumpMoveToUpCommand1 = new MoveToPointCommand(characterTransform, characterTransform.position.AddY(1), 0.25f);
            yield return characterJumpMoveToUpCommand1.Await();
            
            var rotationCommand1 = new RotationCommand(characterTransform, new Vector3(0, 0, 0), 0.5f);
            yield return rotationCommand1.Await();
            
            var characterJumpToBedCommand1 = new MoveToPointCommand(characterTransform, _startPosition, 0.5f);
            yield return characterJumpToBedCommand1.Await();
            
            _spriteRenderer.sortingOrder = 0;
            GameData.CharacterController.enabled = true;
            _cinemachineVirtual.gameObject.SetActive(false);*/
        }
    }
}