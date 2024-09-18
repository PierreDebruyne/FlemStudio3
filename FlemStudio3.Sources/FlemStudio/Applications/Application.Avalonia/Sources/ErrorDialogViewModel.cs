namespace FlemStudio.Applications.Avalonia
{
    public class ErrorDialogViewModel : ViewModelBase
    {

        public string DialogTitle => "An error occured.";

        public double MinHeight => 150;

        public double MinWidth => 300;

        public virtual double Height => MinHeight;
        public virtual double Width => MinWidth;


        public Action? OnOK = null;

        public string Message { get; }
        public string Description { get; }



        public ErrorDialogViewModel()
        {
            Message = "An error occured.";
            Description = "No details for this error.";
        }

        public ErrorDialogViewModel(Exception e)
        {
            Message = e.Message;
            Description = e.StackTrace;
        }

        public void ClickOk()
        {
            this.OnOK?.Invoke();
        }
    }
}
