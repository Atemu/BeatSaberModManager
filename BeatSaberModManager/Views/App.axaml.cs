using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;

using BeatSaberModManager.Views.Localization;
using BeatSaberModManager.Views.Theming;
using BeatSaberModManager.Views.Windows;

using ReactiveUI;

using Serilog;


namespace BeatSaberModManager.Views
{
    /// <inheritdoc />
    public class App : Application
    {
        private readonly ILogger _logger;
        private readonly LocalizationManager _localizationManager;
        private readonly ThemeManager _themeManager;
        private readonly Lazy<Window> _mainWindow;

        /// <inheritdoc />
        public App(IEnumerable<IDataTemplate> viewTemplates, ILogger logger, LocalizationManager localizationManager, ThemeManager themeManager, Lazy<Window> mainWindow)
        {
            _logger = logger;
            _localizationManager = localizationManager;
            _themeManager = themeManager;
            _mainWindow = mainWindow;
            DataTemplates.AddRange(viewTemplates);
            if (Program.IsProduction)
                RxApp.DefaultExceptionHandler = Observer.Create<Exception>(e => _ = ShowExceptionAsync(e));
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            _localizationManager.Initialize(this);
            _themeManager.Initialize(this);
        }

        /// <inheritdoc />
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.MainWindow = _mainWindow.Value;
        }

        private async Task ShowExceptionAsync(Exception e)
        {
            _logger.Fatal(e, "Application crashed");
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime { MainWindow: not null } lifetime)
                return;
            lifetime.MainWindow.Show();
            await new ExceptionWindow(e).ShowDialog(lifetime.MainWindow);
            lifetime.Shutdown(-1);
        }
    }
}
