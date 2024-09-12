using FlemStudio.LayoutManagement.Core.Applications;
using FlemStudio.LayoutManagement.Core.Editors;
using FlemStudio.LayoutManagement.Core.Layouts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.LayoutManagement.Core
{
    public class LayoutManager
    {
        public string FolderPath { get; }
        public LayoutService LayoutService { get; }
        public ApplicationService ApplicationService { get; }

        public LayoutManager(string folderPath)
        {
            FolderPath = folderPath;
            LayoutService = new LayoutService(folderPath + "/" + "Layouts");
            ApplicationService = new ApplicationService(folderPath + "/" + "Applications");

            LayoutService.AddLayoutType(new WindowLayoutType(LayoutService));
            LayoutService.AddLayoutType(new RowLayoutType(LayoutService));
            LayoutService.AddLayoutType(new ColumnLayoutType(LayoutService));
            LayoutService.AddLayoutType(new ApplicationTableLayoutType(LayoutService, ApplicationService));
            LayoutService.AddLayoutType(new EditorTableLayoutType(LayoutService));

            ApplicationService.AddApplicationType(new TestApplicationType());

        }

        public void Update(float deltaTime)
        {
            LayoutService.Update(deltaTime);
            ApplicationService.Update(deltaTime);
        }

        public void Dispose()
        {
            LayoutService.Dispose();
            ApplicationService.Dispose();
        }
    }
}
