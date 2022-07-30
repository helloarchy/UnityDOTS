using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Systems
{
    /**
     * Include array of random number generators as part of the initialisation group
     */
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class RandomSystem : SystemBase
    {
        public NativeArray<Random> RandomArray { get; private set; }

        protected override void OnCreate()
        {
            // Number of thread available on the system running
            const int availableThreads = JobsUtility.MaxJobThreadCount;
            var seed = new System.Random();

            // Initialise using seed for different randomness
            var randomArray = new Random[availableThreads];
            for (var i = 0; i < availableThreads; i++)
                randomArray[i] = new Random((uint) seed.Next());

            RandomArray = new NativeArray<Random>(randomArray, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            RandomArray.Dispose();
        }
    }
}