using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using System;

namespace FlemStudio.Project.Avalonia;

public class ViewLocator : IDataTemplate
{
    public Control Build(object data)
    {
        var viewModelType = data.GetType();

        var viewName = viewModelType.FullName!.Replace("ViewModel", "View");


        var type = viewModelType.Assembly.GetType(viewName);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + viewName };
        }
    }

    public bool Match(object? data)
    {
        return data is ReactiveObject;
    }
}
