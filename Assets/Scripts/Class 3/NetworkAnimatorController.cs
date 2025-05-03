using UnityEngine;
using Fusion;

namespace Class3
{
    [RequireComponent(typeof(NetworkPlayerController), typeof(NetworkMecanimAnimator))]
    public class NetworkAnimatorController : NetworkBehaviour
    {
        private NetworkPlayerController networkPlayerController;
        private NetworkMecanimAnimator networkMecanicAnimator;

        private const string IsMovingBoolName = "Is Moving";
        private const string TauntTriggerName = "Taunt";


        void Awake ()
        {
            networkMecanicAnimator = GetComponent<NetworkMecanimAnimator>();
            networkPlayerController = GetComponent<NetworkPlayerController>();
        }

        public override void Spawned ()
        {
            networkPlayerController.OnMovementStarted += OnMovementStart;
            networkPlayerController.OnMovementStopped += OnMovementStop;
        }

        public override void Despawned (NetworkRunner runner, bool hasState)
        {
            networkPlayerController.OnMovementStarted -= OnMovementStart;
            networkPlayerController.OnMovementStopped -= OnMovementStop;
        }

        void Update ()
        {
            if (!Object.HasInputAuthority)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                Rpc_PlayTaunt(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                Rpc_PlayTaunt(2);
        }

        private void OnMovementStart ()
        {
            networkMecanicAnimator.Animator.SetBool(IsMovingBoolName, true);
        }

        private void OnMovementStop ()
        {
            networkMecanicAnimator.Animator.SetBool(IsMovingBoolName, false);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_PlayTaunt (int tauntNumber)
        {
            Rpc_RelayTaunt(tauntNumber);
        } 
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        private void Rpc_RelayTaunt (int tauntNumber)
        {
            networkMecanicAnimator.SetTrigger($"{TauntTriggerName} {tauntNumber}", passThroughOnInputAuthority: true);
        } 
    }
}