using Gameplay.Managers;
using Gameplay.Spawners;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTestingManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown LeftGoalkeeper;
    [SerializeField] private TMP_Dropdown LeftPlayer;
    [SerializeField] private TMP_Dropdown RightGoalkeeper;
    [SerializeField] private TMP_Dropdown RightPlayer;
    [SerializeField] private GameObject DropdownContainer;

    private AbilityActor LeftGoalkeeperAA;
    private AbilityActor LeftPlayerAA;
    private AbilityActor RightGoalkeeperAA;
    private AbilityActor RightPlayerAA;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DropdownContainer.SetActive(!DropdownContainer.activeSelf);
        }
    }
    private void OnEnable()
    {
        LeftGoalkeeper.onValueChanged.AddListener(delegate { OnLeftGoalkeeperDropdown(LeftGoalkeeper); });
        LeftPlayer.onValueChanged.AddListener(delegate { OnLeftPlayerDropdown(LeftPlayer); });
        RightGoalkeeper.onValueChanged.AddListener(delegate { OnRightGoalkeeperDropdown(RightGoalkeeper); });
        RightPlayer.onValueChanged.AddListener(delegate { OnRightPlayerDropdown(RightPlayer); });
    }
    private void OnDisable()
    {
        LeftGoalkeeper.onValueChanged.RemoveListener(delegate { OnLeftGoalkeeperDropdown(LeftGoalkeeper); });
        LeftPlayer.onValueChanged.RemoveListener(delegate { OnLeftPlayerDropdown(LeftPlayer); });
        RightGoalkeeper.onValueChanged.RemoveListener(delegate { OnRightGoalkeeperDropdown(RightGoalkeeper); });
        RightPlayer.onValueChanged.RemoveListener(delegate { OnRightPlayerDropdown(RightPlayer); });
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
}
