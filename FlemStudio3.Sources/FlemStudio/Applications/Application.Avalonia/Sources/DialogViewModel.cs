namespace FlemStudio.Applications.Avalonia
{
    public abstract class DialogViewModel : ViewModelBase
    {
        public abstract string DialogTitle { get; }
        public abstract double MinHeight { get; }
        public abstract double MinWidth { get; }

        public virtual double Height => MinHeight;
        public virtual double Width => MinWidth;


        public Action? OnSubmit = null;
        public Action? OnCancel = null;
    }




}
