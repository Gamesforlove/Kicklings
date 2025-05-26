using Gameplay.CharacterComponents.Player;
using Scene_Management;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents
{
    public interface IEntity
    {
        public void SetUp();
        public void Reset();
    }
    public abstract class Entity : MonoBehaviour, IEntity
    {
        protected JointsController JointsController;
        protected PlayerActions PlayerActions;
        protected PlayerIndicator PlayerIndicator;
        protected ClothesSetter ClothesSetter;
        protected BodyPartsController BodyPartsController;
        
        protected PlayerInput PlayerInput;
        protected Rigidbody2D Rigidbody;

        public virtual void SetUp()
        {
            CacheComponents();
            
            bool isRightSide = gameObject.transform.position.x > 0;
            
            JointsController.SetJointsLimits();
            SetCharacterClothes(isRightSide);
            
            if (isRightSide)
                ChangeValuesForRightSide();
        }

        public virtual void Reset()
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = 0;
            transform.eulerAngles = Vector3.zero;
            
            BodyPartsController.ResetBodyParts();
            JointsController.ResetJoints();
        }

        void CacheComponents()
        {
            JointsController = GetComponent<JointsController>();
            PlayerActions = GetComponent<PlayerActions>();
            PlayerIndicator = GetComponentInChildren<PlayerIndicator>();
            ClothesSetter = GetComponent<ClothesSetter>();
            BodyPartsController = GetComponent<BodyPartsController>();
            
            PlayerInput = GetComponent<PlayerInput>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void ChangeValuesForRightSide()
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            PlayerActions.KickingLegSpeed *= -1;
        }
        
        void SetCharacterClothes(bool isRightSide)
        {

            if (isRightSide)
            {
                ClothesSetter.SetClothes(
                    MatchFlow.Match.Settings.RightSideShirtIndex,
                    MatchFlow.Match.Settings.RightSideShoesIndex
                );
            }
            else
            {
                ClothesSetter.SetClothes(
                    MatchFlow.Match.Settings.LeftSideShirtIndex,
                    MatchFlow.Match.Settings.LeftSideShoesIndex
                );
            }
        }
    }
}