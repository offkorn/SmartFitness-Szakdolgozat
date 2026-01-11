using Supabase;
using System.Text.Json;

namespace SmartFitness.Services
    {
    public static class SupabaseClient
    {


        private static Client _client;

        public static Client Client => _client
            ?? throw new InvalidOperationException("Supabase client is not initialized.");

        public static async Task InitializeAsync()
        {
            var config = AppConfig.Load();

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            _client = new Client(config.SupabaseUrl, config.SupabaseAnonKey, options);

            
            var saved = Preferences.Get("supabase_session", null);
            if (saved != null)
            {
                try
                {
                    var restoredSession = JsonSerializer.Deserialize<Supabase.Gotrue.Session>(saved);
                    if (restoredSession != null && !string.IsNullOrEmpty(restoredSession.AccessToken) && !string.IsNullOrEmpty(restoredSession.RefreshToken))
                    {
                        _ = _client.Auth.SetSession(restoredSession.AccessToken, restoredSession.RefreshToken);
                    }
                }
                catch { }
            }

            

            await _client.InitializeAsync();

            Console.WriteLine("--------------- ************* SESSION ON INIT = " + (SupabaseClient.Client.Auth.CurrentSession != null));
        }
    }
}


