using FlemStudio.LayoutManagement.Avalonia.Applications;
using FlemStudio.LayoutManagement.Avalonia.Layouts;
using FlemStudio.LayoutManagement.Core;
using FlemStudio.LayoutManagement.Core.Applications;

namespace FlemStudio.LayoutManagement.Avalonia
{
    public class LayoutManagerAvalonia
    {
        protected LayoutManager LayoutManager;
        public LayoutViewModelService LayoutFeatures { get; }
        public ApplicationFeatures ApplicationFeatures { get; }

        public LayoutManagerAvalonia(LayoutManager layoutManager)
        {
            LayoutManager = layoutManager;
            LayoutFeatures = new LayoutViewModelService(LayoutManager.LayoutService);
            ApplicationFeatures = new ApplicationFeatures(LayoutManager.ApplicationService);


            WindowLayoutViewModelType windowType = new WindowLayoutViewModelType(LayoutManager.LayoutService.GetLayoutType("Window"));
            LayoutFeatures.AddLayoutType(windowType);

            ColumnLayoutViewModelType columnType = new ColumnLayoutViewModelType(LayoutManager.LayoutService.GetLayoutType("Column"));
            LayoutFeatures.AddLayoutType(columnType);

            RowLayoutViewModelType rowType = new RowLayoutViewModelType(LayoutManager.LayoutService.GetLayoutType("Row"));
            LayoutFeatures.AddLayoutType(rowType);

            ApplicationTableLayoutViewModelType appTableType = new ApplicationTableLayoutViewModelType(ApplicationFeatures, LayoutManager.LayoutService.GetLayoutType("ApplicationTable"));
            LayoutFeatures.AddLayoutType(appTableType);



            TestApplicationViewModelType testAppType = new TestApplicationViewModelType(LayoutManager.ApplicationService.GetApplicationType<TestApplicationType>());
            ApplicationFeatures.AddApplicationType(testAppType);
        }
    }
}
