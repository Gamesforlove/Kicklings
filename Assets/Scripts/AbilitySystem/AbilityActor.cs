using Gameplay.CharacterComponents.Player;
using Gameplay.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static ColliderEventForwarder;
using static Gameplay.Spawners.PlayersSpawner;

public enum Team { A, B }
public class AbilityActor : MonoBehaviour
{
    public PlayerType PlayerType { get; private set; }
    public Team Team { get; private set; }
    [field:SerializeField] public Transform BallPoint { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public AbilityName TestingAbility { get; set; }
    public bool PerformingAbility { get; private set; } = false;

    public event Action BallTouched;
    public event Action<Rigidbody2D> EntityTouched;

    [SerializeField] private List<AbilityConfig> abilityConfigs = new(); 
    [SerializeField] private List<ColliderEventForwarder> BodyColliders;

    private Dictionary<AbilityName, IAbility> abilities = new();
    private List<AbilityActor> players = new List<AbilityActor>();
    private BallScript ball;
    private AbilityExecutionContext context;

    void Start()
    {
        foreach (ColliderEventForwarder forwarder in BodyColliders)
        {
            forwarder.collider.excludeLayers = 1 << gameObject.layer;
        }
/*        foreach (var config in abilityConfigs)
        {
            abilities[config.AbilityName] = config.CreateAbility(this);
        }*/
        //ball = FindFirstObjectByType<BallManager>().Ball;
        ball = BallManager.Instance.Ball;
        context = new AbilityExecutionContext(this);
        context.Ball = ball;
        context.Players = players;
    }
    private void OnEnable()
    {
        foreach (var collider in BodyColliders)
        {
            collider.CollisionEntered += OnColliderTouched;
        }
    }
    private void OnDisable()
    {
        foreach (var forwarder in BodyColliders)
        {
            forwarder.CollisionEntered -= OnColliderTouched;
        }
    }
    public async void ExecuteAbility(AbilityName abilityName, AbilityExecutionContext context)
    {
        if (!abilities.ContainsKey(abilityName))
        {
            Debug.Log($"{gameObject.name} doesn't have *{abilityName.ToString()}* ability!");
            return;
        }

        Debug.Log($"{gameObject.name} begins *{abilityName.ToString()}* ability!");
        if (PerformingAbility)
        {
            Debug.Log($"{gameObject.name} is performing ability!");
            return;
        }

        PerformingAbility = true;
        if (abilities.TryGetValue(abilityName, out var ability))
        {
            await ability.Execute(context);
        }
        PerformingAbility = false;
    }
    public IEnumerator ExecuteAbilityCoroutine(AbilityName abilityName, AbilityExecutionContext context)
    {
        if (!abilities.ContainsKey(abilityName))
        {
            Debug.Log($"{gameObject.name} doesn't have *{abilityName.ToString()}* ability!");
            yield break;
        }

        Debug.Log($"{gameObject.name} begins *{abilityName.ToString()}* ability!");
        if (PerformingAbility)
        {
            Debug.Log($"{gameObject.name} is performing ability!");
            yield break;
        }

        PerformingAbility = true;
        if (abilities.TryGetValue(abilityName, out var ability))
        {
            yield return StartCoroutine(ability.ExecuteCoroutine(context));
        }
        PerformingAbility = false;
    }
    public void OnAbilityPerformed_EVENT(InputAction.CallbackContext context)
    {
        //if (context.performed) ExecuteAbility(TestingAbility, this.context);
        if (context.performed) StartCoroutine(ExecuteAbilityCoroutine(TestingAbility, this.context));
    }
    public void SetUp(Team team, PlayerType playerType)
    {
        Team = team;
        PlayerType = playerType;
    }
    public void SetUpPlayersList(List<GameObject> players)
    {
        foreach (var config in abilityConfigs)
        {
            abilities[config.AbilityName] = config.CreateAbility(this);
        }
        foreach (var player in players)
        {
            this.players.Add(player.GetComponent<AbilityActor>());
        }
    }
    private void OnColliderTouched(Collision2D collision, BodyPartCollider bodyPart)
    {
        if (collision.gameObject.CompareTag("Ball")) 
        {
            BallTouched?.Invoke();
            return;
        }
        if (bodyPart == BodyPartCollider.KickingLeg && collision.gameObject.CompareTag("Entity"))
        {
            EntityTouched?.Invoke(collision.rigidbody);
        }
    }
}
