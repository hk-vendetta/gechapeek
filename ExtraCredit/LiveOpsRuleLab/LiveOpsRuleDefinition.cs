using System;
using System.Collections.Generic;

[Serializable]
public class LiveOpsRuleDefinition
{
    public string ruleId;
    public string displayName;
    public bool enabled;

    // ISO 8601 UTC timestamps. Example: 2026-04-10T18:00:00Z
    public string startUtc;
    public string endUtc;

    // Audience filters.
    public int minPlayerLevel;
    public int maxPlayerLevel;
    public bool allowAllSegments;
    public bool allowWeekends;
    public List<string> includedSegments;
    public List<string> excludedSegments;
    public List<string> allowedRegions;

    public string notes;

    public static LiveOpsRuleDefinition CreateDefault()
    {
        return new LiveOpsRuleDefinition
        {
            ruleId = Guid.NewGuid().ToString("N").Substring(0, 8),
            displayName = "Double Drop Weekend",
            enabled = true,
            startUtc = DateTime.UtcNow.ToString("yyyy-MM-ddT00:00:00Z"),
            endUtc = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-ddT23:59:59Z"),
            minPlayerLevel = 1,
            maxPlayerLevel = 999,
            allowAllSegments = true,
            allowWeekends = true,
            includedSegments = new List<string>(),
            excludedSegments = new List<string>(),
            allowedRegions = new List<string>(),
            notes = string.Empty,
        };
    }
}

[Serializable]
public class LiveOpsAudienceProfile
{
    public string playerId;
    public int playerLevel;
    public string region;
    public List<string> segments;

    public static LiveOpsAudienceProfile CreateExample()
    {
        return new LiveOpsAudienceProfile
        {
            playerId = "player_001",
            playerLevel = 20,
            region = "US",
            segments = new List<string> { "spender", "returning" },
        };
    }
}

[Serializable]
public class LiveOpsSimulationResult
{
    public string evaluatedAtUtc;
    public bool passedValidation;
    public bool passesSchedule;
    public bool passesAudience;
    public bool isEligible;
    public List<string> messages;

    public LiveOpsSimulationResult()
    {
        messages = new List<string>();
    }
}
