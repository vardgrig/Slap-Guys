using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class PlayerCamera : MonoBehaviourPunCallbacks
{
    PlayerManager player;
    StandartPlayerInput _input;

    private const float _threshold = 0.01f;
    CinemachineVirtualCamera _cinVirtualCam;

    float _cinemachineTargetYaw;
    float _cinemachineTargetPitch;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    [SerializeField] float CameraAngleOverride = 0.0f;


    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    void Start()
    {
        if (!photonView.IsMine)
            return;

        player = GetComponent<PlayerManager>();
        _input = player.Input;
        _cinVirtualCam = player.CinVirtualCam;

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _cinVirtualCam.Follow = CinemachineCameraTarget.transform;
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine)
            return;
        CameraRotation();
    }
    private void CameraRotation()
    {
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = player.IsCurrentDeviceGamepad ? Time.deltaTime : 1.0f;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
