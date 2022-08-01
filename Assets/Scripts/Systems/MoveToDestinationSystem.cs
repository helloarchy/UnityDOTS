using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class MoveToDestinationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var up = new float3(0, 1, 0);

            Entities.ForEach((
                ref Translation translation,
                ref Rotation rotation,
                in Destination destination,
                in MovementSpeed movementSpeed) =>
            {
                // Return early if we're already at the destination
                if (math.all(destination.Value == translation.Value)) return;

                var toDestination = destination.Value - translation.Value;
                rotation.Value = quaternion.LookRotation(toDestination, up);

                /* Move towards destination with each frame. If movement goes beyond destination,
                 then only move to the destination.*/
                var movement = math.normalize(toDestination) * movementSpeed.value * deltaTime;
                if (math.length(movement) >= math.length(toDestination))
                    translation.Value = destination.Value;
                else
                    translation.Value += movement;
            }).Schedule();
        }
    }
}