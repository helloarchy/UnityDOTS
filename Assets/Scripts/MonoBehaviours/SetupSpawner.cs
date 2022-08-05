using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    public class SetupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject personPrefab;
        [SerializeField] private int gridSize;
        [SerializeField] private int spread;
        [SerializeField] private Vector2 moveSpeedRange = new Vector2(4, 7);
        [SerializeField] private Vector2 lifetimeRange = new Vector2(10, 100);

        private BlobAssetStore _blob;

        /**
         * Spawn entities
         * Allocate memory, convert prefab to an entity, instantiate in game world
         * using the worlds entity manager.
         */
        private void Start()
        {
            _blob = new BlobAssetStore();

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blob);
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(personPrefab, settings);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            for (var x = 0; x < gridSize; x++)
            for (var z = 0; z < gridSize; z++)
            {
                var instance = entityManager.Instantiate(entity);
                var position = new float3(x * spread, 0.5f, z * spread);

                entityManager.SetComponentData(instance, new Translation
                {
                    Value = position
                });

                entityManager.SetComponentData(instance, new Destination
                {
                    Value = position
                });

                var speed = Random.Range(moveSpeedRange.x, moveSpeedRange.y);
                entityManager.SetComponentData(instance, new MovementSpeed
                {
                    Value = speed
                });

                var lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y);
                entityManager.SetComponentData(instance, new Lifetime
                {
                    Value = lifetime
                });
            }
        }

        private void OnDestroy()
        {
            _blob.Dispose();
        }
    }
}