namespace FirstPersonController
{
    public interface IJumpIntent : IIntent
    {
        bool wantsToJump { get; }
    }
}