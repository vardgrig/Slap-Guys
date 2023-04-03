using System.Collections;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    StandartPlayerInput input;
    Rigidbody rb;
    TestPlayerAnimation anim;

    [SerializeField] float moveSpeed;
    float _speed;
    [SerializeField] float jumpForce;

    [SerializeField] float groundedOffset;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] float gravity;
    public bool canMove = true;
    bool isStuned = false;
    bool slide;
    float pushForce;
    Vector3 pushDir;
    bool isGrounded;

    private Vector3 _movementDirection;
    private float _targetRotation;
    private float _rotationVelocity;

    [SerializeField] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _speedChangeRate = 10.0f;
    [SerializeField] private float _verticalVelocity = 0.0f;
    float FallTimeout = 0.35f;
    float _fallTimeoutDelta;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<StandartPlayerInput>();
        anim = GetComponent<TestPlayerAnimation>();
    }

    void FixedUpdate()
    {
        JumpAndGravity();
        if (canMove)
            Move();
    }

    private void Move()
    {
        float targetSpeed = moveSpeed;

        if (input.move == Vector2.zero)
            targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.fixedDeltaTime * _speedChangeRate);

            _speed = Mathf.Round(moveSpeed * 1000f) / 1000f;

        }
        else
        {
            _speed = targetSpeed;
        }

        anim.AnimationBlend(targetSpeed, _speedChangeRate);
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        if (input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        anim.MovementAnimation(inputMagnitude);

        _movementDirection = targetDirection.normalized * (_speed * Time.fixedDeltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + _movementDirection);
    }

    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            _fallTimeoutDelta = FallTimeout;

            anim.IdleAnimation();

            if (rb.velocity.y < 0.0f)
                rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);

            if (input.jump)
            {
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(jumpForce * gravity), rb.velocity.z);

                anim.JumpAnimation();
            }
            input.jump = false;
        }
        else
        {
            if (_fallTimeoutDelta >= 0.0f)
                _fallTimeoutDelta -= Time.fixedDeltaTime;
            else
                anim.FreeFallAnimation();

        }
    }

    void Update()
    {
        IsGrounded();
    }

    void IsGrounded()
    {
        Vector3 spherePosition = new(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, 0.25f, GroundLayer, QueryTriggerInteraction.Ignore);

        anim.GroundedAnimation(isGrounded);
    }
}