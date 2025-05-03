using UnityEngine;
using Fusion;

namespace Class3
{
    [RequireComponent(typeof(NetworkCharacterController))]
    public class NetworkColorChanger : NetworkBehaviour
    {
        [Networked] private Color Color { get; set; } = Color.white;

        private SkinnedMeshRenderer[] skinnedMeshRenderers;


        void Awake ()
        {
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
        }

        public override void Spawned ()
        {
            UpdateColor();
        }

        void Update ()
        {
            if (!Object.HasInputAuthority)
                return;

            if (Input.GetKeyDown(KeyCode.R))
                Rpc_SetColor(Color.red);
            else if (Input.GetKeyDown(KeyCode.G))
                Rpc_SetColor(Color.green);
            else if (Input.GetKeyDown(KeyCode.B))
                Rpc_SetColor(Color.blue);
        }

        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_SetColor (Color color)
        {
            Rpc_RelayColor(color);
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        private void Rpc_RelayColor (Color color)
        {
            Color = color;
            UpdateColor();
        }

        private void UpdateColor ()
        {
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                for (int j = 0; j < skinnedMeshRenderers[i].materials.Length; j++)
                    skinnedMeshRenderers[i].materials[j].color = Color;
            }
        }
    }
}