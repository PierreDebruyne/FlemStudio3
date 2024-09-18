using FlemStudio.LayoutManagement.Avalonia.Applications;
using FlemStudio.LayoutManagement.Core.Applications;
using FlemStudio.LayoutManagement.Core.Layouts;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class ApplicationTableLayoutViewModelType : LayoutContainerViewModelType
    {
        public ApplicationFeatures ApplicationFeatures { get; protected set; }
        public ApplicationTableLayoutViewModelType(ApplicationFeatures applicationFeatures, LayoutType type) : base(type)
        {
            ApplicationFeatures = applicationFeatures;
        }

        public override LayoutViewModel CreateLayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutUser user)
        {
            return new ApplicationTableLayoutViewModel(layoutViewModelService, this, (ApplicationTableLayoutUser)user);
        }

        public ApplicationTableLayoutViewModel? LastFocusedTab = null;

        public void OnTabFocus(ApplicationTableLayoutViewModel tabLayout)
        {
            LastFocusedTab = tabLayout;
            OnFocus?.Invoke(tabLayout);
        }

        public override MenuItemViewModel? CreateMenuItemViewModel()
        {
            List<MenuItemViewModel> applicationTypeMenuItems = ApplicationFeatures.CreateMenuItemViewModels();
            return new MenuItemViewModel(Name, LayoutType.Type, applicationTypeMenuItems);
        }
    }
}
