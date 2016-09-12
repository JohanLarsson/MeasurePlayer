namespace MeasurePlayer
{
    using System.Windows.Input;

    public class Commands
    {
        public static RoutedUICommand ToggleFullScreen { get; } = new RoutedUICommand("Toggle fullscreen", nameof(ToggleFullScreen),typeof(Commands));
    }
}
