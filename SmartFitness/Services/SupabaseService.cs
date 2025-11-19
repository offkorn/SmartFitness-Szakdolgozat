using Supabase;

namespace SmartFitness.Services
{
    public static class SupabaseClient
    {
        private static Client _client;

        public static Client Client => _client ?? throw new InvalidOperationException("Supabase client is not initialized. Call Initialize() first.");

        public static void Initialize()
        {
            try
            {
                var url = "https://wctvnxubclukdhgrzwkp.supabase.co";
                var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndjdHZueHViY2x1a2RoZ3J6d2twIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTkxNTg5NTIsImV4cCI6MjA3NDczNDk1Mn0.x-yBvv0yf6LuS47pXVVogJZ8hqwj-IpH5K3JQR35Xnw";
                _client = new Client(url, key);
                _client.InitializeAsync().GetAwaiter().GetResult(); // Szinkron módon megvárjuk
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Supabase initialization failed: {ex.Message}");
                throw; // Vagy kezeld másképp
            }
        }

        // Opcionális: Aszinkron inicializálás, ha később szeretnéd használni
        public static async Task InitializeAsync()
        {
            var url = "https://wctvnxubclukdhgrzwkp.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndjdHZueHViY2x1a2RoZ3J6d2twIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTkxNTg5NTIsImV4cCI6MjA3NDczNDk1Mn0.x-yBvv0yf6LuS47pXVVogJZ8hqwj-IpH5K3JQR35Xnw";
            _client = new Client(url, key);
            await _client.InitializeAsync();
        }
    }
}
