using CommunityToolkit.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Syncfusion.Maui.Core.Hosting;

namespace SmartFitness;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("InriaSerif-Light.ttf", "InriaSerifLight");
                fonts.AddFont("InriaSerif-Bold.ttf", "InriaSerifBold");
                fonts.AddFont("InriaSerif-Regular.ttf", "InriaSerifRegular");
                fonts.AddFont("MadimiOne-Regular.ttf", "MadamiOne");
                fonts.AddFont("K2D-Bold.ttf", "K2D-Bold");
                fonts.AddFont("K2D-ExtraBold.ttf", "K2D-ExtraBold");
                fonts.AddFont("K2D-ExtraLight.ttf", "K2D-ExtraLight");
                fonts.AddFont("K2D-Light.ttf", "K2D-Light");
                fonts.AddFont("K2D-Medium.ttf", "K2D-Medium");
                fonts.AddFont("K2D-Regular.ttf", "K2D-Regular");
                fonts.AddFont("K2D-Thin.ttf", "K2D-Thin");
                fonts.AddFont("Kalam-Regular.ttf", "Kalam-Regular");
            }
            ).UseMauiCommunityToolkit()
            .ConfigureSyncfusionCore();

        // Aláhúzás eltüntetése Entry komponensnél Androidon
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", static (h, v) =>
        {
#if ANDROID
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
#if IOS
            h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

        return builder.Build();

    }
}
