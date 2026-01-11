using System.Text.Json;

namespace SmartFitness
{
    

public class AppConfig
    {
        public required string SupabaseUrl { get; set; }
        public required string SupabaseAnonKey { get; set; }

        public static AppConfig Load()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();


            // ensure deserialization result is not null
            return JsonSerializer.Deserialize<AppConfig>(content)
                   ?? throw new InvalidOperationException("Failed to deserialize AppConfig from appsettings.json.");
        }
    }

}
