using FlemStudio.LayoutManagement.Avalonia.Layouts;
using FlemStudio.LayoutManagement.Core.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
    public class WindowLayoutViewModelType : LayoutViewModelType
    {
        public WindowLayoutViewModelType(LayoutType layoutType) : base(layoutType)
        {
        }

        public override LayoutViewModel CreateLayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutUser user)
        {
            return new WindowLayoutViewModel(layoutViewModelService, this, (WindowLayoutUser)user);
        }
    }
}
