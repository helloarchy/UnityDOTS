using Unity.Entities;
using UnityEngine;

namespace MonoBehaviours
{
    public class SetupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject personPrefab;
        [SerializeField] private int gridSize;
        [SerializeField] private int spread;

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
            entityManager.Instantiate(entity);
        }

        private void OnDestroy()
        {
            _blob.Dispose();
        }
    }
}