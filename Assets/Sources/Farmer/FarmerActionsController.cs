using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Sources.Farmer
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class FarmerActionsController : MonoBehaviour
    {
        public event Action<float> RotationEvent;
        public event Action<float> MoveEvent; 
        public event Action StartPlantingEvent;
        public event Func<bool> PlantingEventFinished;

        private NavMeshAgent _navMeshAgent;
        private Sequence _sequence;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public async UniTask SetMoveDestination(Vector3 moveDestinationPoint, CancellationToken cancellationToken)
        {
            var rotation = Quaternion.LookRotation(transform.position - moveDestinationPoint);
            var duration = Mathf.Clamp(rotation.eulerAngles.y, 0, 180) / 90f;

            // reset sequence if still exists
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            await _sequence.AppendCallback(() =>
                {
                    RotationEvent?.Invoke(rotation.eulerAngles.y <= 180 ? duration : -duration);
                })
                .Append(transform.DOLookAt(moveDestinationPoint, duration, AxisConstraint.Y, Vector3.up))
                .AppendCallback(() => RotationEvent?.Invoke(0))
                .Play()
                .AsyncWaitForCompletion()
                .AsUniTask()
                .SuppressCancellationThrow();
            
            _navMeshAgent.destination = moveDestinationPoint;
            await UniTask.WaitUntil((() =>
                {
                    MoveEvent?.Invoke(_navMeshAgent.velocity.magnitude);
                    return IsPathDestinationReached();
                }), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            MoveEvent?.Invoke(0);
        }

        public bool IsPathDestinationReached() => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;

        public Vector3 GetFarmerVelocity() => _navMeshAgent.velocity;

        public async UniTask PlantAction(CancellationToken cancellationToken)
        {
            StartPlantingEvent?.Invoke();
            await UniTask.WaitWhile(() => PlantingEventFinished?.Invoke() ?? false, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
        }
    }
}