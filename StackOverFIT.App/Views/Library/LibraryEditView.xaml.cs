using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.Library
{
    public partial class LibraryEditView : ContentPageBase
    {
        public LibraryEditView(LibraryEditViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}