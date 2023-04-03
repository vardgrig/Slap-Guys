using Photon.Pun;
using UnityEngine;

public class PlayerAnimation : MonoBehaviourPunCallbacks
{
    PlayerManager player;
    CharacterController _controller;
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;


    Animator _animator;
    // animation IDs
    int _animIDSpeed;
    int _animIDGrounded;
    int _animIDJump;
    int _animIDFreeFall;
    int _animIDMotionSpeed;

    float _animationBlend;
    bool _hasAnimator;

    private void Start()
    {
        if (!photonView.IsMine)
            return;

        player = GetComponent<PlayerManager>();
        _controller = player.Controller;
        _hasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();
    }
    private void Update()
    {
        if (!photonView.IsMine)
            return;

        _hasAnimator = TryGetComponent(out _animator);

    }
    void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    public void MovementAnimation(float inputMagnitude)
    {
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }
    public void IdleAnimation()
    {
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);
        }
    }
    public void JumpAnimation() 
    {
        if (_hasAnimator)
            _animator.SetBool(_animIDJump, true);
    }
    public void FreeFallAnimation()
    {
        if (_hasAnimator)
            _animator.SetBool(_animIDFreeFall, true);
    }
    public void GroundedAnimation(bool Grounded)
    {
        if (_hasAnimator)
            _animator.SetBool(_animIDGrounded, Grounded);
    }
    public void AnimationBlend(float targetSpeed, float SpeedChangeRate)
    {
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f)
            _animationBlend = 0f;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!photonView.IsMine)
            return;
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (!photonView.IsMine)
            return;
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
}

