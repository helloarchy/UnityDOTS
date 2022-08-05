using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct Lifetime : IComponentData
    {
        public float Value;
    }
}