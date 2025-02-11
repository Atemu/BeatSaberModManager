﻿using System.Text.Json.Serialization;


namespace BeatSaberModManager.Models.Implementations.BeatSaber.BeatMods
{
    /// <summary>
    /// Provides a download link as well as the mod's files and their hashes.
    /// </summary>
    public class BeatModsDownload
    {
        /// <summary>
        /// The url to download the mod from.
        /// </summary>
        [JsonPropertyName("url")]
        public required string Url { get; init; }

        /// <summary>
        /// The mod's files and their corresponding hashes.
        /// </summary>
        [JsonPropertyName("hashMd5")]
        public required BeatModsHash[] Hashes { get; init; }
    }
}
