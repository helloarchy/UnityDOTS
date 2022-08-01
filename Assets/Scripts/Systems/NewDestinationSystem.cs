using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class NewDestinationSystem : SystemBase
    {
        private RandomSystem _randomSystem;

        protected override void OnCreate()
        {
            _randomSystem = World.GetExistingSystem<RandomSystem>();
        }

        protected override void OnUpdate()
        {
            var randoms = _randomSystem.RandomArray;

            // Allow access to the random array across threads
            Entities
                .WithNativeDisableParallelForRestriction(randoms)
                .ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) =>
                {
                    var distance = math.abs(math.length(destination.Value - translation.Value));
                    if (distance < 0.1f)
                    {
                        var random = randoms[nativeThreadIndex];
                        destination.Value.x = random.NextFloat(0, 500);
                        destination.Value.z = random.NextFloat(0, 500);

                        // Save accessed array back into it to preserve next fetching.
                        randoms[nativeThreadIndex] = random;
                    }
                }).ScheduleParallel();
        }
    }
}