using UnityEngine;

public class TestPlayerAnimation : MonoBehaviour
{
    Animator animator;
    // animation IDs
    int _animIDSpeed;
    int _animIDGrounded;
    int _animIDJump;
    int _animIDFreeFall;
    int _animIDMotionSpeed;

    float _animationBlend;
    bool _hasAnimator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        AssignAnimationIDs();
        _hasAnimator = TryGetComponent(out animator);
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
            animator.SetFloat(_animIDSpeed, _animationBlend);
            animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }
    public void IdleAnimation()
    {
        if (_hasAnimator)
        {
            animator.SetBool(_animIDJump, false);
            animator.SetBool(_animIDFreeFall, false);
        }
    }
    public void JumpAnimation()
    {
        if (_hasAnimator)
            animator.SetBool(_animIDJump, true);
    }
    public void FreeFallAnimation()
    {
        if (_hasAnimator)
            animator.SetBool(_animIDFreeFall, true);
    }
    public void GroundedAnimation(bool Grounded)
    {
        if (_hasAnimator)
        {
            animator.SetBool(_animIDFreeFall, false);
            animator.SetBool(_animIDGrounded, Grounded);
        }
    }
    public void AnimationBlend(float targetSpeed, float SpeedChangeRate)
    {
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f)
            _animationBlend = 0f;
    }
}
