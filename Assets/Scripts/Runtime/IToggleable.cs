namespace HandcraftedGames.Common
{
    public interface IToggleable
    {
        bool IsEnabled { get; }

        void Enable();
        void Disable();
    }
}