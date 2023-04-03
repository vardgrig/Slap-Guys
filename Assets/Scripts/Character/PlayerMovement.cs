using System.Collections;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    PlayerManager player;
    PlayerAnimation playerAnim;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] float _moveSpeed = 11f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField] float jumpHeight = 3.5f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] float gravity = -30f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] float JumpTimeout = 0f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    [SerializeField] float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] float GroundedRadius = 0.22f;

    [Tooltip("What layers the character uses as ground")]
    [SerializeField] LayerMask GroundLayers;

    CharacterController _controller;
    StandartPlayerInput _input;
    Camera _mainCamera;

    float _speed;

    float _targetRotation = 0.0f;
    public float _verticalVelocity;
    float _terminalVelocity = 53.0f;
    float _rotationVelocity;

    float _jumpTimeoutDelta;
    float _fallTimeoutDelta;

    public float _trampolinePower = 0;
    public Vector3 _colissionTargetDirection;
    public bool IsLocal;


    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        playerAnim = GetComponent<PlayerAnimation>();
    }
    void Start()
    {
        if (!photonView.IsMine)
            return;

        _input = player.Input;
        _controller = player.Controller;
        _mainCamera = player.MainCamera;

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (!photonView.IsMine)
            return;

        JumpAndGravity();
        GroundedCheck();

        if (_controller.enabled)
            Move();
    }
    public void StopPlayer()
    {
        if (!photonView.IsMine)
            return;

        _controller.enabled = false;
        Debug.Log("You finished");
    }
    private void Move()
    {
        float targetSpeed = _moveSpeed;

        if (_input.move == Vector2.zero)
            targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        playerAnim.AnimationBlend(targetSpeed, SpeedChangeRate);
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero && player.CanMove)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        if (player.CanMove)
        {
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            playerAnim.MovementAnimation(inputMagnitude);
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        else
        {
            _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

    }
    private void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            playerAnim.IdleAnimation();

            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;

            if (_input.jump && _trampolinePower == 0)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                playerAnim.JumpAnimation();
            }
            else if (_trampolinePower > 0 && !_input.jump)
                _verticalVelocity = Mathf.Sqrt(_trampolinePower * -2f * gravity);

            if (_jumpTimeoutDelta >= 0.0f)
                _jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
                _fallTimeoutDelta -= Time.deltaTime;
            else
                playerAnim.FreeFallAnimation();

            _input.jump = false;
            _trampolinePower = 0;
        }

        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += gravity * Time.deltaTime;
    }
    private void GroundedCheck()
    {
        Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        playerAnim.GroundedAnimation(Grounded);
    }
}