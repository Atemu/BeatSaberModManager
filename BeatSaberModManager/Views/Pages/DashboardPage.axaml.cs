﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;

using BeatSaberModManager.ViewModels;


namespace BeatSaberModManager.Views.Pages
{
    /// <summary>
    /// View for additional information and tools.
    /// </summary>
    public partial class DashboardPage : ReactiveUserControl<DashboardViewModel>
    {
        private FilePickerOpenOptions? _filePickerOpenOptions;
        private FilePickerOpenOptions FilePickerOpenOptions => _filePickerOpenOptions ??= new FilePickerOpenOptions
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("BeatSaber Playlist")
                {
                    Patterns = new [] { "*.bplist" }
                }
            }
        };

        /// <summary>
        /// [Required by Avalonia]
        /// </summary>
        public DashboardPage() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPage"/> class.
        /// </summary>
        public DashboardPage(DashboardViewModel viewModel, Window window)
        {
            InitializeComponent();
            ViewModel = viewModel;
            viewModel.PickPlaylistInteraction.RegisterHandler(async context => context.SetOutput(await SelectPlaylistFileAsync(window)));
        }

        private async Task<string?> SelectPlaylistFileAsync(TopLevel window)
        {
            IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(FilePickerOpenOptions);
            return files.Count == 1 ? files[0].TryGetLocalPath() : null;
        }
    }
}
