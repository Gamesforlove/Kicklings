using Gameplay.CharacterComponents.Player;
using Scene_Management;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents
{
    public abstract class Entity : MonoBehaviour
    {
        protected JointsConfigurator JointsConfigurator;
        protected PlayerActions PlayerActions;
        protected PlayerInput PlayerInput;
        protected PlayerIndicator PlayerIndicator;
        protected ClothesSetter ClothesSetter;

        public virtual void SetUp()
        {
            CacheComponents();
            
            bool isRightSide = gameObject.transform.position.x > 0;
            
            JointsConfigurator.SetJointsLimits();
            SetCharacterClothes(isRightSide);
            
            if (isRightSide)
                ChangeValuesForRightSide();
        }
        
        void CacheComponents()
        {
            JointsConfigurator = GetComponent<JointsConfigurator>();
            PlayerActions = GetComponent<PlayerActions>();
            PlayerInput = GetComponent<PlayerInput>();
            PlayerIndicator = GetComponentInChildren<PlayerIndicator>();
            ClothesSetter = GetComponent<ClothesSetter>();
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