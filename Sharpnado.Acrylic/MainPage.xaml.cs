﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Sharpnado.Presentation.Forms.RenderedViews;
using Sharpnado.Tasks;

using Xamarin.Forms;

namespace Sharpnado.Acrylic
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private bool _isAcrylicBlurEnabled;

        private MaterialFrame.MaterialFrame.BlurStyle _blurStyle;

        private bool _isSettingsShown;

        private static readonly GridLength SettingsRowHeight = new GridLength(30);

        private readonly Button[] _blurStyleButtons;

        public MainPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            _isAcrylicBlurEnabled = true;

            SettingsFrame.IsVisible = _isSettingsShown;
            SettingsFrame.Opacity = _isSettingsShown ? 1 : 0;
            BlurStyleRow.Height = !_isAcrylicBlurEnabled ? new GridLength() : SettingsRowHeight;

            _blurStyleButtons = new[] { LightButton, DarkButton, ExtraLightButton };

            ResourcesHelper.SetAcrylic(_isAcrylicBlurEnabled);
            BlurStyleButtonOnClicked(LightButton, EventArgs.Empty);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TaskMonitor.Create(DelayExecute);
        }

        private async Task DelayExecute()
        {
            await Task.Delay(3000);

            var dump = PlatformHelper.Instance.DumpNativeViewHierarchy(Search, true);
            Console.WriteLine($"Search Frame dump:{Environment.NewLine}{dump}");

            dump = PlatformHelper.Instance.DumpNativeViewHierarchy(ImageFrame, true);
            Console.WriteLine($"Image Frame dump:{Environment.NewLine}{dump}");
        }

        private void SettingsButtonOnClicked(object sender, EventArgs e)
        {
            if (!_isSettingsShown)
            {
                BlurStyleRow.Height = _isAcrylicBlurEnabled ? SettingsRowHeight : 0;
                SettingsFrame.IsVisible = true;

                TaskMonitor.Create(SettingsFrame.FadeTo(1));
                _isSettingsShown = true;
                return;
            }

            // Hide
            _isSettingsShown = false;
            TaskMonitor.Create( async() =>
                {
                    await SettingsFrame.FadeTo(0);
                    SettingsFrame.IsVisible = false;
                });
        }

        private void SwitchOnToggled(object sender, ToggledEventArgs e)
        {
            var switchBlur = (Switch)sender;

            _isAcrylicBlurEnabled = switchBlur.IsToggled;

            ResourcesHelper.SetAcrylic(_isAcrylicBlurEnabled);
            if (_isAcrylicBlurEnabled)
            {
                BlurStyleButtonOnClicked(LightButton, EventArgs.Empty);
            }

            BlurStyleRow.Height = _isAcrylicBlurEnabled ? SettingsRowHeight : 0;
        }

        private void BlurStyleButtonOnClicked(object sender, EventArgs e)
        {
            var selectedButton = (Button)sender;

            if (selectedButton == LightButton)
            {
                _blurStyle = MaterialFrame.MaterialFrame.BlurStyle.Light;
            }
            else if (selectedButton == DarkButton)
            {
                _blurStyle = MaterialFrame.MaterialFrame.BlurStyle.Dark;
            }
            else
            {
                _blurStyle = MaterialFrame.MaterialFrame.BlurStyle.ExtraLight;
            }

            ResourcesHelper.SetBlurStyle(_blurStyle);

            selectedButton.TextColor = _blurStyle != MaterialFrame.MaterialFrame.BlurStyle.ExtraLight
                                           ? ResourcesHelper.GetResourceColor("TextPrimaryColor" )
                                           : ResourcesHelper.GetResourceColor("TextPrimaryDarkColor");

            selectedButton.BackgroundColor = ResourcesHelper.GetResourceColor(ResourcesHelper.DynamicPrimaryColor);

            foreach (var button in _blurStyleButtons)
            {
                if (button == selectedButton)
                {
                    continue;
                }

                button.TextColor = _blurStyle != MaterialFrame.MaterialFrame.BlurStyle.ExtraLight
                                       ? ResourcesHelper.GetResourceColor("TextPrimaryDarkColor")
                                       : ResourcesHelper.GetResourceColor("TextPrimaryColor" );
                button.BackgroundColor = Color.Transparent;
            }
        }
    }
}
