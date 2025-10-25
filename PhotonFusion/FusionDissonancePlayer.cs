using Dissonance;
using Fusion;
using UnityEngine;

namespace FearFactory.Voice
{
    public class FusionDissonanceTrackedPlayer : NetworkBehaviour, IDissonancePlayer
    {
        [SerializeField] DissonanceComms comms;
        [SerializeField] Transform tracked;

        [Networked] NetworkString<_64> NetPlayerId { get; set; }

        bool registered;
        bool spawnedOk;
        string idCache;

        public string PlayerId => spawnedOk ? NetPlayerId.ToString() : idCache;
        public Vector3 Position => tracked ? tracked.position : transform.position;
        public Quaternion Rotation => tracked ? tracked.rotation : transform.rotation;
        public NetworkPlayerType Type => Object && Object.HasInputAuthority ? NetworkPlayerType.Local : NetworkPlayerType.Remote;
        public bool IsTracking => registered;

        bool Online => Runner && Runner.IsRunning && spawnedOk;

        public override void Spawned()
        {
            spawnedOk = true;
            if (!comms) comms = FindObjectOfType<DissonanceComms>();
            if (!tracked) tracked = transform;
            TrySetLocalPlayerId();
        }

        void Update()
        {
            if (!Online) { StopIfRegistered(); return; }
            if (!comms) comms = FindObjectOfType<DissonanceComms>();
            if (!comms) { StopIfRegistered(); return; }
            if (Object && Object.HasInputAuthority) TrySetLocalPlayerId();
            TryRegisterOrUnregister();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            spawnedOk = false;
            StopIfRegistered();
        }

        void OnDisable()
        {
            spawnedOk = false;
            StopIfRegistered();
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        void RpcSetPlayerId(string id)
        {
            NetPlayerId = id;
            idCache = id;
        }

        void TrySetLocalPlayerId()
        {
            if (!spawnedOk || !comms) return;
            if (string.IsNullOrEmpty(NetPlayerId.ToString()))
            {
                var id = comms.LocalPlayerName;
                if (!string.IsNullOrEmpty(id)) RpcSetPlayerId(id);
            }
        }

        void TryRegisterOrUnregister()
        {
            if (!spawnedOk || !comms) { StopIfRegistered(); return; }
            var haveId = !string.IsNullOrEmpty(NetPlayerId.ToString());
            if (haveId && !registered)
            {
                comms.TrackPlayerPosition(this);
                registered = true;
            }
            else if (!haveId && registered)
            {
                StopIfRegistered();
            }
        }

        void StopIfRegistered()
        {
            if (registered && comms)
            {
                comms.StopTracking(this);
                registered = false;
            }
        }
    }
}
