using Gameplay.CharacterComponents.Player;
using Scene_Management;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents
{
    public interface IEntity
    {
        public void SetUp(EntityData entityData);
        public void Reset();
    }

    public abstract class Entity : MonoBehaviour, IEntity
    {
        protected EntityData EntityData;
        
        protected JointsController JointsController;
        protected PlayerActions PlayerActions;
        protected ClothesSetter ClothesSetter;
        protected BodyPartsController BodyPartsController;
        protected StabilizeComponent StabilizeComponent;
        protected Rigidbody2D Rigidbody;

        public virtual void SetUp(EntityData entityData)
        {
            EntityData = entityData;
            CacheComponents();
            
            bool isRightSide = gameObject.transform.position.x > 0;
            if (isRightSide)
                ChangeValuesForRightSide();
            
            JointsController.SetJointsLimits();
            SetCharacterClothes(isRightSide);
            PlayerActions.SetUp(EntityData);
            StabilizeComponent.SetUp(EntityData);
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
            ClothesSetter = GetComponent<ClothesSetter>();
            BodyPartsController = GetComponent<BodyPartsController>();
            StabilizeComponent = GetComponent<StabilizeComponent>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void ChangeValuesForRightSide()
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        
        void SetCharacterClothes(bool isRightSide)
        {

            if (isRightSide)
            {
                if (!MatchFlow.Match.Settings.IsTournamentMatch)
                    SetClothesFreeMode(isRightSide);
                else
                    ClothesSetter.SetClothes(MatchFlow.Match.Settings.RightCountryImageIndex);
            }
            else
            {
                if (!MatchFlow.Match.Settings.IsTournamentMatch)
                    SetClothesFreeMode(isRightSide);
                else
                    ClothesSetter.SetClothes(MatchFlow.Match.Settings.LeftCountryImageIndex);
            }
        }

        private void SetClothesFreeMode(bool isRightSide)
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