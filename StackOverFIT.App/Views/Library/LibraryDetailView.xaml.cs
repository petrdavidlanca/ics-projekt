using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.Library
{
    public partial class LibraryDetailView : ContentPageBase
    {
        public LibraryDetailView(LibraryDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}