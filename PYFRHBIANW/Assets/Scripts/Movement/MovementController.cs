using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementConfig config;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider physicsCollider;

    [SerializeField] private AudioClip jumpSFX;

    [field: SerializeField] public MovementContext CurrentContext { get; private set; }

    private void Awake()
    {
        CurrentContext = new MovementContext();
    }

    public struct MovementContext
    {
        public bool isGrounded;
        public bool isStableGrounded;
        public Vector3 interpolatedNormal;
        public Vector3 normal;

        public float groundedTime;
        public float airborneTime;
    }

    private float ungroundTime;

    public void UpdateMovement(Vector2 input, float deltaTime, Transform cameraTransform)
    {
        ungroundTime -= Time.deltaTime;

        input.Normalize();

        var context = new MovementContext();
        context.groundedTime = CurrentContext.groundedTime + deltaTime;
        context.airborneTime = CurrentContext.airborneTime + deltaTime;

        // Grounded check
        GroundedCheck(ref context);

        // Snap to ground
        if (ungroundTime <= 0f)
        {
            SnapToGround(ref context);
        }

        // Movement
        CalculateMovement(input, deltaTime, cameraTransform, context);

        CurrentContext = context;
    }

    public void TryStartJump()
    {
        if (ungroundTime > 0f)
        {
            return;
        }

        if (CurrentContext.isStableGrounded || CurrentContext.airborneTime < config.CoyoteTime)
        {
            rb.velocity = new Vector3(rb.velocity.x, config.JumpVelocity, rb.velocity.z);
            SFXOneshotPlayer.Instance.PlaySFXOneshot(jumpSFX);
            ungroundTime = 0.15f;
        }
    }

    private void SnapToGround(ref MovementContext context)
    {
        rb.AddForce(-context.normal * config.DefaultGravity);
        var isMovingAwayFromGround = Vector3.Dot(rb.velocity, context.normal) > 0f;
        if (context.isGrounded && isMovingAwayFromGround)
        {
            // Snap to ground
            var snapToGround = Vector3.ProjectOnPlane(rb.velocity, context.normal).normalized * rb.velocity.magnitude;
            rb.velocity = snapToGround;
        }
    }

    private void CalculateMovement(Vector2 input, float deltaTime, Transform cameraTransform, MovementContext context)
    {
        // project input against normal
        var moveDir = Vector3.ProjectOnPlane(
            cameraTransform.forward * input.y + cameraTransform.right * input.x,
            context.normal);

        // calculate acceleration
        var acceleration = context.isGrounded
            ? config.GroundAcceleration
            : config.AirAcceleration;

        // calculate friction
        var isBunnyHopInterval = context.groundedTime < config.BunnyHopInterval;
        var friction = context.isGrounded && !isBunnyHopInterval
            ? config.GroundFriction
            : config.AirFriction;

        // calculate max speed
        var maxSpeed = context.isGrounded
            ? config.MaxGroundSpeed
            : config.MaxAirSpeed;

        // calculate velocity
        var currentVel = Vector3.ProjectOnPlane(rb.velocity, context.normal);
        var targetVel = Vector3.ProjectOnPlane(moveDir, context.normal).normalized * maxSpeed;

        var newVel = currentVel;

        // Air strafing
        if (!context.isGrounded && targetVel != Vector3.zero)
        {
            currentVel = targetVel.normalized * currentVel.magnitude;
            rb.velocity = new Vector3(currentVel.x, rb.velocity.y, currentVel.z);
        }

        // Increasing acceleration
        if (Vector3.Dot(targetVel, currentVel) >= 0)
        {
            newVel = Vector3.MoveTowards(currentVel, targetVel,
                Mathf.MoveTowards(acceleration, 0, currentVel.magnitude > maxSpeed ? friction : 0) * deltaTime);
        }
        // Decreasing
        else
        {
            newVel = Vector3.MoveTowards(currentVel, targetVel, (friction + acceleration) * deltaTime);
        }

        var step = newVel - currentVel;

        // apply friction

        // Movement velocity
        newVel = rb.velocity + step;

        rb.velocity = newVel;
    }

    private void GroundedCheck(ref MovementContext context)
    {
        // spherecast
        var spherecastOrigin = physicsCollider.bounds.center + Vector3.up * config.SpherecastOffset;
        var spherecastDirection = Vector3.down;
        var spherecastDistance = config.SpherecastDistance;
        var spherecastRadius = config.SpherecastRadius;
        var spherecastLayerMask = config.GroundedLayerMask;
        var didHit = Physics.SphereCast(
            spherecastOrigin,
            spherecastRadius,
            spherecastDirection,
            out var hitInfo,
            spherecastDistance,
            spherecastLayerMask,
            QueryTriggerInteraction.Ignore);

        if (didHit)
        {
            context.isGrounded = true;
            context.interpolatedNormal = hitInfo.normal;
            context.normal = GetRawNormal(hitInfo.point, hitInfo.distance * Vector3.down + spherecastOrigin);

            // check angle
            var angle = Vector3.Angle(Vector3.up, context.interpolatedNormal);
            context.isStableGrounded = angle <= config.MaxStableGroundedAngle;
        }
        else
        {
            context.isGrounded = false;
            context.isStableGrounded = false;
            context.interpolatedNormal = Vector3.up;
            context.normal = Vector3.up;
        }

        if (context.isGrounded)
        {
            Debug.DrawLine(hitInfo.point, hitInfo.point + context.normal, Color.blue, 5f);
            Debug.DrawLine(hitInfo.point, hitInfo.point + context.interpolatedNormal, Color.red, 5f);
        }

        if (!context.isGrounded)
        {
            context.groundedTime = 0;
        }
        else
        {
            context.airborneTime = 0;
        }
    }

    private Vector2 GetRawNormal(Vector3 hitPos, Vector3 collHitCenter)
    {
        var vecToHitPos = hitPos - collHitCenter;
        var didHit = Physics.Raycast(collHitCenter - vecToHitPos.normalized * 0.5f,
            vecToHitPos.normalized,
            out var hitInfo,
            vecToHitPos.magnitude + 0.6f,
            config.GroundedLayerMask,
            QueryTriggerInteraction.Ignore);

        if (didHit)
        {
            Debug.DrawLine(collHitCenter, collHitCenter + vecToHitPos.normalized * (vecToHitPos.magnitude + 0.5f),
                Color.magenta,
                5f);
            return hitInfo.normal;
        }

        Debug.DrawLine(collHitCenter, collHitCenter + vecToHitPos.normalized * (vecToHitPos.magnitude + 0.5f),
            Color.green,
            5f);
        return Vector3.up;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var spherecastOrigin = physicsCollider.bounds.center + Vector3.up * config.SpherecastOffset;
        Gizmos.DrawWireSphere(spherecastOrigin, config.SpherecastRadius);

        Gizmos.DrawWireSphere(spherecastOrigin + Vector3.down * config.SpherecastDistance, config.SpherecastRadius);

        Gizmos.DrawLine(spherecastOrigin, spherecastOrigin + Vector3.down * config.SpherecastDistance);
    }
}