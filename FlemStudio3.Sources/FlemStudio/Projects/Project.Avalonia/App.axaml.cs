using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FlemStudio.Project.Avalonia.Sources;
using FlemStudio.Project.Core;
using FlemStudio.Project.UI;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using FlemStudio.LayoutManagement.Core.Layouts;
using FlemStudio.LayoutManagement.Avalonia.Layouts;



namespace FlemStudio.Project.Avalonia;

public partial class App : Application
{

    static string InstallDirectory = "C:\\Users\\Pierre\\Desktop\\FlemStudio\\FlemStudio3\\InstallFolder";
    protected FlemStudioProject Project;
    protected FlemStudioProjectUI ProjectUI;
    protected FlemStudioProjectAvalonia ProjectAvalonia;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.Args.Length == 0)
            {
                throw new Exception("No project to open.");
            }

            Project = new FlemStudioProject(InstallDirectory, desktop.Args[0], ["core", "avalonia"]);
            ProjectUI = new FlemStudioProjectUI(Project, Project.ProjectDirectoryPath + "/" + "UI");
            ProjectAvalonia = new FlemStudioProjectAvalonia(ProjectUI);

            desktop.Startup += OnStartup;
            desktop.Exit += OnExit;

            ProjectAvalonia.Init();

            desktop.MainWindow = ProjectAvalonia.MainWindow;
            
        }
        /*
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }
        */

        base.OnFrameworkInitializationCompleted();
    }


    protected Task? UpdateTask;
    protected bool Closing = false;

    private void OnStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
    {
        UpdateTask = new Task(OnUpdate);
        UpdateTask.Start();
        Debug.WriteLine("Update task launched.");
    }

    private void OnUpdate()
    {
        DateTime lastUpdate = DateTime.Now;
        while (Closing == false)
        {
            DateTime now = DateTime.Now;
            float deltaTime = (float)(now - lastUpdate).TotalSeconds;
            Project.Update(deltaTime);
            ProjectUI.Update(deltaTime);

            lastUpdate = now;
            Thread.Sleep(200);
        }
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        Closing = true;
        if (UpdateTask != null)
        {
            Task.WaitAll(new[] { UpdateTask });
            UpdateTask.Dispose();
            Debug.WriteLine("Update task stopped.");
        }
        ProjectUI.Dispose();
        Project.Dispose();

    }
}
