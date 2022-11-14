using Sources.Farmer;
using Sources.Signals;
using Zenject;

namespace Sources.DI
{
    public class BaseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<FieldCellSelectedSignal>().OptionalSubscriber();
            Container.DeclareSignal<ActionSelectedSignal>().OptionalSubscriber();
            Container.DeclareSignal<PlantGrownSignal>().OptionalSubscriber();
            Container.DeclareSignal<HarvestedSignal>().OptionalSubscriber();

            Container.Bind<GameTimer>().FromComponentInHierarchy().AsSingle();
            Container.Bind<MainGameCamera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FarmerActionsController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<FarmerActionsExecutor>().FromComponentInHierarchy().AsSingle();
            Container.Bind<HarvestedDataStorage>().AsSingle();
        }
    }
}