﻿using SpotifyDashboard.Server.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SpotifyDashboard.Server.Services
{
    public class TrackService
    {
        private readonly HttpClient _httpClient;

        public TrackService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<Track>> GetTopTenSongs(string token)
        {

            // General procedure to get the access token value
            var split = token.Split(' ');
            var auth = split[1];

            _httpClient.BaseAddress = new Uri("https://api.spotify.com/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth);

            // Http call to the spotify api address
            using HttpResponseMessage response = await _httpClient.GetAsync("v1/me/top/tracks");

            var responseBody = await response.Content.ReadAsStringAsync();

            // Retrieving the items from the json Object
            var jObj = JsonNode.Parse(responseBody)?.AsObject();
            var items = jObj["items"]?.AsArray();

            var tracks = JsonSerializer.Deserialize<List<Track>>(items.ToJsonString());

            // For each track object
            for (int i = 0; i < tracks.Count; i++)
            {
                var track = tracks[i];
                var item = items[i];

                // Assign to tthe Artist property the value of the artist name
                var artists = item["artists"]?.AsArray();
                if (artists.Count > 0)
                {
                    track.Artist = artists[0]["name"]?.ToString();
                }

                // Assign to the ImageUrl the value of the image url
                var album = item["album"]?.AsObject();
                var images = album["images"]?.AsArray();
                if (images.Count > 0)
                {
                    track.ImageUrl = images[0]["url"]?.ToString();
                }
            }

            return tracks;
        }

        public async Task<IEnumerable<Track>> GetRecommendedSongs(string token, string seedArtist, string seedGenres, string seedTrack)
        {

            // General procedure to get the access token value
            var split = token.Split(' ');
            var auth = split[1];

            var queryParams = $"seed_artists={seedArtist}&seed_genres={Uri.EscapeDataString(seedGenres)}&seed_tracks={seedTrack}";

            _httpClient.BaseAddress = new Uri("https://api.spotify.com/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth);

            // Http call to the spotify api address
            using HttpResponseMessage response = await _httpClient.GetAsync($"v1/recommendations?{queryParams}");

            var responseBody = await response.Content.ReadAsStringAsync();

            // Retrieving the tracks from the json Object
            var jObj = JsonNode.Parse(responseBody).AsObject();
            var tracks = jObj["tracks"].AsArray();

            var recommend = JsonSerializer.Deserialize<List<Track>>(tracks);

            // For each track object
            for (int i = 0; i < tracks.Count; i++)
            {
                var track = tracks[i];
                var rec = recommend[i];

                // Assign to the ImageUrl property the value of the images array url
                var album = track["album"].AsObject();
                var images = album["images"].AsArray();

                rec.ImageUrl = images[0]["url"]?.ToString();

                // Assign to the ArtistName property the artist object name value
                var artist = track["artists"].AsArray();
                var artistName = artist[0]["name"]?.ToString();

                // Assign to the SpotifyUrl property the value of the spotify external url
                var extUrl = track["external_urls"].AsObject();
                var url = extUrl["spotify"]?.ToString();

                rec.SpotifyUrl = url;

                rec.Artist = artistName;
            }

            return recommend;
        }
    }
}