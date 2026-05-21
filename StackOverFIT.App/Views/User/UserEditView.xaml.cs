using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.User
{
    public partial class UserEditView : ContentPageBase
    {
        public UserEditView(UserEditViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}