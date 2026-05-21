using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views.VideoGame
{
    public partial class VideoGameDetailView : ContentPageBase
    {
        public VideoGameDetailView(VideoGameDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}