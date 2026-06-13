using Puniemu.Src.Server.GameServer.DataClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic;

using BefriendYokaiInfo = (long YoukaiID, int SoultLevel);
public static class LotYoukaiManager
{
    private static readonly Random _rng = Random.Shared;

    /*
        Data collected from Robbett watch amazing video on befriending
        _________________________________________________________________

        Normal shrine bonus: Does nothing
        Super shrine bonus: 100% increase (Does not stack with food, game takes the higher multiplier)
        
        E Rank baseline: 8%
        D Rank baseline: 7%
        C Rank baseline: 6%
        B Rank baseline: 4%
        A Rank baseline: 2%
        S Rank baseline: 1%
        SS Rank baseline: 10%
        
        1-heart food: ~50% increase in befriend rate
        2-heart food: ~75% increase in befriend rate
        3-heart food: ~100% increase in befriend rate
        4-heart food: ~325% increase in befriend rate

        Befriender boost formula is currently (Note: formula probably not accurate to the original game but it should be the same ceiling since its using the points from the MST): 
        
        ∏(i=0 to N-1) [1 + ((P - 0.006i) × 0.08) / 100]
        
        Where P is the amount of ingame points
        and N is the amount of soult used
    */

    // Base befriend rates per enemy yokai rank
    public static readonly Dictionary<RarityType, float> BaseRateByRank = new()
    {
        { RarityType.RarityE,   0.08f  },
        { RarityType.RarityD,   0.07f  },
        { RarityType.RarityC,   0.06f  },
        { RarityType.RarityB,   0.04f  },
        { RarityType.RarityA,   0.01f  },
        { RarityType.RarityS,   0.01f  },
        { RarityType.RaritySS,  0.10f  }, // pass battles should get their odds overridden in stage_data
    };

    // 0=no food, 1=1-heart, 2=2-heart, 3=3-heart, 4=4-heart
    private static readonly float[] FoodMultipliers = new float[]
    {
        1.00f,  // bit 0 — no food,   base x 1.00
        1.50f,  // bit 1 — 1-heart,   base x 1.50
        1.75f,  // bit 2 — 2-heart,   base x 1.75
        2.00f,  // bit 3 — 3-heart,   base x 2.00
        4.25f,  // bit 4 — 4-heart,   base x 4.25
    };

    private static readonly float SUPER_SHRINE_MULTIPLIER = FoodMultipliers[3];
    private static float GetSoultimateBoost(string pattern, BefriendYokaiInfo[] befrienders, float[] ptsArr)
    {
        double totalMultiplier = 1.0;
        for(int i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
            var useCount = (byte)(c - '0');
            const double BASE = 0.08;
            double percent = 0;
            for(int j = 0; j < useCount; j++)
            {
                percent += ptsArr[i] * (BASE - (0.006 * j));
            }
            double multiplier = 1 + (percent / 100);

            totalMultiplier *= multiplier;
        }

        return (float)totalMultiplier;
    }

    private static float GetBefriendWeight(RarityType enemyRank, int bitPosition, float soultimateBoost, bool isSuperShrine)
    {
        float baseRate = BaseRateByRank[enemyRank];
        float foodMultiplier = FoodMultipliers[bitPosition];
        var shrineMultiplier = 0.0f;
        var preClamp = baseRate * soultimateBoost;
        if (isSuperShrine) shrineMultiplier = SUPER_SHRINE_MULTIPLIER;
        preClamp *= Math.Max(shrineMultiplier, foodMultiplier);
        return Math.Clamp(preClamp, 0f, 1f);
    }

    private static string GenerateLotResult(RarityType enemyRank, float soultimateBoost, bool isSuperShrine)
    {
        var bits = new char[5];
        for (int bitPos = 0; bitPos < 5; bitPos++)
        {
            float weight = GetBefriendWeight(enemyRank, bitPos, soultimateBoost, isSuperShrine);
            bits[bitPos] = _rng.NextDouble() < weight ? '1' : '0';
        }
        return new string(bits);
    }

    // Generates 4^numBefrienders lotPatterns
    // befriender[i].Rank == RarityNone means no befriender in that slot
    private static List<string> GenerateLotPatterns(BefriendYokaiInfo[] befrienders)
    {
        int befrienderCount = 0;
        foreach (var b in befrienders)
            if (b.SoultLevel != 0) befrienderCount++;

        var activeSlots = new List<int>();
        for (int slot = 0; slot < befrienders.Length; slot++)
            if (befrienders[slot].SoultLevel != 0) activeSlots.Add(slot);

        var patterns = new List<string>();

        void Recurse(char[] current, int depth)
        {
            if (depth == activeSlots.Count)
            {
                patterns.Add(new string(current));
                return;
            }

            int index = activeSlots[depth];

            for (char c = '0'; c <= '3'; c++)
            {
                current[index] = c;
                Recurse(current, depth + 1);
            }

            current[index] = '0'; // restore
        }

        var chars = new string('0', befrienders.Length).ToCharArray();
        Recurse(chars, 0);

        return patterns;
    }
    private static float[] GetBefrienderPts(BefriendYokaiInfo[] befrienders)
    {
        const float PT_DIVISOR = 187.5f;
        var ptsArr = new float[befrienders.Length];
        for (int i = 0; i < befrienders.Length; i++)
        {
            if (befrienders[i].SoultLevel != 0)
                ptsArr[i] = (MstSkillLevelManager.GetEntry(befrienders[i].YoukaiID, befrienders[i].SoultLevel).GetBefrienderPt() / PT_DIVISOR);
            else ptsArr[i] = 0;
        }
        return ptsArr;
    }
    public static LotYoukaiInfoList GenerateLotYoukai(BefriendYokaiInfo[] befrienders, RarityType enemyRank, bool isSuperShrine)
    {
        var ptsArr = GetBefrienderPts(befrienders);
        var patterns = GenerateLotPatterns(befrienders);
        var lotList = new LotYoukaiInfoList();

        foreach (var pattern in patterns)
        {
            float soultimateBoost = GetSoultimateBoost(pattern, befrienders, ptsArr);
            lotList.Entries.Add(new LotYoukaiInfo
            {
                LotPattern = pattern,
                LotResult = GenerateLotResult(enemyRank, soultimateBoost, isSuperShrine)
            });
        }

        return lotList;
    }
}