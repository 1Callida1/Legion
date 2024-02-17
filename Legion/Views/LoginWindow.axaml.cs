using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Legion
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void StackPanelPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
        }

        private void ExitButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}