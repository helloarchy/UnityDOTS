using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct MovementSpeed : IComponentData
    {
        public float value;
    }
}
