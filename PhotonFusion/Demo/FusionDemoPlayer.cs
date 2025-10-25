using Fusion;

namespace Dissonance.Integrations.PhotonFusion.Demo
{
    public class FusionDemoPlayer
        : NetworkBehaviour
    {
        private NetworkCharacterController _cc;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.direction.Normalize();
                _cc.Move(5 * Runner.DeltaTime * data.direction);
            }
        }
    }
}
