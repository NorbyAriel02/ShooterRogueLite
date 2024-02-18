using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using Unity.FPS.Game;
using UnityEngine;

[RequireComponent(typeof(EnemyControllerV2))]
public class EnemyMobileV2 : MonoBehaviour
{
    public enum AIState
    {
        Patrol,
        Follow,
        Attack,
    }

    public Animator Animator;

    [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
    [Range(0f, 1f)]
    public float AttackStopDistanceRatio = 0.5f;

    [Tooltip("The random hit damage effects")]
    public ParticleSystem[] RandomHitSparks;

    public ParticleSystem[] OnDetectVfx;
    public AudioClip OnDetectSfx;

    [Header("Sound")] public AudioClip MovementSound;
    public MinMaxFloat PitchDistortionMovementSpeed;

    public AIState AiState { get; private set; }
    EnemyControllerV2 m_EnemyControllerV2;
    AudioSource m_AudioSource;

    const string k_AnimMoveSpeedParameter = "MoveSpeed";
    const string k_AnimAttackParameter = "Attack";
    const string k_AnimAlertedParameter = "Alerted";
    const string k_AnimOnDamagedParameter = "OnDamaged";

    void Start()
    {
        m_EnemyControllerV2 = GetComponent<EnemyControllerV2>();
        DebugUtility.HandleErrorIfNullGetComponent<EnemyControllerV2, EnemyMobile>(m_EnemyControllerV2, this,
            gameObject);

        m_EnemyControllerV2.onAttack += OnAttack;
        m_EnemyControllerV2.onDetectedTarget += OnDetectedTarget;
        m_EnemyControllerV2.onLostTarget += OnLostTarget;
        m_EnemyControllerV2.SetPathDestinationToClosestNode();
        m_EnemyControllerV2.onDamaged += OnDamaged;

        // Start patrolling
        AiState = AIState.Patrol;

        // adding a audio source to play the movement sound on it
        m_AudioSource = GetComponent<AudioSource>();
        DebugUtility.HandleErrorIfNullGetComponent<AudioSource, EnemyMobile>(m_AudioSource, this, gameObject);
        m_AudioSource.clip = MovementSound;
        m_AudioSource.Play();
    }

    void Update()
    {
        UpdateAiStateTransitions();
        UpdateCurrentAiState();

        float moveSpeed = m_EnemyControllerV2.NavMeshAgent.velocity.magnitude;

        // Update animator speed parameter
        Animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);

        // changing the pitch of the movement sound depending on the movement speed
        m_AudioSource.pitch = Mathf.Lerp(PitchDistortionMovementSpeed.Min, PitchDistortionMovementSpeed.Max,
            moveSpeed / m_EnemyControllerV2.NavMeshAgent.speed);
    }

    void UpdateAiStateTransitions()
    {
        // Handle transitions 
        switch (AiState)
        {
            case AIState.Follow:
                // Transition to attack when there is a line of sight to the target
                if (m_EnemyControllerV2.IsSeeingTarget && m_EnemyControllerV2.IsTargetInAttackRange)
                {
                    AiState = AIState.Attack;
                    m_EnemyControllerV2.SetNavDestination(transform.position);
                }

                break;
            case AIState.Attack:
                // Transition to follow when no longer a target in attack range
                if (!m_EnemyControllerV2.IsTargetInAttackRange)
                {
                    AiState = AIState.Follow;
                }

                break;
        }
    }

    void UpdateCurrentAiState()
    {
        // Handle logic 
        switch (AiState)
        {
            case AIState.Patrol:
                m_EnemyControllerV2.UpdatePathDestination();
                m_EnemyControllerV2.SetNavDestination(m_EnemyControllerV2.GetDestinationOnPath());
                break;
            case AIState.Follow:
                m_EnemyControllerV2.SetNavDestination(m_EnemyControllerV2.KnownDetectedTarget.transform.position);
                m_EnemyControllerV2.OrientTowards(m_EnemyControllerV2.KnownDetectedTarget.transform.position);
                m_EnemyControllerV2.OrientWeaponsTowards(m_EnemyControllerV2.KnownDetectedTarget.transform.position);
                break;
            case AIState.Attack:
                if (Vector3.Distance(m_EnemyControllerV2.KnownDetectedTarget.transform.position,
                        m_EnemyControllerV2.DetectionModuleV2.DetectionSourcePoint.position)
                    >= (AttackStopDistanceRatio * m_EnemyControllerV2.DetectionModuleV2.AttackRange))
                {
                    m_EnemyControllerV2.SetNavDestination(m_EnemyControllerV2.KnownDetectedTarget.transform.position);
                }
                else
                {
                    m_EnemyControllerV2.SetNavDestination(transform.position);
                }

                m_EnemyControllerV2.OrientTowards(m_EnemyControllerV2.KnownDetectedTarget.transform.position);
                m_EnemyControllerV2.TryAtack(m_EnemyControllerV2.KnownDetectedTarget.transform.position);
                break;
        }
    }

    void OnAttack()
    {
        Animator.SetTrigger(k_AnimAttackParameter);
    }

    void OnDetectedTarget()
    {
        if (AiState == AIState.Patrol)
        {
            AiState = AIState.Follow;
        }

        for (int i = 0; i < OnDetectVfx.Length; i++)
        {
            OnDetectVfx[i].Play();
        }

        if (OnDetectSfx)
        {
            AudioUtility.CreateSFX(OnDetectSfx, transform.position, AudioUtility.AudioGroups.EnemyDetection, 1f);
        }

        Animator.SetBool(k_AnimAlertedParameter, true);
    }

    void OnLostTarget()
    {
        if (AiState == AIState.Follow || AiState == AIState.Attack)
        {
            AiState = AIState.Patrol;
        }

        for (int i = 0; i < OnDetectVfx.Length; i++)
        {
            OnDetectVfx[i].Stop();
        }

        Animator.SetBool(k_AnimAlertedParameter, false);
    }

    void OnDamaged()
    {
        if (RandomHitSparks.Length > 0)
        {
            int n = Random.Range(0, RandomHitSparks.Length - 1);
            RandomHitSparks[n].Play();
        }

        Animator.SetTrigger(k_AnimOnDamagedParameter);
    }
}
