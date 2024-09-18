namespace FlemStudio.Applications.Avalonia
{
    public class MenuViewModel
    {
        public Action<List<object>>? Submit;
        public List<MenuItemViewModel> Items { get; set; }

        public MenuViewModel(List<MenuItemViewModel> items)
        {
            Items = items;

            foreach (var item in Items)
            {
                item.Submit = OnItemCommand;
            }
        }

        public void OnItemCommand(List<object> paramters)
        {
            Submit?.Invoke(paramters);
        }
    }

    public class MenuItemViewModel
    {
        public Action<List<object>>? Submit;
        public List<MenuItemViewModel>? Items { get; set; }
        public string Header { get; }
        public object Parameters { get; }

        public MenuItemViewModel(string header, object parameters, List<MenuItemViewModel>? items = null)
        {
            Header = header;
            Parameters = parameters;
            Items = items;
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    item.Submit = OnItemSubmit;
                }
            }
        }


        public void Command()
        {
            if (Items == null)
            {
                Submit?.Invoke(new List<object> { Parameters });
            }
        }
        public void OnItemSubmit(List<object> itemParamters)
        {
            List<object> paramters = [Parameters, .. itemParamters];
            Submit?.Invoke(paramters);
        }
    }
}
