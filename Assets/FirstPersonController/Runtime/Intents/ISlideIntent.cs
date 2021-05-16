namespace FirstPersonController
{
    public interface ISlideIntent : IIntent
    {
        bool wantsToSlide { get; }
    }
}