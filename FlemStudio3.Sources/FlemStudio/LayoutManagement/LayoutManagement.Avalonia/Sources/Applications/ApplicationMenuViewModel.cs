using ReactiveUI;
using System;
using System.Collections.Generic;

namespace FlemStudio.LayoutManagement.Avalonia.Applications
{

    public class ApplicationMenuViewModel : ReactiveObject
    {
        protected ApplicationFeatures ApplicationFeatures;
        public List<ApplicationMenuItemViewModel> Items { get; } = new();

        protected Action<string> Command;

        public ApplicationMenuViewModel(ApplicationFeatures applicationFeatures, Action<string> command)
        {
            ApplicationFeatures = applicationFeatures;
            Command = command;
            foreach (var applicationType in ApplicationFeatures.ApplicationViewModelTypes.Values)
            {
                Items.Add(new ApplicationMenuItemViewModel(this, applicationType.Name, applicationType.ApplicationType.Name));
            }
        }

        internal void OnItemCommand(ApplicationMenuItemViewModel item)
        {
            Command.Invoke(item.ApplicationType);
        }
    }
    public class ApplicationMenuItemViewModel : ReactiveObject
    {
        protected ApplicationMenuViewModel ApplicationMenuViewModel;
        public string Header { get; }
        public string ApplicationType { get; }


        public ApplicationMenuItemViewModel(ApplicationMenuViewModel applicationMenuViewModel, string header, string applicationType)
        {
            ApplicationMenuViewModel = applicationMenuViewModel;
            Header = header;
            ApplicationType = applicationType;
        }

        public void Command()
        {
            ApplicationMenuViewModel.OnItemCommand(this);
        }



    }
}
