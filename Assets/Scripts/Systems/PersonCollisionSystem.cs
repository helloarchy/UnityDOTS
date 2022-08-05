using System;
using System.Diagnostics;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public class PersonCollisionSystem : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        protected override void OnCreate()
        {
            // Get all systems built for the physics world
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();

            // Get all the modification physics (gravity, collisions, ...)
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            Dependency = new PersonCollisionJob
            {
                personGroup = GetComponentDataFromEntity<PersonTag>(true),
                colourGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
                seed = DateTimeOffset.Now.Millisecond
            }.Schedule(
                _stepPhysicsWorld.Simulation,
                ref _buildPhysicsWorld.PhysicsWorld,
                Dependency);
        }

        private struct PersonCollisionJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<PersonTag> personGroup;
            public ComponentDataFromEntity<URPMaterialPropertyBaseColor> colourGroup;
            public float seed;

            public void Execute(TriggerEvent triggerEvent)
            {
                var isEntityAPerson = personGroup.HasComponent(triggerEvent.EntityA);
                var isEntityBPerson = personGroup.HasComponent(triggerEvent.EntityB);

                if (!isEntityAPerson || !isEntityBPerson) return;

                var random = new Random((uint)(1 + seed + triggerEvent.BodyIndexA * triggerEvent.BodyIndexB));

                random = ChangeMaterialColour(random, triggerEvent.EntityA); // Reuse random to avoid duplication
                ChangeMaterialColour(random, triggerEvent.EntityB);
            }

            private Random ChangeMaterialColour(Random random, Entity entity)
            {
                if (colourGroup.HasComponent(entity))
                {
                    var colourComponent = colourGroup[entity];

                    colourComponent.Value.x = random.NextFloat(0, 1); // Where 1 is 255 from R
                    colourComponent.Value.y = random.NextFloat(0, 1); // Where 1 is 255 from G
                    colourComponent.Value.z = random.NextFloat(0, 1); // Where 1 is 255 from B

                    colourGroup[entity] = colourComponent; // Set it back
                }
                else
                {
                    Debug.Print("No match");
                }

                return random;
            }
        }
    }
}