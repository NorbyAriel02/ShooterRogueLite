using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
public class EnemyControllerV2 : MonoBehaviour
{    
    [Header("Parameters")]
    [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
    public float SelfDestructYHeight = -20f;

    [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
    public float PathReachingRadius = 2f;

    [Tooltip("The speed at which the enemy rotates")]
    public float OrientationSpeed = 10f;

    [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
    public float DeathDuration = 0f;


    [Header("Weapons Parameters")]
    [Tooltip("Allow weapon swapping for this enemy")]
    public bool SwapToNextWeapon = false;

    [Tooltip("Time delay between a weapon swap and the next attack")]
    public float DelayAfterWeaponSwap = 0f;

    [Header("Sounds")]
    [Tooltip("Sound played when recieving damages")]
    public AudioClip DamageTick;

    [Header("VFX")]
    [Tooltip("The VFX prefab spawned when the enemy dies")]
    public GameObject DeathVfx;

    [Tooltip("The point at which the death VFX is spawned")]
    public Transform DeathVfxSpawnPoint;

    [Header("Loot")]
    [Tooltip("The object this enemy can drop when dying")]
    public GameObject LootPrefab;

    [Tooltip("The chance the object has to drop")]
    [Range(0, 1)]
    public float DropRate = 1f;

    [Header("Debug Display")]
    [Tooltip("Color of the sphere gizmo representing the path reaching range")]
    public Color PathReachingRangeColor = Color.yellow;

    [Tooltip("Color of the sphere gizmo representing the attack range")]
    public Color AttackRangeColor = Color.red;

    [Tooltip("Color of the sphere gizmo representing the detection range")]
    public Color DetectionRangeColor = Color.blue;

    public UnityAction onAttack;
    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;
    public UnityAction onDamaged;

    float m_LastTimeDamaged = float.NegativeInfinity;
        
    public PatrolPath PatrolPath { get; set; }
    public GameObject KnownDetectedTarget => DetectionModuleV2.KnownDetectedTarget;
    public bool IsTargetInAttackRange => DetectionModuleV2.IsTargetInAttackRange;
    public bool IsSeeingTarget => DetectionModuleV2.IsSeeingTarget;
    public bool HadKnownTarget => DetectionModuleV2.HadKnownTarget;
    public NavMeshAgent NavMeshAgent { get; private set; }
    public DetectionModuleV2 DetectionModuleV2 { get; private set; }

    int m_PathDestinationNodeIndex;
    ActorsManager m_ActorsManager;
    Health m_Health;
    Actor m_Actor;
    Collider[] m_SelfColliders;
    GameFlowManager m_GameFlowManager;
    bool m_WasDamagedThisFrame;
    float m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
    int m_CurrentWeaponIndex;
    WeaponController m_CurrentWeapon;
    WeaponController[] m_Weapons;
    NavigationModule m_NavigationModule;

    void Start()
    {
        m_ActorsManager = FindObjectOfType<ActorsManager>();
        DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyControllerV2>(m_ActorsManager, this);

        m_Health = GetComponent<Health>();
        DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyControllerV2>(m_Health, this, gameObject);

        m_Actor = GetComponent<Actor>();
        DebugUtility.HandleErrorIfNullGetComponent<Actor, EnemyControllerV2>(m_Actor, this, gameObject);

        NavMeshAgent = GetComponent<NavMeshAgent>();
        m_SelfColliders = GetComponentsInChildren<Collider>();

        m_GameFlowManager = FindObjectOfType<GameFlowManager>();
        DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, EnemyControllerV2>(m_GameFlowManager, this);

        // Subscribe to damage & death actions
        m_Health.OnDie += OnDie;
        m_Health.OnDamaged += OnDamaged;

        // Find and initialize all weapons
        FindAndInitializeAllWeapons();
        var weapon = GetCurrentWeapon();
        weapon.ShowWeapon(true);

        var DetectionModuleV2s = GetComponentsInChildren<DetectionModuleV2>();
        DebugUtility.HandleErrorIfNoComponentFound<DetectionModuleV2, EnemyControllerV2>(DetectionModuleV2s.Length, this,
            gameObject);
        DebugUtility.HandleWarningIfDuplicateObjects<DetectionModuleV2, EnemyControllerV2>(DetectionModuleV2s.Length,
            this, gameObject);
        // Initialize detection module
        DetectionModuleV2 = DetectionModuleV2s[0];
        DetectionModuleV2.onDetectedTarget += OnDetectedTarget;
        DetectionModuleV2.onLostTarget += OnLostTarget;
        onAttack += DetectionModuleV2.OnAttack;

        var navigationModules = GetComponentsInChildren<NavigationModule>();
        DebugUtility.HandleWarningIfDuplicateObjects<DetectionModuleV2, EnemyControllerV2>(DetectionModuleV2s.Length,
            this, gameObject);
        // Override navmesh agent data
        if (navigationModules.Length > 0)
        {
            m_NavigationModule = navigationModules[0];
            NavMeshAgent.speed = m_NavigationModule.MoveSpeed;
            NavMeshAgent.angularSpeed = m_NavigationModule.AngularSpeed;
            NavMeshAgent.acceleration = m_NavigationModule.Acceleration;
        }
    }

    void Update()
    {
        //Esto es por si el enemigo cae del plano de navegacion
        //para que se destruya, puedo eliminarlo para mejor el performans del juego
        //EnsureIsWithinLevelBounds();

        //esto entiendo detecta al player
        DetectionModuleV2.HandleTargetDetection(m_Actor, m_SelfColliders);
                
        m_WasDamagedThisFrame = false;
    }

    void EnsureIsWithinLevelBounds()
    {
        // at every frame, this tests for conditions to kill the enemy
        if (transform.position.y < SelfDestructYHeight)
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnLostTarget()
    {
        onLostTarget.Invoke();
    }

    void OnDetectedTarget()
    {
        onDetectedTarget.Invoke();
    }

    public void OrientTowards(Vector3 lookPosition)
    {
        Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
        if (lookDirection.sqrMagnitude != 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
        }
    }

    bool IsPathValid()
    {
        return PatrolPath && PatrolPath.PathNodes.Count > 0;
    }

    public void ResetPathDestination()
    {
        m_PathDestinationNodeIndex = 0;
    }

    public void SetPathDestinationToClosestNode()
    {
        if (IsPathValid())
        {
            int closestPathNodeIndex = 0;
            for (int i = 0; i < PatrolPath.PathNodes.Count; i++)
            {
                float distanceToPathNode = PatrolPath.GetDistanceToNode(transform.position, i);
                if (distanceToPathNode < PatrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
                {
                    closestPathNodeIndex = i;
                }
            }

            m_PathDestinationNodeIndex = closestPathNodeIndex;
        }
        else
        {
            m_PathDestinationNodeIndex = 0;
        }
    }

    public Vector3 GetDestinationOnPath()
    {
        if (IsPathValid())
        {
            return PatrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
        }
        else
        {
            return transform.position;
        }
    }

    public void SetNavDestination(Vector3 destination)
    {
        if (NavMeshAgent)
        {
            NavMeshAgent.SetDestination(destination);
        }
    }

    public void UpdatePathDestination(bool inverseOrder = false)
    {
        if (IsPathValid())
        {
            // Check if reached the path destination
            if ((transform.position - GetDestinationOnPath()).magnitude <= PathReachingRadius)
            {
                // increment path destination index
                m_PathDestinationNodeIndex =
                    inverseOrder ? (m_PathDestinationNodeIndex - 1) : (m_PathDestinationNodeIndex + 1);
                if (m_PathDestinationNodeIndex < 0)
                {
                    m_PathDestinationNodeIndex += PatrolPath.PathNodes.Count;
                }

                if (m_PathDestinationNodeIndex >= PatrolPath.PathNodes.Count)
                {
                    m_PathDestinationNodeIndex -= PatrolPath.PathNodes.Count;
                }
            }
        }
    }

    void OnDamaged(float damage, GameObject damageSource)
    {
        // test if the damage source is the player
        if (damageSource && !damageSource.GetComponent<EnemyControllerV2>())
        {
            // pursue the player
            DetectionModuleV2.OnDamaged(damageSource);

            onDamaged?.Invoke();
            m_LastTimeDamaged = Time.time;

            // play the damage tick sound
            if (DamageTick && !m_WasDamagedThisFrame)
                AudioUtility.CreateSFX(DamageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

            m_WasDamagedThisFrame = true;
        }
    }

    void OnDie()
    {
        // spawn a particle system when dying
        var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
        Destroy(vfx, 5f);

        // loot an object
        if (TryDropItem())
        {
            Instantiate(LootPrefab, transform.position, Quaternion.identity);
        }

        // this will call the OnDestroy function
        Destroy(gameObject, DeathDuration);
    }

    void OnDrawGizmosSelected()
    {
        // Path reaching range
        Gizmos.color = PathReachingRangeColor;
        Gizmos.DrawWireSphere(transform.position, PathReachingRadius);

        if (DetectionModuleV2 != null)
        {
            // Detection range
            Gizmos.color = DetectionRangeColor;
            Gizmos.DrawWireSphere(transform.position, DetectionModuleV2.DetectionRange);

            // Attack range
            Gizmos.color = AttackRangeColor;
            Gizmos.DrawWireSphere(transform.position, DetectionModuleV2.AttackRange);
        }
    }

    public void OrientWeaponsTowards(Vector3 lookPosition)
    {
        for (int i = 0; i < m_Weapons.Length; i++)
        {
            // orient weapon towards player
            Vector3 weaponForward = (lookPosition - m_Weapons[i].WeaponRoot.transform.position).normalized;
            m_Weapons[i].transform.forward = weaponForward;
        }
    }

    public bool TryAtack(Vector3 enemyPosition)
    {
        if (m_GameFlowManager.GameIsEnding)
            return false;

        OrientWeaponsTowards(enemyPosition);

        if ((m_LastTimeWeaponSwapped + DelayAfterWeaponSwap) >= Time.time)
            return false;

        // Shoot the weapon
        bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

        if (didFire && onAttack != null)
        {
            onAttack.Invoke();

            if (SwapToNextWeapon && m_Weapons.Length > 1)
            {
                int nextWeaponIndex = (m_CurrentWeaponIndex + 1) % m_Weapons.Length;
                SetCurrentWeapon(nextWeaponIndex);
            }
        }

        return didFire;
    }

    public bool TryDropItem()
    {
        if (DropRate == 0 || LootPrefab == null)
            return false;
        else if (DropRate == 1)
            return true;
        else
            return (Random.value <= DropRate);
    }

    void FindAndInitializeAllWeapons()
    {
        // Check if we already found and initialized the weapons
        if (m_Weapons == null)
        {
            m_Weapons = GetComponentsInChildren<WeaponController>();
            DebugUtility.HandleErrorIfNoComponentFound<WeaponController, EnemyControllerV2>(m_Weapons.Length, this,
                gameObject);

            for (int i = 0; i < m_Weapons.Length; i++)
            {
                m_Weapons[i].Owner = gameObject;
            }
        }
    }

    public WeaponController GetCurrentWeapon()
    {
        FindAndInitializeAllWeapons();
        // Check if no weapon is currently selected
        if (m_CurrentWeapon == null)
        {
            // Set the first weapon of the weapons list as the current weapon
            SetCurrentWeapon(0);
        }

        DebugUtility.HandleErrorIfNullGetComponent<WeaponController, EnemyControllerV2>(m_CurrentWeapon, this,
            gameObject);

        return m_CurrentWeapon;
    }

    void SetCurrentWeapon(int index)
    {
        m_CurrentWeaponIndex = index;
        m_CurrentWeapon = m_Weapons[m_CurrentWeaponIndex];
        if (SwapToNextWeapon)
        {
            m_LastTimeWeaponSwapped = Time.time;
        }
        else
        {
            m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
        }
    }
}