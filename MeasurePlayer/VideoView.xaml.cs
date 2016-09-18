namespace MeasurePlayer
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class VideoView : UserControl
    {
        public VideoView()
        {
            this.InitializeComponent();
        }

        //public void Step(int frames)
        //{
        //    if (this.MediaElement.Position == null || this.Info?.FrameRate == null)
        //    {
        //        return;
        //    }

        //    var jump = frames / this.Info.FrameRate;
        //    if (jump != null)
        //    {
        //        this.MediaElement.Skip(jump.Value);
        //    }
        //}

        //private static int Multiplier()
        //{
        //    if (Keyboard.Modifiers == ModifierKeys.Control)
        //    {
        //        return 10;
        //    }

        //    if (Keyboard.Modifiers == ModifierKeys.Shift)
        //    {
        //        return 100;
        //    }

        //    if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        //    {
        //        return 1000;
        //    }

        //    return 1;
        //}

        //private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    this.Step(Multiplier() * Math.Sign(e.Delta));
        //    e.Handled = true;
        //}

        //private void OnKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Left)
        //    {
        //        this.Step(-Multiplier());
        //        e.Handled = true;
        //    }
        //    else if (e.Key == Key.Right)
        //    {
        //        this.Step(Multiplier());
        //        e.Handled = true;
        //    }
        //}
    }
}
