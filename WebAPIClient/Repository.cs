﻿namespace WebAPIClient
{
    using System;
    using System.Text.Json.Serialization;

    // Sealed: No class can inherit from this class
    public sealed record class Repository(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("html_url")] Uri GitHubHomeUrl,
        [property: JsonPropertyName("homepage")] Uri Homepage,
        [property: JsonPropertyName("watchers")] int Watchers,
        [property: JsonPropertyName("pushed_at")] DateTime LastPushUtc)
    {
        public DateTime LastPush => LastPushUtc.ToLocalTime();
    }
}
