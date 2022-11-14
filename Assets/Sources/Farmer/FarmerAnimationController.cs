using UnityEngine;

namespace Sources.Farmer
{
    [RequireComponent(typeof(Animator))]
    public class FarmerAnimationController : MonoBehaviour
    {
        [SerializeField] private FarmerActionsController farmerActionsController;
        
        private Animator _animator;
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int Plant = Animator.StringToHash("Plant");

        public bool IsPlanting { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            farmerActionsController.RotationEvent += OnRotationEventHandler;
            
            farmerActionsController.PlantingEventFinished += OnPlantingEventFinishedHandler;
            farmerActionsController.StartPlantingEvent += OnStartPlantingEventHandler;

            farmerActionsController.MoveEvent += OnMoveEventHandler;
        }

        private void OnMoveEventHandler(float speed)
        {
            _animator.SetFloat(Forward, speed);
        }

        private void OnStartPlantingEventHandler()
        {
            IsPlanting = true;
            _animator.SetTrigger(Plant);
        }

        private bool OnPlantingEventFinishedHandler() => IsPlanting;

        private void OnRotationEventHandler(float rotationSide)
        {
            _animator.SetFloat(Turn, rotationSide);
        }

        public void Planted()
        {
        }

        public void AnimationFinished() => IsPlanting = false;

        private void OnDestroy()
        {
            farmerActionsController.RotationEvent -= OnRotationEventHandler;
            farmerActionsController.PlantingEventFinished -= OnPlantingEventFinishedHandler;
            farmerActionsController.StartPlantingEvent -= OnStartPlantingEventHandler;
            farmerActionsController.MoveEvent -= OnMoveEventHandler;
        }
    }
}
