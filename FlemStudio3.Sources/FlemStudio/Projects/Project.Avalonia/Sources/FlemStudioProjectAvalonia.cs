using FlemStudio.Applications.Avalonia;
using FlemStudio.AssetExplorerApplication.Avalonia;
using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.AssetManagement.Avalonia;
using FlemStudio.LayoutManagement.Avalonia;
using FlemStudio.LayoutManagement.Avalonia.Layouts;
using FlemStudio.Project.Avalonia.Sources;
using FlemStudio.Project.UI;
using System;

namespace FlemStudio.Project.Avalonia
{
    public class FlemStudioProjectAvalonia : AvaloniaApplication
    {
        protected FlemStudioProjectUI ProjectUI;
        public LayoutManagerAvalonia LayoutManagerAvalonia { get; }
        public AssetManagerAvalonia AssetManagerAvalonia { get; }


        public FlemStudioProjectAvalonia(FlemStudioProjectUI projectUI)
        {
            ProjectUI = projectUI;
            LayoutManagerAvalonia = new LayoutManagerAvalonia(projectUI.LayoutManager);
            AssetManagerAvalonia = new AssetManagerAvalonia(this, projectUI.CoreProject.AssetManager, projectUI.CoreProject.ExtensionImporter);


            LayoutManagerAvalonia.ApplicationFeatures.AddApplicationType(new AssetExplorerApplicationViewModelType(projectUI.LayoutManager.ApplicationService.GetApplicationType<AssetExplorerApplicationType>(), AssetManagerAvalonia));


        }

        public void Init()
        {
            for (int i = 0; i < ProjectUI.MainLayoutUser.WindowCount; i++)
            {
                Guid windowLayoutGuid = ProjectUI.MainLayoutUser.GetWindowContent(i);
                WindowLayoutViewModel windowViewModel = (WindowLayoutViewModel)LayoutManagerAvalonia.LayoutFeatures.CreateLayoutViewModel(windowLayoutGuid);
                MainViewModel mainViewModel = new MainViewModel(windowViewModel);

                MainWindow mainWindow = new MainWindow()
                {
                    DataContext = mainViewModel
                };

                this.AddWindow(mainWindow);

            }
        }


    }
}
