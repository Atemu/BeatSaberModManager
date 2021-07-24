﻿using Avalonia;
using Avalonia.ReactiveUI;


namespace BeatSaberModManager
{
    public class Program
    {
        public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        private static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .UseReactiveUI()
                      .LogToTrace();
    }
}