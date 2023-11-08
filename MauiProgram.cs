using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using tcc_mypet_app.Views.App;
using tcc_mypet_app.Views.Auth;

namespace tcc_mypet_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCore()
                .UseMauiCommunityToolkitMarkup()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Lobster-Regular.ttf", "Lobster");
                    fonts.AddFont("Typo_Round_Bold_Demo.otf", "TypoRoundBoldDemo");
                    fonts.AddFont("Poppins-Black.ttf", "PoppinsBlack");
                    fonts.AddFont("Poppins-BlackItalic.ttf", "PoppinsBlackItalic");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                    fonts.AddFont("Poppins-BoldItalic.ttf", "PoppinsBoldItalic");
                    fonts.AddFont("Poppins-ExtraBold.ttf", "PoppinsExtraBold");
                    fonts.AddFont("Poppins-ExtraBoldItalic.ttf", "PoppinsExtraBoldItalic");
                    fonts.AddFont("Poppins-ExtraLight.ttf", "PoppinsExtraLight");
                    fonts.AddFont("Poppins-ExtraLightItalic.ttf", "PoppinsExtraLightItalic");
                    fonts.AddFont("Poppins-Italic.ttf", "PoppinsItalic");
                    fonts.AddFont("Poppins-Light.ttf", "PoppinsLight");
                    fonts.AddFont("Poppins-LightItalic.ttf", "PoppinsLightItalic");
                    fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                    fonts.AddFont("Poppins-MediumItalic.ttf", "PoppinsMediumItalic");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                    fonts.AddFont("Poppins-SemiBoldItalic.ttf", "PoppinsSemiBoldItalic");
                    fonts.AddFont("Poppins-Thin.ttf", "PoppinsThin");
                    fonts.AddFont("Poppins-ThinItalic.ttf", "PoppinsThinItalic");
                }).RegisterViews();

            #if DEBUG
            builder.Logging.AddDebug();
            #endif
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                #if __ANDROID__
                                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                #elif __IOS__
                                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                #endif
            });
            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(Picker), (handler, view) =>
            {
                #if __ANDROID__
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                #elif __IOS__
                                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                #endif
            });

            return builder.Build();
        }
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<LoginView>();
            mauiAppBuilder.Services.AddTransient<HomeView>();
            mauiAppBuilder.Services.AddTransient<AppView>();

            return mauiAppBuilder;
        }
    }
}