using SmartFitness.Models;
using SmartFitness.Services;
using Supabase;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFitness.Services
{
    public static class ProgramService
    {
        private static Client Client => SupabaseClient.Client;

        public static async Task<ActiveProgramModel?> GetActiveProgramAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return null;

                Debug.WriteLine($"GetActiveProgram for userId={userId}");

                var response = await Client
                    .From<ActiveProgramModel>()
                    .Where(x => x.UserId == userId && x.IsActive == true)
                    .Get();

                var active = response.Models.FirstOrDefault();

                if (active != null)
                {
                    Debug.WriteLine($"Active program found: planId={active.PlanId}, start={active.StartDate}, end={active.EndDate}");
                }
                else
                {
                    Debug.WriteLine("No active program found.");
                }

                return active;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetActiveProgram ERROR: {ex}");
                return null;
            }
        }

        public static async Task StartProgramAsync(string userId, string planId, int weeks)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("userId is null or empty.", nameof(userId));

                if (string.IsNullOrWhiteSpace(planId))
                    throw new ArgumentException("planId is null or empty.", nameof(planId));

                if (weeks <= 0)
                    weeks = 3;

                Debug.WriteLine($"StartProgram userId={userId}, planId={planId}, weeks={weeks}");

                // Előző aktív program deaktiválása
                var existing = await Client
                    .From<ActiveProgramModel>()
                    .Where(x => x.UserId == userId && x.IsActive == true)
                    .Get();

                if (existing.Models.Any())
                {
                    Debug.WriteLine($" Deactivating {existing.Models.Count} active programs");

                    foreach (var ap in existing.Models)
                    {
                        ap.IsActive = false;
                        await Client.From<ActiveProgramModel>().Update(ap);
                    }
                }


                //  Új aktív program beszúrása
                var now = DateTime.UtcNow;
                var end = now.AddDays(7 * weeks);

                var activeProgram = new ActiveProgramModel
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    PlanId = planId,
                    StartDate = now,
                    EndDate = end,
                    IsActive = true
                };

                var insertResponse = await Client
                    .From<ActiveProgramModel>()
                    .Insert(activeProgram);

                Debug.WriteLine(" New active program inserted successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" StartProgramAsync ERROR: {ex}");
                throw;
            }
        }

        public static async Task DeactivateProgramAsync(string activeProgramId)
        {
            await SupabaseClient.Client
                .From<ActiveProgramModel>()
                .Where(x => x.Id == activeProgramId)
                .Set(x => x.IsActive, false)
                .Update();
        }

    }
}
