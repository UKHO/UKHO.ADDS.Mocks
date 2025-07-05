

namespace UKHO.ADDS.Mocks.Domain.States
{
    public sealed class StateDefinition
    {
        public StateDefinition(string state, string description)
        {
            State = state;
            Description = description;
        }

        public string State { get; }
        public string Description { get; }
    }
}
