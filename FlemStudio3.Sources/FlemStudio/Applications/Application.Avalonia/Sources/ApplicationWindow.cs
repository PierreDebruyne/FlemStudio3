using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.Applications.Avalonia
{
    public class ApplicationWindow : Window
    {


        public override void BeginInit()
        {
            base.BeginInit();
            this.GotFocus += Focus;
            this.Closed += Close;
        }

        public Action<ApplicationWindow>? OnFocus;

        private void Focus(object? sender, GotFocusEventArgs e)
        {
            OnFocus?.Invoke(this);
        }

        public Action<ApplicationWindow>? OnClose;
        private void Close(object? sender, EventArgs e)
        {
            OnClose?.Invoke(this);
        }
    }
}
