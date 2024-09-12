using FlemStudio.LayoutManagement.Core.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlemStudio.LayoutManagement.Avalonia.Layouts
{
  

    public class ColumnLayoutViewModelType : LayoutViewModelType
    {
        public ColumnLayoutViewModelType(LayoutType type) : base(type)
        {
            
        }

        public override LayoutViewModel CreateLayoutViewModel(LayoutViewModelService layoutViewModelService, LayoutUser user)
        {
            return new ColumnLayoutViewModel(layoutViewModelService, this, (ColumnLayoutUser)user);
        }
    }
}
