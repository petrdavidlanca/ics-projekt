using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.User
{
    public partial class UserDetailView : ContentPageBase
    {
        public UserDetailView(UserDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}