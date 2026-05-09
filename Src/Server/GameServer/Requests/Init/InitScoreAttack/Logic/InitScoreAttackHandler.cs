using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puniemu.Src.Server.GameServer.DataClasses;
using Puniemu.Src.Server.GameServer.Requests.InitScoreAttack.DataClasses;
using Puniemu.Src.Utils.GeneralUtils;

namespace Puniemu.Src.Server.GameServer.Requests.InitScoreAttack.Logic
{
    public static class InitScoreAttackHandler
    {
        public static async Task HandleAsync(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            var readResult = await ctx.Request.BodyReader.ReadAsync();
            var encRequest = System.Text.Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            ctx.Request.BodyReader.AdvanceTo(readResult.Buffer.End);
            
            // Décryptage de la requête
            var requestJsonString = NHNCrypt.Logic.NHNCrypt.DecryptRequest(encRequest);
            var req = JsonConvert.DeserializeObject<InitScoreAttackRequest>(requestJsonString!);

            var res = new InitScoreAttackResponse();
            var resdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(res))!;
            
            // Tables de base
            List<string> tables = new();
            tables.Add("ywp_user_data");
            tables.Add("ywp_user_score_attack_reward");
            tables.Add("ywp_mst_score_attack_league");
            tables.Add("ywp_mst_score_attack_reward");
            tables.Add("ywp_mst_score_attack_item");
            tables.Add("ywp_mst_score_attack");
            tables.Add("ywp_mst_big_boss");
            
            res.ResponseCode = 0;
            
            // Charger les tables de base
            await GeneralUtils.AddTablesToResponse(tables, resdict!, false, req!.Level5UserId!);
            
            try
            {
                // Calculer weekSeq basé sur la date actuelle
                var currentWeek = GetCurrentWeekSeq();
                resdict["weekSeq"] = currentWeek;
                
                // Déterminer la ligue du joueur (défaut: Bronze = 5)
                var userLeague = await DetermineUserLeague(req.Level5UserId!);
                resdict["leagueId"] = userLeague;
                
                Console.WriteLine($"[InitScoreAttack] User {req.Level5UserId} - WeekSeq: {currentWeek}, LeagueId: {userLeague}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InitScoreAttack] Error processing for {req?.Level5UserId}: {ex}");
                // Valeurs par défaut en cas d'erreur
                resdict["weekSeq"] = 202538;
                resdict["leagueId"] = 5;
            }

            var marshalledResponse = JsonConvert.SerializeObject(resdict);
            var encryptedResponse = NHNCrypt.Logic.NHNCrypt.EncryptResponse(marshalledResponse);
            await ctx.Response.WriteAsync(encryptedResponse);
        }

        // Calculer le numéro de semaine basé sur la date actuelle
        private static int GetCurrentWeekSeq()
        {
            try
            {
                // Base: semaine du 1er janvier 2020 = 200001
                var baseDate = new DateTime(2020, 1, 1);
                var currentDate = DateTime.UtcNow;
                
                // Calculer le nombre de semaines depuis la date de base
                var daysDiff = (currentDate - baseDate).Days;
                var weeksDiff = daysDiff / 7;
                
                // Format: YYYYWW (année + numéro de semaine)
                var year = currentDate.Year;
                var weekOfYear = (currentDate.DayOfYear - 1) / 7 + 1;
                
                return year * 100 + Math.Min(weekOfYear, 53);
            }
            catch
            {
                // Valeur par défaut si le calcul échoue
                return 202538;
            }
        }

        // Déterminer la ligue du joueur basé sur ses stats
        private static async Task<int> DetermineUserLeague(string gdkey)
        {
            try
            {
                // Récupérer les données utilisateur
                var userData = await DBService.Logic.DBService.GetYwpUserAsync<YwpUserData>(gdkey, "ywp_user_data");
                if (userData == null) return 5; // Bronze par défaut
                
                // Calculer le score total du joueur
                var (totalStars, totalScore) = await ComputePlayerTotalScore(gdkey);
                
                // Logique de détermination de ligue basée sur le score/étoiles
                if (totalStars >= 1000 || totalScore >= 50000000) return 1; // Diamond
                if (totalStars >= 800 || totalScore >= 30000000) return 2;  // Platinum  
                if (totalStars >= 600 || totalScore >= 15000000) return 3;  // Gold
                if (totalStars >= 400 || totalScore >= 5000000) return 4;   // Silver
                
                return 5; // Bronze
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DetermineUserLeague] Error for {gdkey}: {ex}");
                return 5; // Bronze par défaut
            }
        }

        // Calculer le score total du joueur
        private static async Task<(int totalStars, long totalScore)> ComputePlayerTotalScore(string gdkey)
        {
            try
            {
                // Lire les données de stage
                var stageRaw = await DBService.Logic.DBService.GetYwpUserAsync<string>(gdkey, "ywp_user_stage");
                if (string.IsNullOrEmpty(stageRaw))
                {
                    var stageList = await DBService.Logic.DBService.GetYwpUserAsync<List<string>>(gdkey, "ywp_user_stage");
                    if (stageList == null || stageList.Count == 0) return (0, 0);
                    stageRaw = string.Join("*", stageList);
                }

                int totalStars = 0;
                long totalScore = 0;
                var entries = stageRaw.Split('*', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var entry in entries)
                {
                    var parts = entry.Split('|');
                    if (parts.Length >= 6)
                    {
                        int s1 = int.TryParse(parts[2], out var t1) ? t1 : 0;
                        int s2 = int.TryParse(parts[3], out var t2) ? t2 : 0;
                        int s3 = int.TryParse(parts[4], out var t3) ? t3 : 0;
                        totalStars += s1 + s2 + s3;

                        long score = long.TryParse(parts[5], out var sc) ? sc : 0;
                        totalScore += score;
                    }
                }

                return (totalStars, totalScore);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ComputePlayerTotalScore] Error for {gdkey}: {ex}");
                return (0, 0);
            }
        }
    }
}