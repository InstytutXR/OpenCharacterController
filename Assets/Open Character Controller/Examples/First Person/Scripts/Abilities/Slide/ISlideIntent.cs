namespace OpenCharacterController.Examples
{
    public interface ISlideIntent : IIntent
    {
        bool wantsToSlide { get; }
    }
}