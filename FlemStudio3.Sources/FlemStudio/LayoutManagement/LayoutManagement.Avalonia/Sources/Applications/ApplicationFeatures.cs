
using FlemStudio.LayoutManagement.Core.Applications;
using ReactiveUI;

namespace FlemStudio.LayoutManagement.Avalonia.Applications
{
    public class ApplicationFeatures
    {
        protected ApplicationService ApplicationService;
        internal Dictionary<string, ApplicationViewModelType> ApplicationViewModelTypes = new();


        public ApplicationFeatures(ApplicationService applicationService)
        {
            ApplicationService = applicationService;

        }

        public void AddApplicationType(ApplicationViewModelType type)
        {
            this.ApplicationViewModelTypes.Add(type.ApplicationType.Name, type);

        }

        public ApplicationViewModel CreateApplicationViewModel(Guid guid)
        {
            ApplicationUser user = ApplicationService.UseApplication(guid);
            ApplicationViewModelType applicationViewModelType;
            try
            {
                applicationViewModelType = ApplicationViewModelTypes[user.ApplicationType];

            }
            catch (Exception ex)
            {
                user.Dispose();
                throw new Exception("This application type is not implemented: " + user.ApplicationType, ex);
            }
            return applicationViewModelType.CreateApplicationViewModel(user);
        }

        public ApplicationUser CreateNewApplication(string type)
        {
            return ApplicationService.UseNewApplication(type);

        }

        public List<MenuItemViewModel> CreateMenuItemViewModels()
        {
            List<MenuItemViewModel> items = new();
            foreach (var appType in ApplicationViewModelTypes.Values)
            {
                MenuItemViewModel? item = appType.CreateMenuItemViewModel();
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }


    }

    public interface IApplicationViewModel : IDisposable
    {
        public abstract string Header { get; }

        public abstract string TabItem { get; }
    }

    public abstract class ApplicationViewModel : ReactiveObject, IApplicationViewModel
    {
        protected ApplicationViewModelType ApplicationViewModelType;
        protected ApplicationUser ApplicationUser;

        protected ApplicationViewModel(ApplicationViewModelType applicationViewModelType, ApplicationUser applicationUser)
        {
            ApplicationViewModelType = applicationViewModelType;
            ApplicationUser = applicationUser;
        }

        public abstract string Header { get; }

        public abstract string TabItem { get; }

        public virtual void Dispose()
        {
            ApplicationUser.Dispose();
        }
    }

    public abstract class ApplicationViewModel<TApplicationViewModelType, TApplicationUser> : ApplicationViewModel where TApplicationViewModelType : ApplicationViewModelType where TApplicationUser : ApplicationUser
    {
        protected new TApplicationViewModelType ApplicationViewModelType;
        protected new TApplicationUser ApplicationUser;
        protected ApplicationViewModel(TApplicationViewModelType applicationViewModelType, TApplicationUser applicationUser) : base(applicationViewModelType, applicationUser)
        {
            ApplicationViewModelType = applicationViewModelType;
            ApplicationUser = applicationUser;
        }
    }


    public abstract class ApplicationViewModelType
    {
        public ApplicationType ApplicationType { get; protected set; }
        public abstract string Name { get; }

        protected ApplicationViewModelType(ApplicationType applicationType)
        {
            ApplicationType = applicationType;
        }

        public abstract ApplicationViewModel CreateApplicationViewModel(ApplicationUser user);

        public virtual MenuItemViewModel? CreateMenuItemViewModel()
        {
            return new MenuItemViewModel(Name, ApplicationType.Name);
        }
    }

    public abstract class ApplicationViewModelType<TApplicationType, TApplicationViewModel, TApplicationUser> : ApplicationViewModelType
        where TApplicationType : ApplicationType
        where TApplicationViewModel : ApplicationViewModel
        where TApplicationUser : ApplicationUser
    {
        protected ApplicationViewModelType(ApplicationType applicationType) : base(applicationType)
        {
        }

        public override ApplicationViewModel CreateApplicationViewModel(ApplicationUser user)
        {
            return DoCreateApplicationViewModel((TApplicationUser)user);
        }

        public abstract TApplicationViewModel DoCreateApplicationViewModel(TApplicationUser user);
    }

    /*

    public class AssetFileExplorerViewModelType : ApplicationViewModelType
    {
        protected AssetFilesFeatures AssetFilesFeatures;

        public AssetFileExplorerViewModelType(AssetFilesFeatures assetFilesFeatures) : base("AssetFileExplorer")
        {
            AssetFilesFeatures = assetFilesFeatures;
        }

        public override string Name => "Asset File Explorer";

        public override ApplicationViewModel CreateApplicationViewModel(ApplicationUser user)
        {
            return new AssetFilesExplorerViewModel(AssetFilesFeatures, (AssetFileExplorerApplicationUser)user);
        }
    }

    public class AssetFileHierarchyViewModelType : ApplicationViewModelType
    {
        protected AssetFilesFeatures AssetFilesFeatures;

        public AssetFileHierarchyViewModelType(AssetFilesFeatures assetFilesFeatures) : base("AssetFileHierarchy")
        {
            AssetFilesFeatures = assetFilesFeatures;
        }

        public override string Name => "Asset File Hierarchy";

        public override ApplicationViewModel CreateApplicationViewModel(ApplicationUser user)
        {
            return new AssetFilesHierarchyViewModel(AssetFilesFeatures, (AssetFileHierarchyApplicationUser)user);
        }
    }

    public class AssetRegistryViewModelType : ApplicationViewModelType
    {
        protected AssetFilesFeatures AssetFilesFeatures;

        public AssetRegistryViewModelType(AssetFilesFeatures assetFilesFeatures) : base("AssetRegistry")
        {
            AssetFilesFeatures = assetFilesFeatures;
        }

        public override string Name => "Asset Registry";

        public override ApplicationViewModel CreateApplicationViewModel(ApplicationUser user)
        {
            return new AssetRegistryViewModel(AssetFilesFeatures, (AssetRegistryApplicationUser)user);
        }
    }

    */
}
