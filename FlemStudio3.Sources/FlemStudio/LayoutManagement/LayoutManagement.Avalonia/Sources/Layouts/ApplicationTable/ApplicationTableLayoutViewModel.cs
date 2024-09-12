
using FlemStudio.LayoutManagement.Avalonia.Applications;
using FlemStudio.LayoutManagement.Core.Applications;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class ApplicationTableLayoutViewModel : LayoutContainerViewModel
    {

        internal ApplicationTableLayoutUser TabLayout;


        protected ApplicationTableLayoutViewModelType LayoutViewModelType;
        internal ApplicationFeatures ApplicationFeatures => LayoutViewModelType.ApplicationFeatures;

        public ApplicationTableLayoutViewModel(LayoutViewModelService layoutViewModelService, ApplicationTableLayoutViewModelType layoutViewModelType, ApplicationTableLayoutUser tabLayout) : base (layoutViewModelService, layoutViewModelType)
        {
            LayoutViewModelType = layoutViewModelType;
            TabLayout = tabLayout;
            Init();
        }

        public ObservableCollection<ApplicationTableLayoutItemViewModel> Tabs { get; } = new();
        protected ApplicationTableLayoutItemViewModel? _ActiveTab;
        public ApplicationTableLayoutItemViewModel? ActiveTab
        {
            get => _ActiveTab;
            set
            {
                if (_ActiveTab == value) return;
                if (_ActiveTab != null)
                {
                    _ActiveTab.IsActive = false;
                }
                _ActiveTab = value;
                if (_ActiveTab != null)
                {
                    _ActiveTab.IsActive = true;
                }
                this.RaisePropertyChanged(nameof(ActiveTab));
            }
        }

        private void Init()
        {
            TabLayout.ActiveTabChanged += OnActiveTabChanged;
            TabLayout.TabAdded += OnTabAdded;
            TabLayout.TabRemoved += OnTabRemoved;

            int tabCount = TabLayout.TabCount;
            for (int i = 0; i < tabCount; i++)
            {

                Tabs.Add(new ApplicationTableLayoutItemViewModel(this, TabLayout.GetAppGuid(i)));
            }
            if (TabLayout.ActiveTab > -1)
            {
                ActiveTab = Tabs[TabLayout.ActiveTab];
            }

            AddApplicationMenu = new(LayoutViewModelType.ApplicationFeatures, AddApplicationMenuCommand);
        }

        public override void Dispose()
        {
            TabLayout.Dispose();
        }

        private void OnActiveTabChanged(int oldActiveTab, int newActiveTab)
        {
            if (newActiveTab > -1)
            {
                ActiveTab = Tabs[newActiveTab];
            }
            else
            {
                ActiveTab = null;
            }

        }
        internal void SetActiveTab(ApplicationTableLayoutItemViewModel? tab)
        {
            if (tab == null)
            {
                TabLayout.ActiveTab = -1;
            }
            else
            {
                int index = Tabs.IndexOf(tab);
                TabLayout.ActiveTab = index;
            }
        }
        private void OnTabAdded(int index)
        {
            Tabs.Insert(index, new ApplicationTableLayoutItemViewModel(this, TabLayout.GetAppGuid(index)));
            this.RaisePropertyChanged(nameof(IsTableVisible));
        }

        private void OnTabRemoved(int index)
        {

            ApplicationTableLayoutItemViewModel item = Tabs[index];
            Tabs.RemoveAt(index);
            item.Dispose();
            if (index == TabLayout.ActiveTab)
            {
                if (index >= 0 && index < Tabs.Count)
                {
                    ActiveTab = Tabs[index];
                }
                else
                {
                    ActiveTab = null;
                }
            }


            this.RaisePropertyChanged(nameof(IsTableVisible));
        }

        internal void OnCloseCommand()
        {
            if (ActiveTab != null)
            {

                TabLayout.RemoveTab(TabLayout.ActiveTab);

            }
        }

        public bool IsTableVisible => Tabs.Count > 1;
        //public bool IsTableVisible => true;


        public ApplicationMenuViewModel AddApplicationMenu { get; protected set; }



        public void AddApplicationMenuCommand(string applicationType)
        {
            TabLayout.AddTab_NewApp(TabLayout.TabCount, applicationType);
            TabLayout.ActiveTab = TabLayout.TabCount - 1;
        }

        public void Focus()
        {
            this.IsFocus = !this.IsFocus;
            LayoutViewModelType.OnTabFocus(this);
        }
    }



}
