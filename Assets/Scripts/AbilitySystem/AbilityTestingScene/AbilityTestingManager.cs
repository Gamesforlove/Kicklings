using Gameplay.CharacterComponents;
using Gameplay.Spawners;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AbilityTestingManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown LeftGoalkeeper;
    [SerializeField] private TMP_Dropdown LeftPlayer;
    [SerializeField] private TMP_Dropdown RightGoalkeeper;
    [SerializeField] private TMP_Dropdown RightPlayer;
    [SerializeField] private TMP_Dropdown _positionPreset;
    [SerializeField] private GameObject DropdownContainer;

    private AbilityActor LeftGoalkeeperAA;
    private AbilityActor LeftPlayerAA;
    private AbilityActor RightGoalkeeperAA;
    private AbilityActor RightPlayerAA;

    private Vector2 LeftGoalkeeperDefaultPos;
    private Vector2 LeftPlayerDefaultPos;
    private Vector2 RightGoalkeeperDefaultPos;
    private Vector2 RightPlayerDefaultPos;

    private BallScript ball;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DropdownContainer.SetActive(!DropdownContainer.activeSelf);
        }
    }
    private void FindBallAndPositions()
    {
        ball = FindFirstObjectByType<BallScript>();
        LeftGoalkeeperDefaultPos = LeftGoalkeeperAA.transform.position;
        LeftPlayerDefaultPos = LeftPlayerAA.transform.position;
        RightGoalkeeperDefaultPos = RightGoalkeeperAA.transform.position;
        RightPlayerDefaultPos = RightPlayerAA.transform.position;
    }
    private void OnEnable()
    {
        #if !UNITY_EDITOR
            gameObject.SetActive(false);
            return;
        #endif

        LeftGoalkeeper.onValueChanged.AddListener(delegate { OnLeftGoalkeeperDropdown(LeftGoalkeeper); });
        LeftPlayer.onValueChanged.AddListener(delegate { OnLeftPlayerDropdown(LeftPlayer); });
        RightGoalkeeper.onValueChanged.AddListener(delegate { OnRightGoalkeeperDropdown(RightGoalkeeper); });
        RightPlayer.onValueChanged.AddListener(delegate { OnRightPlayerDropdown(RightPlayer); });
        _positionPreset.onValueChanged.AddListener(delegate { OnPositionPresetDropdown(_positionPreset); });
    }
    private void OnDisable()
    {
        LeftGoalkeeper.onValueChanged.RemoveListener(delegate { OnLeftGoalkeeperDropdown(LeftGoalkeeper); });
        LeftPlayer.onValueChanged.RemoveListener(delegate { OnLeftPlayerDropdown(LeftPlayer); });
        RightGoalkeeper.onValueChanged.RemoveListener(delegate { OnRightGoalkeeperDropdown(RightGoalkeeper); });
        RightPlayer.onValueChanged.RemoveListener(delegate { OnRightPlayerDropdown(RightPlayer); });
        _positionPreset.onValueChanged.RemoveListener(delegate { OnPositionPresetDropdown(_positionPreset); });
    }
    public void SetUpAbilityActors(IReadOnlyList<AbilityActor> abilityActors)
    {
        if (abilityActors.Count == 0)
        {
            return;
        }

        LeftGoalkeeperAA = abilityActors.First(x => x.Team == Team.Left && x.PlayerType == PlayersSpawner.PlayerType.Goalkeeper);
        LeftPlayerAA = abilityActors.First(x => x.Team == Team.Left && x.PlayerType == PlayersSpawner.PlayerType.Normal);
        RightGoalkeeperAA = abilityActors.First(x => x.Team == Team.Right && x.PlayerType == PlayersSpawner.PlayerType.Goalkeeper);
        RightPlayerAA = abilityActors.First(x => x.Team == Team.Right && x.PlayerType == PlayersSpawner.PlayerType.Normal);

        OnLeftGoalkeeperDropdown(LeftGoalkeeper);
        OnLeftPlayerDropdown(LeftPlayer);
        OnRightGoalkeeperDropdown(RightGoalkeeper);
        OnRightPlayerDropdown(RightPlayer);


        Invoke(nameof(FindBallAndPositions), 1f);
    }

    private void OnLeftGoalkeeperDropdown(TMP_Dropdown dropdown)
    {
        Enum.TryParse(dropdown.captionText.text, out AbilityName ability);        
        LeftGoalkeeperAA.TestingAbility = ability;
    }
    private void OnLeftPlayerDropdown(TMP_Dropdown dropdown)
    {
        Enum.TryParse(dropdown.captionText.text, out AbilityName ability);
        LeftPlayerAA.TestingAbility = ability;
    }
    private void OnRightGoalkeeperDropdown(TMP_Dropdown dropdown)
    {
        Enum.TryParse(dropdown.captionText.text, out AbilityName ability);
        RightGoalkeeperAA.TestingAbility = ability;
    }
    private void OnRightPlayerDropdown(TMP_Dropdown dropdown)
    {
        Enum.TryParse(dropdown.captionText.text, out AbilityName ability);
        RightPlayerAA.TestingAbility = ability;
    }
    private void OnPositionPresetDropdown(TMP_Dropdown dropdown)
    {
        Enum.TryParse(dropdown.captionText.text, out PositionPreset result);
        switch (result)
        {
            case PositionPreset.Default:
                ball.transform.position = Vector3.zero;

                LeftGoalkeeperAA.GetComponent<IEntity>().Reset();
                LeftGoalkeeperAA.transform.SetPositionAndRotation(LeftGoalkeeperDefaultPos, Quaternion.identity);

                LeftPlayerAA.GetComponent<IEntity>().Reset();
                LeftPlayerAA.transform.SetPositionAndRotation(LeftPlayerDefaultPos, Quaternion.identity);

                RightGoalkeeperAA.GetComponent<IEntity>().Reset();
                RightGoalkeeperAA.transform.SetPositionAndRotation(RightGoalkeeperDefaultPos, Quaternion.identity);

                RightPlayerAA.GetComponent<IEntity>().Reset();
                RightPlayerAA.transform.SetPositionAndRotation(RightPlayerDefaultPos, Quaternion.identity);

                break;
            case PositionPreset.RBRB:
                ball.transform.position = LeftGoalkeeperAA.BallPoint.position;

                LeftGoalkeeperAA.GetComponent<IEntity>().Reset();
                LeftGoalkeeperAA.transform.SetPositionAndRotation(LeftGoalkeeperDefaultPos, Quaternion.identity);

                LeftPlayerAA.GetComponent<IEntity>().Reset();
                LeftPlayerAA.transform.SetPositionAndRotation(new Vector3(RightPlayerDefaultPos.x - 1.5f, RightPlayerDefaultPos.y, RightPlayerDefaultPos.y), Quaternion.identity);

                RightGoalkeeperAA.GetComponent<IEntity>().Reset();
                RightGoalkeeperAA.transform.SetPositionAndRotation(RightGoalkeeperDefaultPos, Quaternion.identity);

                RightPlayerAA.GetComponent<IEntity>().Reset();
                RightPlayerAA.transform.SetPositionAndRotation(new Vector3(LeftPlayerDefaultPos.x + 1.5f, LeftPlayerDefaultPos.y, LeftPlayerDefaultPos.y), Quaternion.identity);

                break;
            case PositionPreset.BallToGK:
                ball.transform.position = LeftGoalkeeperAA.BallPoint.position;
                break;
            case PositionPreset.BallToPlayer:
                ball.transform.position = LeftPlayerAA.BallPoint.position;
                break;
            default:
                break;
        }
    }
    private enum PositionPreset
    {
        Default,
        RBRB,
        BallToGK,
        BallToPlayer
    }
}
