using Avalonia;
using Avalonia.Controls;
using System;

namespace FlemStudio.Applications.Avalonia
{
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
            //this.Opened += OnOpened;

        }



        private void OnOpened(object? sender, EventArgs e)
        {
            int window_w = (int)this.DesiredSize.Width / 2;
            int window_h = (int)this.DesiredSize.Height / 2;

            int x = (int)(this.Owner.Bounds.Width / 2) - window_w;
            int y = (int)(this.Owner.Bounds.Height / 2) - window_h;
            this.Position = new PixelPoint(x, y);

        }
    }
}
