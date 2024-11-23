using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.SmartCollider
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(Collider))]
    public class SmartCollider : UdonSharpBehaviour
    {
        [SerializeField] Collider[] TargetColliders;

        float _y;
        bool _active = true;

        void OnEnable()
        {
            SetY();
            SetCollidersActive(false);
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            SetY();
            SetCollidersActive(player.GetPosition().y >= _y);
        }

        public override void OnPlayerTriggerStay(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            SetCollidersActive(player.GetPosition().y >= _y);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            SetCollidersActive(false);
        }

        void SetCollidersActive(bool active)
        {
            if (_active == active) return;

            _active = active;
            var len = TargetColliders.Length;
            for (var i = 0; i < len; i++)
            {
                TargetColliders[i].enabled = active;
            }
        }

        void SetY()
        {
            _y = GetComponent<Collider>().bounds.min.y;
        }
    }
}
