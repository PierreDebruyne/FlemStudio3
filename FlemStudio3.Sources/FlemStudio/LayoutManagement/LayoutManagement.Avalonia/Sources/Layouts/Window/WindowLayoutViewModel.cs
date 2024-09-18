using FlemStudio.LayoutManagement.Core.Layouts;
using ReactiveUI;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class WindowLayoutViewModel : LayoutViewModel
    {

        protected WindowLayoutUser WindowLayoutUser;
        public bool EditMode => LayoutViewModelService.EditMode;
        private void OnEditModeChanged(bool obj)
        {
            this.RaisePropertyChanged(nameof(EditMode));
        }


        public Guid ContentGuid => WindowLayoutUser.ContentLayout;
        public LayoutViewModel? ContentLayoutViewModel { get; protected set; }

        public WindowLayoutViewModel(LayoutViewModelService layoutViewModelService, WindowLayoutViewModelType type, WindowLayoutUser windowLayoutUser) : base(layoutViewModelService, type)
        {

            WindowLayoutUser = windowLayoutUser;
            WindowLayoutUser.ContentLayoutChanged += OnContentLayoutChanged;
            LayoutViewModelService.EditModeChanged += OnEditModeChanged;

            if (ContentGuid != Guid.Empty)
            {

                ContentLayoutViewModel = LayoutViewModelService.CreateLayoutViewModel(WindowLayoutUser.ContentLayout);
                ContentLayoutViewModel.NeedSimplify += OnNeedSimplify;
            }
        }

        private void OnContentLayoutChanged(Guid oldContentGuid, Guid newContentGuid)
        {
            ContentLayoutViewModel?.Dispose();
            if (newContentGuid == Guid.Empty)
            {
                ContentLayoutViewModel = null;
            }
            else
            {
                ContentLayoutViewModel = LayoutViewModelService.CreateLayoutViewModel(newContentGuid);
                ContentLayoutViewModel.NeedSimplify += OnNeedSimplify;
            }
            this.RaisePropertyChanged(nameof(ContentLayoutViewModel));
        }
        private void OnNeedSimplify(Guid layoutGuidToKeep)
        {

            Guid oldItemGuid = WindowLayoutUser.ContentLayout;
            WindowLayoutUser.ContentLayout = layoutGuidToKeep;
            LayoutViewModelService.RemoveLayout(oldItemGuid, false);

        }
        public override void Dispose()
        {
            WindowLayoutUser.Dispose();
        }
    }
}
