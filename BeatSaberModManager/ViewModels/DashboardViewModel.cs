﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using BeatSaberModManager.Models.Implementations.Progress;
using BeatSaberModManager.Services.Implementations.BeatSaber.Playlists;
using BeatSaberModManager.Services.Implementations.Observables;
using BeatSaberModManager.Services.Interfaces;
using BeatSaberModManager.Utils;

using ReactiveUI;


namespace BeatSaberModManager.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="BeatSaberModManager.Views.Pages.DashboardPage"/>.
    /// </summary>
    public class DashboardViewModel : ViewModelBase
    {
        private readonly SettingsViewModel _settingsViewModel;
        private readonly IStatusProgress _statusProgress;
        private readonly PlaylistInstaller _playlistInstaller;
        private readonly ObservableAsPropertyHelper<string?> _gameVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        public DashboardViewModel(ModsViewModel modsViewModel, SettingsViewModel settingsViewModel, IGameVersionProvider gameVersionProvider, IGameLauncher gameLauncher, IGamePathsProvider gamePathsProvider, IStatusProgress statusProgress, PlaylistInstaller playlistInstaller)
        {
            _settingsViewModel = settingsViewModel;
            _statusProgress = statusProgress;
            _playlistInstaller = playlistInstaller;
            AppVersion = Program.Version;
            ModsViewModel = modsViewModel;
            PickPlaylistInteraction = new Interaction<Unit, string?>();
            settingsViewModel.ValidatedInstallDirObservable.SelectMany(gameVersionProvider.DetectGameVersionAsync).ToProperty(this, nameof(GameVersion), out _gameVersion);
            DirectoryExistsObservable appDataDirExistsObservable = new();
            settingsViewModel.ValidatedInstallDirObservable.Select(gamePathsProvider.GetAppDataPath).Subscribe(x => appDataDirExistsObservable.Path = x);
            OpenAppDataCommand = ReactiveCommand.Create<Unit, bool>(_ => PlatformUtils.TryOpenUri(appDataDirExistsObservable.Path!), appDataDirExistsObservable.ObserveOn(RxApp.MainThreadScheduler));
            DirectoryExistsObservable logsDirExistsObservable = new();
            settingsViewModel.ValidatedInstallDirObservable.Select(gamePathsProvider.GetLogsPath).Subscribe(x => logsDirExistsObservable.Path = x);
            OpenLogsCommand = ReactiveCommand.Create<Unit, bool>(_ => PlatformUtils.TryOpenUri(logsDirExistsObservable.Path!), logsDirExistsObservable.ObserveOn(RxApp.MainThreadScheduler));
            UninstallModLoaderCommand = ReactiveCommand.CreateFromTask(() => modsViewModel.UninstallModLoaderAsync(settingsViewModel.InstallDir!), modsViewModel.IsSuccessObservable);
            UninstallAllModsCommand = ReactiveCommand.CreateFromTask(() => modsViewModel.UninstallAllModsAsync(settingsViewModel.InstallDir!), modsViewModel.IsSuccessObservable);
            LaunchGameCommand = ReactiveCommand.Create(() => gameLauncher.LaunchGame(settingsViewModel.InstallDir!), settingsViewModel.IsInstallDirValidObservable);
            InstallPlaylistCommand = ReactiveCommand.CreateFromObservable(() => PickPlaylistInteraction.Handle(Unit.Default)
                .WhereNotNull()
                .SelectMany(InstallPlaylistAsync), settingsViewModel.IsInstallDirValidObservable);
        }

        private async Task<bool> InstallPlaylistAsync(string x)
        {
            bool result = await _playlistInstaller.InstallPlaylistAsync(_settingsViewModel.InstallDir!, x, _statusProgress);
            _statusProgress.Report(new ProgressInfo(result ? StatusType.Completed : StatusType.Failed, null));
            return result;
        }

        /// <summary>
        /// The version of the application.
        /// </summary>
        public string AppVersion { get; }

        /// <summary>
        /// The ViewModel for a mods view.
        /// </summary>
        public ModsViewModel ModsViewModel { get; }

        /// <summary>
        /// Opens the game's AppData directory in the file explorer.
        /// </summary>
        public ReactiveCommand<Unit, bool> OpenAppDataCommand { get; }

        /// <summary>
        /// Opens the game's Logs directory in the file explorer.
        /// </summary>
        public ReactiveCommand<Unit, bool> OpenLogsCommand { get; }

        /// <summary>
        /// Uninstalls the mod loader.
        /// </summary>
        public ReactiveCommand<Unit, Unit> UninstallModLoaderCommand { get; }

        /// <summary>
        /// Uninstalls all installed mods.
        /// </summary>
        public ReactiveCommand<Unit, Unit> UninstallAllModsCommand { get; }

        /// <summary>
        /// Launches the game.
        /// </summary>
        public ReactiveCommand<Unit, Unit> LaunchGameCommand { get; }

        /// <summary>
        /// Install a see cref="BeatSaberModManager.Models.Implementations.BeatSaber.Playlists.Playlist"/>.
        /// </summary>
        public ReactiveCommand<Unit, bool> InstallPlaylistCommand { get; }

        /// <summary>
        /// Asks the user to select a playlist file to install.
        /// </summary>
        public Interaction<Unit, string?> PickPlaylistInteraction { get; }

        /// <summary>
        /// The version of the game.
        /// </summary>
        public string? GameVersion => _gameVersion.Value;
    }
}
