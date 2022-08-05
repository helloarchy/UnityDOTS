using Components;
using Unity.Entities;

namespace Systems
{
    public class LifetimeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var buffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) =>
            {
                lifetime.Value -= deltaTime;

                if (lifetime.Value <= 0f)
                    // Schedule entity for destruction now out of life
                    buffer.DestroyEntity(entityInQueryIndex, entity);
            }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}