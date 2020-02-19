using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace PowerCenter.UWP.Controls
{
    public sealed partial class HotkeyTextBox : UserControl
    {
        private VirtualKey _key = VirtualKey.None;
        private VirtualKeyModifiers _modifiers = VirtualKeyModifiers.None;

        public HotkeyTextBox()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty HotkeyProperty =
            DependencyProperty.Register(
                "Hotkey",
                typeof(Hotkey),
                typeof(HotkeyTextBox),
                new PropertyMetadata(
                    new Hotkey(),
                    Hotkey_Changed));

        public Hotkey Hotkey
        {
            get { return (Hotkey)GetValue(HotkeyProperty); }
            set { SetValue(HotkeyProperty, value); }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            _key = VirtualKey.None;
            _modifiers = VirtualKeyModifiers.None;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            // Save result
            Hotkey = new Hotkey(_key, _modifiers);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            e.Handled = true;
            
            var key = e.Key;

            var isModifier = false;
            var modifier = VirtualKeyModifiers.None;
            if (key == VirtualKey.Control)
            {
                isModifier = true;
                modifier = VirtualKeyModifiers.Control;
            }
            else if (key == VirtualKey.Menu)
            {
                isModifier = true;
                modifier = VirtualKeyModifiers.Menu;
            }
            else if (key == VirtualKey.LeftWindows || key == VirtualKey.RightWindows)
            {
                isModifier = true;
                modifier = VirtualKeyModifiers.Windows;
            }
            else if (key == VirtualKey.Shift)
            {
                isModifier = true;
                modifier = VirtualKeyModifiers.Shift;
            }

            if (isModifier)
            {
                _modifiers |= modifier;
                OnHotkeyChanged();
            }
            else if (_key == VirtualKey.None)
            {
                _key = key;
                OnHotkeyChanged();
            }
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnHotkeyChanged()
        {
            textBox.Text = Hotkey.SerializeHotkey(new Hotkey(_key, _modifiers)); 
        }

        private static void Hotkey_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HotkeyTextBox htb = (HotkeyTextBox)d;
            var hotkey = htb.Hotkey;

            htb.textBox.Text = Hotkey.SerializeHotkey(hotkey);
        }
    }
}
