using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace ImagesExtension.Avalonia
{
    public partial class CreateImageSourceAssetDialogView : UserControl
    {
        public CreateImageSourceAssetDialogView()
        {
            InitializeComponent();

        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            OpenFilePickerButton.Click += OnClickFilePickerButton;
        }

        public static FilePickerFileType ImageAll { get; } = new("All Images")
        {
            Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.webp" },
            AppleUniformTypeIdentifiers = new[] { "public.image" },
            MimeTypes = new[] { "image/*" }
        };
        private async void OnClickFilePickerButton(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Image",
                AllowMultiple = false,
                FileTypeFilter = [ImageAll]
            });

            if (files.Count >= 1)
            {
                Input.Text = files[0].Path.AbsolutePath;

            }
        }
    }
}
