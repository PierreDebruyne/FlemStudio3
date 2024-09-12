
using FlemStudio.LayoutManagement.Avalonia.Applications;
using ReactiveUI;
using System;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class ApplicationTableLayoutItemViewModel : ReactiveObject
    {
        protected ApplicationTableLayoutViewModel? TabLayoutViewModel;
        public IApplicationViewModel ApplicationViewModel { get; protected set; }



        public ApplicationTableLayoutItemViewModel(ApplicationTableLayoutViewModel tabLayoutViewModel, Guid appGuid)
        {
            TabLayoutViewModel = tabLayoutViewModel;
            if (appGuid == Guid.Empty)
            {
                ApplicationViewModel = new EmptyApplicationViewModel();
            }
            else if (TabLayoutViewModel != null)
            {
                ApplicationViewModel = TabLayoutViewModel.ApplicationFeatures.CreateApplicationViewModel(appGuid);
            }
        }

        protected bool _IsActive = false;
        public bool IsActive
        {
            get => _IsActive;
            set
            {
                this.RaiseAndSetIfChanged(ref _IsActive, value);
            }
        }

        public string Header
        {
            get => ApplicationViewModel.Header;
        }

        public string TabItem
        {
            get => ApplicationViewModel.TabItem;
        }

        public void OnClickTab()
        {
            this.TabLayoutViewModel?.SetActiveTab(this);
        }

        public void Dispose()
        {
            ApplicationViewModel.Dispose();
        }
    }
}
