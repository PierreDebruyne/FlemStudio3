using FlemStudio.LayoutManagement.Avalonia;
using FlemStudio.LayoutManagement.Avalonia.Layouts;

namespace FlemStudio.Project.Avalonia.Sources;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public LayoutMenuViewModel LayoutMenuViewModel { get; protected set; }
    public WindowLayoutViewModel WindowLayoutViewModel { get; }

    public MainViewModel(WindowLayoutViewModel windowLayoutViewModel)
    {
        WindowLayoutViewModel = windowLayoutViewModel;
        LayoutMenuViewModel = new(windowLayoutViewModel.LayoutViewModelService);

    }

    public void Dispose()
    {
        WindowLayoutViewModel.Dispose();
    }
}
