using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CoopCamera coopCamera;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;
    [SerializeField] private Transform fpCamera;
    [SerializeField] private MovementController movementController;
    [SerializeField] private CombatEntity combatEntity;
    [SerializeField] private Transform model;
    [SerializeField] private ShootingScript shootingScript;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CollisionListener collisionListener;
    [SerializeField] private PhysicMaterial def;
    [SerializeField] private PhysicMaterial ragdoll;
    [SerializeField] private Collider coll;

    [SerializeField] private ThirdPersonInputProvider inputProvider;

    [SerializeField] private VFXScript deathVFX;

    [SerializeField] private float ragdollDuration;
    [SerializeField] private float impulseThreshold;


    private Vector2 lookInput;
    private Vector2 moveInput;

    private int playerNumber;

    private float kb;

    public enum PlayerState
    {
        Default,
        Ragdoll,
        Dead
    }

    private float deadTimer = 0f;
    private float ragdollTimer = 0f;

    private PlayerState state = PlayerState.Default;

    public void Setup(int playerNumber)
    {
        coopCamera.Initialize(playerNumber);
        combatEntity.Initialize(playerNumber);
        shootingScript.Initialize(playerNumber);
        this.playerNumber = playerNumber;
    }

    private void Awake()
    {
        inputProvider.OnLook.AddListener(UpdateLookInput);
        inputProvider.OnMove.AddListener(UpdateMoveInput);
        inputProvider.OnJump.AddListener(TryJump);
        combatEntity.OnDeath += OnDeath;
        combatEntity.OnHit += OnHit;
    }

    private void OnDestroy()
    {
        inputProvider.OnLook.RemoveListener(UpdateLookInput);
        inputProvider.OnMove.RemoveListener(UpdateMoveInput);
        inputProvider.OnJump.RemoveListener(TryJump);
        combatEntity.OnDeath -= OnDeath;
        combatEntity.OnHit -= OnHit;
    }

    private void OnHit(AttackInfo obj)
    {
        SwitchState(PlayerState.Ragdoll);
        Debug.Log(obj.knockback);
        rb.velocity = obj.direction * obj.knockback;
        kb = obj.knockback;
    }

    private void OnDeath(AttackInfo obj)
    {
        SwitchState(PlayerState.Dead);
    }

    private void TryJump(InputAction.CallbackContext context)
    {
        Debug.Log("JUMP");
        movementController.TryStartJump();
    }

    private void UpdateLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void UpdateMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }


    private void Update()
    {
        // third person camera
        thirdPersonCamera.UpdateCamera(lookInput, Time.deltaTime);
        var tangent = Vector3.Cross(Vector3.up, fpCamera.forward);
        model.transform.rotation = Quaternion.LookRotation(Vector3.Cross(tangent, Vector3.up), Vector3.up);

        if (state == PlayerState.Dead)
        {
            deadTimer -= Time.deltaTime;
            if (deadTimer <= 0f)
            {
                SwitchState(PlayerState.Default);
            }
        }

        if (state == PlayerState.Ragdoll)
        {
            rb.velocity = rb.velocity.normalized * kb;
            thirdPersonCamera.UpdateCamera(Vector2.right, Time.deltaTime * 200000f);
        }

        ragdollTimer -= Time.deltaTime;
    }

    private void SwitchState(PlayerState newState)
    {
        if (newState == state)
        {
            return;
        }

        switch (newState)
        {
            case PlayerState.Default:
                coll.material = def;
                model.gameObject.GetComponent<MeshRenderer>().enabled = true;
                shootingScript.gameObject.SetActive(true);
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.transform.rotation = Quaternion.identity;
                rb.transform.position = new Vector3(Random.Range(-25f, 25f), 25f, Random.Range(-25f, 25f));
                break;
            case PlayerState.Ragdoll:
                coll.material = ragdoll;
                shootingScript.gameObject.SetActive(false);
                rb.constraints = RigidbodyConstraints.None;
                rb.angularVelocity = Random.insideUnitSphere * 1000f;
                collisionListener.OnCollisionEnterEvent += OnRagdollCollision;
                ragdollTimer = ragdollDuration;
                break;
            case PlayerState.Dead:
                coll.material = def;
                shootingScript.gameObject.SetActive(false);

                collisionListener.OnCollisionEnterEvent -= OnRagdollCollision;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                deadTimer = 4f;
                Instantiate(deathVFX, model.transform.position, Quaternion.identity);
                ScoreUI.Instance.IncrementDeath(playerNumber);
                model.gameObject.GetComponent<MeshRenderer>().enabled = false;
                break;
            default:
                break;
        }

        state = newState;
    }

    private void OnRagdollCollision(Collision obj)
    {
        if (ragdollTimer <= 0f || obj.impulse.magnitude > impulseThreshold)
        {
            // SwitchState(PlayerState.Dead);
        }
    }

    private void FixedUpdate()
    {
        if (state == PlayerState.Default)
        {
            // movement
            movementController.UpdateMovement(moveInput, Time.fixedDeltaTime, fpCamera.transform);
        }
        else if (state == PlayerState.Ragdoll)
        {
            rb.AddForce(Physics.gravity * rb.mass);
        }
    }
}