using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{

    public Transform armsTransform;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [Header("Movement")]
    public float groundMaxSpeed;
    public float sprintSpeedModifier = 1.5f;
    public float sprintStaminaCost;
    public float speedLerpGround;
    public float airMaxSpeed;
    public float accelerationAir;
    public float jumpForce;
    public float gravity = 20f;
    
    [Header("Look")]
    public float rotationSpeed = 200f;
    public float minVerticalAngle = -89f;
    public float maxVerticalAngle = 89f;

    CharacterController cc;
    PlayerInput input;
    PlayerArms arms;

    bool isGrounded;
    Vector3 groundNormal;
    float armsVerticalAngle = 0f;
    Vector3 currentVelocity;

    TriggerArea interactTrigger;
    List<IInteractable> interactables;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        arms = GetComponent<PlayerArms>();
        interactables = new List<IInteractable>();

        interactTrigger = GetComponentInChildren<TriggerArea>();
        interactTrigger.TriggerEnter += OnInteractTriggerEnter;
        interactTrigger.TriggerExit += OnInteractTriggerExit;
    }

    void Update()
    {
        HandleRotation();
        GroundCheck();
        HandleInteractables();
    }

    void FixedUpdate() 
    {
        HandleMovement();
    }

    void HandleRotation()
    {
        if (!PlayerManager.Instance.IsMovingEnabled) return;

        // Horizontal
        var rotH = new Vector3(0f, input.LookHorizontal * rotationSpeed, 0f);
        transform.Rotate(rotH, Space.Self);

        // Vertical
        armsVerticalAngle += input.LookVertical * rotationSpeed;
        armsVerticalAngle = Mathf.Clamp(armsVerticalAngle, minVerticalAngle, maxVerticalAngle);

        armsTransform.transform.localEulerAngles = new Vector3(armsVerticalAngle, 0, 0);
    }

    void GroundCheck()
    {
        isGrounded = false;
        groundNormal = Vector3.up;

        // Use capsule of CharacterController
        var innerHeight = (cc.height - 2 * cc.radius) / 2;
        var point1 = transform.position + Vector3.up * innerHeight;
        var point2 = transform.position + Vector3.down * innerHeight;
        var b = Physics.CapsuleCast(point1, point2, cc.radius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);

        if (b)
        {
            groundNormal = hit.normal;
            if (Vector3.Dot(hit.normal, transform.up) > 0f && IsSlope(groundNormal))
            {
                isGrounded = true;
                // // Snap to ground
                // cc.Move(Vector3.down * hit.distance);
            }
        }
    }

    bool IsSlope(Vector3 groundNormal)
    {
        var angle = Vector3.Angle(transform.up, groundNormal);
        return angle <= cc.slopeLimit;
    }

    void HandleMovement()
    {
        if (!PlayerManager.Instance.IsMovingEnabled) return;

        Vector3 movement = transform.TransformVector(input.Movement);
        float speedModifier = input.Sprint? sprintSpeedModifier : 1f;

        if (isGrounded)
        {
            // Ground Move
            var directionSide = Vector3.Cross(movement, transform.up);
            var directionSlope = Vector3.Cross(groundNormal, directionSide);
            var targetVelocity = directionSlope.normalized * groundMaxSpeed * speedModifier;

            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, speedLerpGround * Time.deltaTime);

            // Jump
            if (input.Jump)
            {
                currentVelocity += Vector3.up * jumpForce;
                input.Jump = false;
                isGrounded = false;
            }
        }
        else
        {
            // Air move
            currentVelocity += movement * accelerationAir * Time.deltaTime;
            // Clamp horizontal speed in air
            var verticalVelocity = currentVelocity.y;
            var horizontalVelocity = Vector3.ProjectOnPlane(currentVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, airMaxSpeed * speedModifier);
            currentVelocity = horizontalVelocity + Vector3.up * verticalVelocity;

            currentVelocity += Vector3.down * gravity * Time.deltaTime;
        }

        cc.Move(currentVelocity * Time.deltaTime);

    }

    public void TakeDamage(float value)
    {
        Debug.Log("Player Take Damage");
    }

    public void Die()
    {
        Debug.Log("Player die");
    }

    void HandleInteractables()
    {
        if (interactables.Count == 0) return;
        int closest = 0;
        float maxDot = 0;
        for (int i = interactables.Count - 1; i >= 0; i--)
        {
            if (interactables[i] is MonoBehaviour
                && ((MonoBehaviour) interactables[i]) == null)
            {
                interactables.RemoveAt(i);
                continue;
            }
            var dis = interactables[i].gameObject.transform.position - transform.position;
            dis = Vector3.ProjectOnPlane(dis, transform.up);
            var dot = Vector2.Dot(transform.forward, dis);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = i;
            }
            if (interactables[i].IsHintShowing)
                interactables[i].HideHint();
        }
        if (interactables.Count == 0) return;
        interactables[closest].ShowHint();
    }

    void OnInteractTriggerEnter(Collider other)
    {
        // Debug.Log("OnInteractTriggerEnter");
        var interactable = other.GetComponent<IInteractable>();
        if (interactable == null)
            interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && !interactables.Contains(interactable))
        {
            interactables.Add(interactable);
        }
    }

    void OnInteractTriggerExit(Collider other)
    {
        // Debug.Log("OnInteractTriggerExit");
        var interactable = other.GetComponent<IInteractable>();
        if (interactable == null)
            interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && interactables.Contains(interactable))
        {
            if (interactable.IsHintShowing)
                interactable.HideHint();
            interactables.Remove(interactable);
        }
    }

}
