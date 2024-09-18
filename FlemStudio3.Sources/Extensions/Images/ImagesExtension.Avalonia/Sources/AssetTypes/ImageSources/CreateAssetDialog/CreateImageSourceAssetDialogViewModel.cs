using FlemStudio.Applications.Avalonia;
using ReactiveUI;

namespace ImagesExtension.Avalonia
{
    public class CreateImageSourceAssetDialogViewModel : DialogViewModel
    {
        public override string DialogTitle => "Create image source Asset";

        public override double MinHeight => 150;

        public override double MinWidth => 400;

        private string _ImagePath = "";
        public string ImagePath
        {
            get => _ImagePath;
            set
            {
                this.RaiseAndSetIfChanged(ref _ImagePath, value);
                CheckForError();
                this.RaisePropertyChanged(nameof(CanSubmit));

            }
        }

        private bool _HasError = false;
        public bool HasError
        {
            get => _HasError;
            protected set
            {
                this.RaiseAndSetIfChanged(ref  this._HasError, value);
            }
        }

        private string? _ErrorMessage = null;
        public string? ErrorMessage
        {
            get => _ErrorMessage;
            protected set
            {
                this.RaiseAndSetIfChanged(ref this._ErrorMessage, value);
            }
        }


        protected void CheckForError()
        {
            if (ImagePath.Length > 0)
            {
                FileInfo fileInfo = new FileInfo(ImagePath);
                if (fileInfo.Exists == false)
                {
                    HasError = true;
                    ErrorMessage = "This image does not exist.";
                }
                else
                {
                    HasError = false;
                    ErrorMessage = null;
                }
            }
            else
            {
                ErrorMessage = null;
                HasError = false;
            }
            
        }

        public bool CanSubmit => ImagePath.Length > 0 && HasError == false;

        public void Submit()
        {
            this.OnSubmit?.Invoke();
        }

        public void Cancel()
        {
            this.OnCancel?.Invoke();
        }
    }
}
