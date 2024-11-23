using System;
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
        [SerializeField] SmartCollider[] ForceActiveOthers;

        float _y;
        bool _active = true;
        bool _previousActive = true;
        int _forceActiveCount;
        public int ForceActiveCount
        {
            get => _forceActiveCount;
            set
            {
                if (_forceActiveCount == value) return;
                _forceActiveCount = value;
                if (_forceActiveCount < 0) _forceActiveCount = 0;
                SetCollidersActive();
            }
        }

        void OnEnable()
        {
            SetY();
            _active = false;
            SetCollidersActive();
        }

        void OnDisable()
        {
            _active = false;
            SetCollidersActive();
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            SetY();
            _active = player.GetPosition().y >= _y;
            SetCollidersActive();
        }

        public override void OnPlayerTriggerStay(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            _active = player.GetPosition().y >= _y;
            SetCollidersActive();
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            _active = false;
            SetCollidersActive();
        }

        void SetCollidersActive()
        {
            var active = _active || _forceActiveCount > 0;
            if (_previousActive == active) return;

            _previousActive = active;
            var len = TargetColliders.Length;
            for (var i = 0; i < len; i++)
            {
                TargetColliders[i].enabled = active;
            }
            len = ForceActiveOthers.Length;
            for (var i = 0; i < len; i++)
            {
                ForceActiveOthers[i].ForceActiveCount += active ? 1 : -1;
            }
        }

        void SetY()
        {
            _y = GetComponent<Collider>().bounds.min.y;
        }
    }
}
