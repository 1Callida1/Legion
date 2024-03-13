using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Legion.Views.ControlTemplates
{
    public class CorneredTextBox : TemplatedControl
    {
        public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<CorneredTextBox, string>(nameof(Text));

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<int> MaxLengthProperty = AvaloniaProperty.Register<CorneredTextBox, int>(nameof(MaxLength));

        public int MaxLength
        {
            get => GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly StyledProperty<int> TextPaddingProperty = AvaloniaProperty.Register<CorneredTextBox, int>(nameof(TextPadding));

        public int TextPadding
        {
            get => GetValue(TextPaddingProperty);
            set => SetValue(TextPaddingProperty, value);
        }
    }
}
