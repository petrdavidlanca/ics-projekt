using StackOverFIT.App.ViewModels;

namespace StackOverFIT.App.Views;

public abstract partial class ContentPageBase : ContentPage
{
    protected ViewModelBase ViewModel { get; }

    public ContentPageBase(ViewModelBase viewModel)
    {
        InitializeComponent();

        BindingContext = ViewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ViewModel.OnAppearingAsync();
    }
}