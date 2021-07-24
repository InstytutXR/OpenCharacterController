namespace OpenCharacterController.Examples
{
    public interface IJumpIntent : IIntent
    {
        bool wantsToJump { get; }
    }
}