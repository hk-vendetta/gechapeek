using System;
using System.Collections.Generic;

public static class LiveOpsRuleValidator
{
    public static List<string> Validate(LiveOpsRuleDefinition rule)
    {
        var errors = new List<string>();

        if (rule == null)
        {
            errors.Add("Rule is null.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(rule.ruleId))
            errors.Add("Rule ID is required.");

        if (string.IsNullOrWhiteSpace(rule.displayName))
            errors.Add("Display Name is required.");

        if (!TryParseUtc(rule.startUtc, out DateTime startUtc))
            errors.Add("Start UTC must be a valid ISO 8601 UTC timestamp.");

        if (!TryParseUtc(rule.endUtc, out DateTime endUtc))
            errors.Add("End UTC must be a valid ISO 8601 UTC timestamp.");

        if (errors.Count == 0 && endUtc < startUtc)
            errors.Add("End UTC must be greater than or equal to Start UTC.");

        if (rule.minPlayerLevel < 1)
            errors.Add("Minimum player level must be at least 1.");

        if (rule.maxPlayerLevel < rule.minPlayerLevel)
            errors.Add("Maximum player level must be greater than or equal to Minimum player level.");

        return errors;
    }

    public static LiveOpsSimulationResult Simulate(
        LiveOpsRuleDefinition rule,
        LiveOpsAudienceProfile profile,
        DateTime evaluationUtc)
    {
        var result = new LiveOpsSimulationResult
        {
            evaluatedAtUtc = evaluationUtc.ToString("o"),
        };

        List<string> validationErrors = Validate(rule);
        result.passedValidation = validationErrors.Count == 0;

        if (!result.passedValidation)
        {
            result.messages.AddRange(validationErrors);
            result.passesSchedule = false;
            result.passesAudience = false;
            result.isEligible = false;
            return result;
        }

        if (!rule.enabled)
            result.messages.Add("Rule is disabled.");

        DateTime startUtc = DateTime.Parse(rule.startUtc).ToUniversalTime();
        DateTime endUtc = DateTime.Parse(rule.endUtc).ToUniversalTime();

        bool withinWindow = evaluationUtc >= startUtc && evaluationUtc <= endUtc;
        result.passesSchedule = withinWindow;

        if (!withinWindow)
            result.messages.Add("Evaluation time is outside the active time window.");

        bool weekendAllowed = rule.allowWeekends ||
            (evaluationUtc.DayOfWeek != DayOfWeek.Saturday && evaluationUtc.DayOfWeek != DayOfWeek.Sunday);

        if (!weekendAllowed)
            result.messages.Add("Rule blocks weekend activation for this evaluation time.");

        bool levelPass = profile != null &&
            profile.playerLevel >= rule.minPlayerLevel &&
            profile.playerLevel <= rule.maxPlayerLevel;

        if (!levelPass)
            result.messages.Add("Player level is outside the configured range.");

        bool regionPass = rule.allowedRegions == null ||
            rule.allowedRegions.Count == 0 ||
            ContainsIgnoreCase(rule.allowedRegions, profile != null ? profile.region : string.Empty);

        if (!regionPass)
            result.messages.Add("Player region is not in the allowed region list.");

        bool segmentPass = true;
        if (!rule.allowAllSegments)
        {
            bool hasIncludedSegment = rule.includedSegments == null || rule.includedSegments.Count == 0;
            if (profile != null && profile.segments != null)
            {
                for (int i = 0; i < profile.segments.Count; i++)
                {
                    string segment = profile.segments[i];
                    if (ContainsIgnoreCase(rule.includedSegments, segment))
                        hasIncludedSegment = true;

                    if (ContainsIgnoreCase(rule.excludedSegments, segment))
                    {
                        segmentPass = false;
                        result.messages.Add($"Player is in excluded segment: {segment}");
                    }
                }
            }

            if (!hasIncludedSegment)
            {
                segmentPass = false;
                result.messages.Add("Player does not match any included segment.");
            }
        }

        result.passesAudience = levelPass && regionPass && segmentPass;
        result.isEligible = rule.enabled && result.passesSchedule && weekendAllowed && result.passesAudience;

        if (result.isEligible)
            result.messages.Add("Simulation passed: this player would see the event or offer.");

        return result;
    }

    private static bool TryParseUtc(string value, out DateTime utc)
    {
        utc = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!DateTime.TryParse(value, out DateTime parsed))
            return false;

        utc = parsed.ToUniversalTime();
        return true;
    }

    private static bool ContainsIgnoreCase(List<string> values, string candidate)
    {
        if (values == null || string.IsNullOrWhiteSpace(candidate))
            return false;

        for (int i = 0; i < values.Count; i++)
        {
            if (string.Equals(values[i], candidate, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
