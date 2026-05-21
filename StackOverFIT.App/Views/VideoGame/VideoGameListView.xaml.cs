using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.VideoGame
{
    public partial class VideoGameListView : ContentPageBase
    {
        public VideoGameListView(VideoGameListViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}