using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerPushSystem : MonoBehaviourPunCallbacks
{
    PlayerManager player;
    CharacterController controller;
    Vector3 direction;
    float pushTimeRemaining = 0.5f;
    [SerializeField] float Thrust = 5f;
    bool isBeingPushed = false;

    private void Start()
    {
        if (!photonView.IsMine)
            return;
        player = GetComponent<PlayerManager>();
        controller = player.Controller;
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (isBeingPushed && direction.magnitude > 0)
        {
            if (pushTimeRemaining > 0)
            {
                pushTimeRemaining -= Time.deltaTime;
                controller.Move(direction *  Time.deltaTime +
                         new Vector3(0.0f, -2f, 0.0f) * Time.deltaTime);
            }
            else
            {
                pushTimeRemaining = 0;
                isBeingPushed = false;
                direction = Vector3.zero;
            }
        }
    }

    public IEnumerator PushPlayer(Vector3 dir, float power, Collider hitCollider)
    {
        if (!photonView.IsMine)
            yield break;

        if (!isBeingPushed)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(),hitCollider,true);
            direction = power * dir;
            isBeingPushed = true;
            pushTimeRemaining = 0.5f;
            yield return new WaitForSecondsRealtime(2f);
            Physics.IgnoreCollision(GetComponent<Collider>(), hitCollider, false);
        }
    }
}
