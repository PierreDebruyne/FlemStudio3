using FlemStudio.AssetExplorerApplication.Core;
using FlemStudio.LayoutManagement.Core;
using FlemStudio.LayoutManagement.Core.Applications;
using FlemStudio.LayoutManagement.Core.Layouts;
using FlemStudio.Project.Core;

namespace FlemStudio.Project.UI
{
    public class FlemStudioProjectUI
    {
        public FlemStudioProject CoreProject { get; }
        public string FolderPath { get; }

        public LayoutManager LayoutManager { get; }
        public MainLayoutUser MainLayoutUser { get; }

        public FlemStudioProjectUI(FlemStudioProject coreProject, string folderPath)
        {
            CoreProject = coreProject;
            FolderPath = folderPath;
            DirectoryInfo layoutFolderInfo = new DirectoryInfo(FolderPath);
            if (layoutFolderInfo.Exists == false)
            {
                layoutFolderInfo.Create();
            }

            LayoutManager = new LayoutManager(FolderPath + "/" + "Layouts");

            LayoutManager.ApplicationService.AddApplicationType(new AssetExplorerApplicationType(CoreProject.AssetManager));




            MainLayoutUser = LayoutManager.LayoutService.UseMainLayout();
            if (MainLayoutUser.WindowCount == 0)
            {
                SetupDefaultWindow();
            }

        }

        public void Update(float deltaTime)
        {
            LayoutManager.Update(deltaTime);
        }

        public void Dispose()
        {
            MainLayoutUser.Dispose();
            LayoutManager.Dispose();
        }


        protected void SetupDefaultWindow()
        {
            using (WindowLayoutUser windowLayoutUser = (WindowLayoutUser)LayoutManager.LayoutService.UseNewLayout("Window"))
            {
                using (RowLayoutUser rowLayoutUser0 = (RowLayoutUser)LayoutManager.LayoutService.UseNewLayout("Row"))
                {
                    using (ColumnLayoutUser columnLayoutUser0 = (ColumnLayoutUser)LayoutManager.LayoutService.UseNewLayout("Column"))
                    {
                        using (ApplicationTableLayoutUser appLayoutUser0 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser0.SetLayoutGuid(0, appLayoutUser0.LayoutGuid);
                        }

                        using (ApplicationTableLayoutUser appLayoutUser1 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser0.SetLayoutGuid(1, appLayoutUser1.LayoutGuid);
                        }
                        using (ApplicationTableLayoutUser appLayoutUser2 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser0.AddItem(2, appLayoutUser2.LayoutGuid);
                        }

                        rowLayoutUser0.SetLayoutGuid(0, columnLayoutUser0.LayoutGuid);
                    }
                    using (ColumnLayoutUser columnLayoutUser1 = (ColumnLayoutUser)LayoutManager.LayoutService.UseNewLayout("Column"))
                    {
                        using (ApplicationTableLayoutUser appLayoutUser0 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser1.SetLayoutGuid(0, appLayoutUser0.LayoutGuid);
                        }

                        using (ApplicationTableLayoutUser appLayoutUser1 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser1.SetLayoutGuid(1, appLayoutUser1.LayoutGuid);
                        }

                        rowLayoutUser0.SetLayoutGuid(1, columnLayoutUser1.LayoutGuid);
                    }
                    using (ColumnLayoutUser columnLayoutUser2 = (ColumnLayoutUser)LayoutManager.LayoutService.UseNewLayout("Column"))
                    {
                        using (ApplicationTableLayoutUser appLayoutUser0 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser2.SetLayoutGuid(0, appLayoutUser0.LayoutGuid);
                        }

                        using (ApplicationTableLayoutUser appLayoutUser1 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser2.SetLayoutGuid(1, appLayoutUser1.LayoutGuid);
                        }
                        using (ApplicationTableLayoutUser appLayoutUser2 = (ApplicationTableLayoutUser)LayoutManager.LayoutService.UseNewLayout("ApplicationTable"))
                        {
                            columnLayoutUser2.AddItem(2, appLayoutUser2.LayoutGuid);
                        }
                        rowLayoutUser0.AddItem(2, columnLayoutUser2.LayoutGuid);
                    }
                    windowLayoutUser.ContentLayout = rowLayoutUser0.LayoutGuid;
                }




                MainLayoutUser.AddWindow(windowLayoutUser);
            }
        }
    }
}
