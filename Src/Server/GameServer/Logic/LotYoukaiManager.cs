using Puniemu.Src.Server.GameServer.DataClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puniemu.Src.Server.GameServer.Logic;

using BefriendYokaiInfo = (RarityType Rank, int SoultLevel);
public static class LotYoukaiManager
{
    private static readonly Random _rng = new Random();

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

        Befriender soultimate boost formula (derived from Robbett's data):
        boost = 1 + BaseK × effectiveActivations^0.631
        where effectiveActivations = sum(k[slot] / BaseK × levelScale[slot] × activations[slot])
        Anchored at A-rank level 7: 3 activations = 1.5x, 9 activations = 2.0x → BaseK=0.25, p=0.631
        k scales with befriender rank — higher rank befrienders are better:
            E: 0.10, D: 0.15, C: 0.18, B: 0.22, A: 0.25, S: 0.35, SS: 0.50
        Soultimate level scales k linearly:
            Level 1 = 20% of k, Level 7 = 100% of k
            levelScale = 0.20 + (0.80 × (level - 1) / 6)
    */

    //base befriend rates per enemy yokai rank
    public static readonly Dictionary<RarityType, float> BaseRateByRank = new()
    {
        {RarityType.RarityE, 0.08f },
        {RarityType.RarityD, 0.07f },
        {RarityType.RarityC, 0.06f },
        {RarityType.RarityB, 0.04f },
        {RarityType.RarityA, 0.02f },
        {RarityType.RarityS, 0.01f },
        {RarityType.RaritySS, 0.10f } //pass battles should get their odds overriden in the stage_data
    };

    //0=no food, 1=1-heart, 2=2-heart, 3=3-heart, 4=4-heart
    private static readonly float[] FoodMultipliers = new float[]
    {
        1.00f,  // bit 0 — no food,   base × 1.00
        1.50f,  // bit 1 — 1-heart,   base × 1.50  
        1.75f,  // bit 2 — 2-heart,   base × 1.75
        2.00f,  // bit 3 — 3-heart,   base × 2.00 
        4.25f,  // bit 4 — 4-heart,   base × 4.25  
    };

    // k value per befriender rank — all values anchored at soultimate level 7
    // Higher rank befrienders = higher k = better boost per activation
    private static readonly Dictionary<RarityType, float> BefrienderKByRank = new()
    {
        { RarityType.RarityE,  0.10f },
        { RarityType.RarityD,  0.15f },
        { RarityType.RarityC,  0.18f },
        { RarityType.RarityB,  0.22f },
        { RarityType.RarityA,  0.25f },
        { RarityType.RarityS,  0.35f },
        { RarityType.RaritySS, 0.50f },
    };

    private const float SoultimateBoostExponent = 0.631f;
    private const float BaseK = 0.25f; // A- rank level 7 anchor from robbet video

    // Scales k by soultteatr level
    // Level 1 = 20% of k, Level 7 = 100% of k
    private static float GetLevelScale(int soultimateLevel)
    {
        int clampedLevel = Math.Clamp(soultimateLevel, 1, 7);
        return 0.20f + (0.80f * (clampedLevel - 1) / 6f);
    }

    // Calculates the combined soultimate boost across all befriender slots
    // Each slot's activations are weighted by its rank's k relative to A-rank (BaseK),
    // then summed into a single effectiveActivations value before applying the exponent once.
    private static float GetSoultimateBoost(string pattern, BefriendYokaiInfo[] befrienders)
    {
        float effectiveActivations = 0f;
        for (int i = 0; i < pattern.Length; i++)
        {
            int activations = pattern[i] - '0';
            if (activations == 0 || befrienders[i].Rank == RarityType.RarityNone) continue;
            float relativeStrength = BefrienderKByRank[befrienders[i].Rank] / BaseK;
            float levelScale = GetLevelScale(befrienders[i].SoultLevel);
            effectiveActivations += relativeStrength * levelScale * activations;
        }
        if (effectiveActivations == 0f) return 1f;
        return 1f + BaseK * MathF.Pow(effectiveActivations, SoultimateBoostExponent);
    }

    private static float GetBefriendWeight(RarityType enemyRank, int bitPosition, float soultimateBoost)
    {
        float baseRate = BaseRateByRank[enemyRank];
        float foodMultiplier = FoodMultipliers[bitPosition];
        return Math.Clamp(baseRate * foodMultiplier * soultimateBoost, 0f, 1f);
    }

    private static string GenerateLotResult(RarityType enemyRank, float soultimateBoost)
    {
        var sb = new StringBuilder(5);
        for (int bitPos = 0; bitPos < 5; bitPos++)
        {
            float weight = GetBefriendWeight(enemyRank, bitPos, soultimateBoost);
            sb.Append(_rng.NextDouble() < weight ? '1' : '0');
        }
        return sb.ToString();
    }

    // Generates 4^numBefrienders lotPatterns
    // befriender[i].Rank == RarityNone means no befriender in that slot
    private static List<string> GenerateLotPatterns(BefriendYokaiInfo[] befrienders)
    {
        int befrienderCount = 0;
        foreach (var b in befrienders)
            if (b.Rank != RarityType.RarityNone) befrienderCount++;

        var patterns = new List<string>();
        int total = (int)Math.Pow(4, befrienderCount);

        for (int i = 0; i < total; i++)
        {
            var slots = new int[5];
            int remaining = i;

            for (int slot = befrienders.Length - 1; slot >= 0; slot--)
            {
                if (befrienders[slot].Rank == RarityType.RarityNone) continue;
                slots[slot] = remaining % 4;
                remaining /= 4;
            }

            var sb = new StringBuilder(5);
            foreach (int s in slots)
                sb.Append(s);
            patterns.Add(sb.ToString());
        }

        return patterns;
    }

   
    public static LotYoukaiInfoList GenerateLotYoukai(BefriendYokaiInfo[] befrienders, RarityType enemyRank)
    {
        var patterns = GenerateLotPatterns(befrienders);
        var lotList = new LotYoukaiInfoList();

        foreach (var pattern in patterns)
        {
            float soultimateBoost = GetSoultimateBoost(pattern, befrienders);
            lotList.Entries.Add(new LotYoukaiInfo
            {
                LotPattern = pattern,
                LotResult = GenerateLotResult(enemyRank, soultimateBoost)
            });
        }

        return lotList;
    }
}