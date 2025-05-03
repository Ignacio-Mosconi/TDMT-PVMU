using System;
using UnityEngine;
using Fusion;

namespace Class3
{
    [RequireComponent(typeof(NetworkCharacterController))]
    public class NetworkPlayerController : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTarget;

        private NetworkCharacterController networkCharacterController;
        private CameraTracker cameraTracker;
        private bool isMoving;

        public event Action OnMovementStarted;
        public event Action OnMovementStopped;


        void Awake ()
        {
            networkCharacterController = GetComponent<NetworkCharacterController>();
            cameraTracker = FindAnyObjectByType<CameraTracker>();
        }

        public override void Spawned ()
        {
            if (!Object.HasInputAuthority)
                return;

            NetworkManager.Instance.LocalPlayer = this;
            cameraTracker.SetFollowTarget(cameraTarget);
        }

        public override void Despawned (NetworkRunner runner, bool hasState)
        {
            if (!Object.HasInputAuthority)
                return;

            NetworkManager.Instance.LocalPlayer = null;
            cameraTracker.SetFollowTarget(null);
        }

        public override void FixedUpdateNetwork ()
        {
            if (!GetInput(out NetworkInputData networkInput))
                return;

            Vector3 moveDirection = GetMoveDirection(networkInput);
            bool wasMoving = isMoving;

            isMoving = moveDirection != Vector3.zero;

            if (isMoving)
            {
                networkCharacterController.Move(moveDirection);

                if (!wasMoving)
                    OnMovementStarted?.Invoke();
            }
            else if (wasMoving)
            {
                OnMovementStopped?.Invoke();
            }
        }


        private Vector3 GetMoveDirection (NetworkInputData networkInput)
        {
            Vector3 moveDirection = Vector3.zero;

            Vector3 forwardDirection = networkInput.LookDirection;
            Vector3 rightDirection = Quaternion.AngleAxis(90f, Vector3.up) * networkInput.LookDirection;

            if (networkInput.IsInputDown(NetworkInputType.MoveForward))
                moveDirection += forwardDirection;

            if (networkInput.IsInputDown(NetworkInputType.MoveBackwards))
                moveDirection -= forwardDirection;

            if (networkInput.IsInputDown(NetworkInputType.MoveLeft))
                moveDirection -= rightDirection;

            if (networkInput.IsInputDown(NetworkInputType.MoveRight))
                moveDirection += rightDirection;

            return moveDirection;
        }

        public Vector3 GetNormalizedLookDirection ()
        {
            Vector3 lookDirection = cameraTracker.transform.forward;

            lookDirection.y = 0f;

            return lookDirection.normalized;
        }
    }
}