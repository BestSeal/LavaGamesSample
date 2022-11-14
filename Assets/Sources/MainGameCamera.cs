using Cinemachine;
using UnityEngine;
using Zenject;

namespace Sources
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    [RequireComponent(typeof(Camera))]
    public class MainGameCamera : MonoBehaviour
    {
        private SignalBus _signulBus;
        public CinemachineVirtualCamera VirtualCamera { get; private set; }
        public Camera MainCamera { get; private set; }
        public Transform CashedTransform { get; private set; }

        private void Awake()
        {
            VirtualCamera = GetComponent<CinemachineVirtualCamera>();
            MainCamera = GetComponent<Camera>();
            CashedTransform = transform;
        }

        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            _signulBus = signalBus;
        }
    }
}