using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.Library
{
    public partial class LibraryListView : ContentPageBase
    {
        public LibraryListView(LibraryListViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}