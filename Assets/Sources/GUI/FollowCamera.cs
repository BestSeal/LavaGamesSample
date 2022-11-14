using UnityEngine;
using Zenject;

namespace Sources.GUI
{
    public class FollowCamera : MonoBehaviour
    {
        private MainGameCamera _mainGameCamera;
        private Transform _cashedTransform;

        private void Awake()
        {
            _cashedTransform = transform;
        }

        [Inject]
        private void Initialize(SignalBus signalBus, MainGameCamera mainGameCamera)
        {
            _mainGameCamera = mainGameCamera;
        }

        private void Update() => transform.LookAt(_cashedTransform.position + _mainGameCamera.CashedTransform.rotation * Vector3.forward, 
            _mainGameCamera.CashedTransform.rotation * Vector3.up);
    }
}