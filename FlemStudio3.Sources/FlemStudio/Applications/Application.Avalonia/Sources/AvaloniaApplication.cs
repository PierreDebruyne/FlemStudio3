using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.Applications.Avalonia
{
    public abstract class AvaloniaApplication
    {
        private List<ApplicationWindow> Windows { get; } = new();
        protected ApplicationWindow? FocusedWindow { get; private set; }

        public ApplicationWindow MainWindow => Windows[0];

        protected void AddWindow(ApplicationWindow window)
        {
            Windows.Add(window);
            window.OnFocus = OnWindowFocus;
            window.OnClose = OnWindowClose;
            if (FocusedWindow == null)
            {
                FocusedWindow = window;
            }
        }

        private void OnWindowFocus(ApplicationWindow window)
        {
            Debug.WriteLine("window got focus");
            FocusedWindow = window;
        }

        private void OnWindowClose(ApplicationWindow window)
        {
            Windows.Remove(window);
        }

        public async Task OpenDialog<T>(T viewModel, Action submit) where T : DialogViewModel
        {
            
            if (FocusedWindow != null)
            {
                var dialog = new DialogWindow()
                {
                    DataContext = viewModel
                };
                viewModel.OnSubmit += () =>
                {
                    dialog.Close(null);
                    submit.Invoke();
                };
                viewModel.OnCancel += () =>
                {
                    dialog.Close(null);
                };
                await dialog.ShowDialog<T>(FocusedWindow);
                
            }
            
        }

        public async Task OpenErrorDialog<T>(T viewModel) where T : ErrorDialogViewModel
        {
            if (FocusedWindow != null)
            {
                var dialog = new DialogWindow()
                {
                    DataContext = viewModel
                };
                viewModel.OnOK += () =>
                {
                    dialog.Close(null);
                    
                };
                await dialog.ShowDialog<T>(FocusedWindow);
            }
        }
    }
}
