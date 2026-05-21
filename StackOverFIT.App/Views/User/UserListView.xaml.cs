using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.User
{
    public partial class UserListView : ContentPageBase
    {
        public UserListView(UserListViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}