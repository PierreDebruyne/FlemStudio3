using FlemStudio.LayoutManagement.Core.Layouts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class LayoutViewModelService
    {
        protected LayoutService LayoutService;
        protected Dictionary<string, LayoutViewModelType> LayoutViewModelTypes = new();
        protected Dictionary<string, LayoutContainerViewModelType> LayoutContainerViewModelTypes = new();

        public LayoutViewModelService(LayoutService layoutService)
        {
            LayoutService = layoutService;

            
        }

        public void AddLayoutType(LayoutViewModelType type)
        {
            this.LayoutViewModelTypes.Add(type.Name, type);
            if (type.GetType().IsAssignableTo(typeof(LayoutContainerViewModelType)))
            {
                this.LayoutContainerViewModelTypes.Add(type.Name, (LayoutContainerViewModelType)type);
                ((LayoutContainerViewModelType)type).OnFocus += OnContainerFocus;
            }

        }

        public LayoutContainerViewModel? FocusedContainer = null;
        private void OnContainerFocus(LayoutContainerViewModel container)
        {
            if (FocusedContainer != null)
            {
                FocusedContainer.IsFocus = false;
                FocusedContainer = null;
            }
            FocusedContainer = container;
            FocusedContainer.IsFocus = true;

        }

        public LayoutViewModel CreateLayoutViewModel(Guid guid)
        {
            LayoutUser user = LayoutService.UseLayout(guid);
            try
            {
                LayoutViewModelType layoutViewModelType = LayoutViewModelTypes[user.LayoutType];
                return layoutViewModelType.CreateLayoutViewModel(this, user);
            }
            catch (Exception ex)
            {
                user.Dispose();
                throw new Exception("This layout type is not implemented: " + user.LayoutType, ex);
            }
        }

        public LayoutUser UseLayout(Guid guid)
        {
            return LayoutService.UseLayout(guid);
        }

        public LayoutUser UseNewLayout(string type)
        {
            return LayoutService.UseNewLayout(type);
        }

        public void RemoveLayout(Guid guid, bool removeChildren)
        {
            LayoutService.RemoveLayout(guid, removeChildren);
        }

        private bool _EditMode = false;
        public bool EditMode
        {
            get => _EditMode;
            set
            {
                if (_EditMode == value) return;
                _EditMode = value;
                EditModeChanged?.Invoke(value);
            }
        }

        public Action<bool>? EditModeChanged;

        public List<MenuItemViewModel> CreateContainerMenuItemViewModels()
        {
            List<MenuItemViewModel> items = new();
            foreach (var containerType in LayoutContainerViewModelTypes.Values)
            {
                MenuItemViewModel? item = containerType.CreateMenuItemViewModel();
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }

    public abstract class LayoutViewModel : ReactiveObject
    {
        public LayoutViewModelService LayoutViewModelService { get; }
        public LayoutViewModelType LayoutType { get; }

        public Action<Guid>? NeedSimplify;

        protected LayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutViewModelType layoutType)
        {
            LayoutViewModelService = layoutViewModelService;
            LayoutType = layoutType;
        }

        public abstract void Dispose();

        
    }

    public abstract class LayoutViewModelType
    {
        public LayoutType LayoutType { get; protected set; }
        public string Name => LayoutType.Type;

        protected LayoutViewModelType(LayoutType layoutType)
        {
            LayoutType = layoutType;
        }

        public abstract LayoutViewModel CreateLayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutUser user);
    }

    public abstract class LayoutContainerViewModelType : LayoutViewModelType
    {
        protected LayoutContainerViewModelType(LayoutType type) : base(type)
        {
        }
        public Action<LayoutContainerViewModel>? OnFocus;

        public abstract MenuItemViewModel? CreateMenuItemViewModel();
    }

    public abstract class LayoutContainerViewModel : LayoutViewModel
    {
        protected LayoutContainerViewModel(LayoutViewModelService layoutViewModelService, LayoutViewModelType layoutType): base(layoutViewModelService, layoutType)
        {
            
        }

        private bool _IsFocus = false;
        public bool IsFocus
        {
            get { return _IsFocus; }
            set
            {
                _IsFocus = value;
                this.RaisePropertyChanged(nameof(IsFocus));
            }
        }
    }
}
