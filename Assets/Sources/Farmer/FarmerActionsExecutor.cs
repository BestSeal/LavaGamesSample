using Cysharp.Threading.Tasks;
using Sources.Farmer.Actions;
using Sources.Signals;
using UnityEngine;
using Zenject;

namespace Sources.Farmer
{
    public class FarmerActionsExecutor : MonoBehaviour
    {
        public bool ExecutorBusy { get; private set; }
        
        private SignalBus _signalBus;

        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            _signalBus.Subscribe<ActionSelectedSignal>(OnActionSelectedSignalHandler);
        }

        private void OnActionSelectedSignalHandler(ActionSelectedSignal signal)
        {
            if(signal?.Action == null) return;

            if (ExecutorBusy) return;
            
            ActionTaskWrapper(signal.Action).Forget();
        }

        private async UniTaskVoid ActionTaskWrapper(BaseAction action)
        {
            ExecutorBusy = true;
            await action.PerformAction(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            ExecutorBusy = false;
        }
    }
}