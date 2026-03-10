using TD_Morpion_MAUI.ViewModels;

namespace TD_Morpion_MAUI.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }
}