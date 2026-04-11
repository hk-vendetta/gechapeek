# Oathsworn: Compact — Gacha System Player Guide

## Overview

The gacha summoning system is the primary way to acquire new playable units. It's designed to be rewarding and fair: you'll always be guaranteed a featured unit within a reasonable number of pulls, and every pull counts toward your progression.

In Oathsworn: Compact, summoning is also part of the world's lore: Prisms preserve oath-patterns of fallen defenders, and compatible Oath-Weavers call those defenders back into battle. In player-facing summon UI, these summon vessels are referred to as **Oath-Prisms**.

### Terminology Convention

- **Prism**: Canon infrastructure and lore term (vaults, nodes, attunement systems)
- **Oath-Prism**: Standard summon object shown in summon UI
- **Covenant Prism**: Premium/event-tier variant used for featured ceremonial reveals
- **Compact Seals**: Standard summon currency shown in UI (internal code field: `gachaTokens`)
- **High Compact Seals**: Premium/event currency used for archive/shop-style progression (internal code field: `archiveTokens`)

---

## Lore Context (Why Summoning Exists)

- **Prisms** are crystallized memory-lattices tied to oath signatures.
- **Oath-Weavers** are required to safely manifest those signatures as playable allies.
- Pulling a unit is not just a store action in this world, it is a compact being reactivated.

This is why the summon system is central to both roster progression and story identity.

---

## How Pulls Work

### Attunement + Reveal Presentation (Current)

Summons now use a staged reveal sequence tied to attunement UI flow:

1. **Attunement Screen** displays active banner info (name, description, featured unit summary, and rate summary).
2. On summon, attunement UI hides and the reveal overlay starts.
3. **Featured Prism Phase** shows the rarest pulled unit first:
   - Tap to unseal
   - Prism cracks
   - Unit reveal
   - Tap to continue
4. **Prism Grid Phase** shows all pulled units:
   - Featured unit appears already revealed in slot 0
   - Remaining prisms auto-crack in sequence
5. Close returns to the attunement screen.

If your pull has only one unit, the sequence can skip directly to close after featured reveal.

### Basic Pull

Pull cost depends on the banner:

| Banner Type | Single Pull | 10-Pull |
|-------------|------------|--------|
| Standard | **100 Compact Seals** | **1,000 Compact Seals** |
| Featured | **200 Compact Seals** | **2,000 Compact Seals** |

Every 10-pull includes a **guaranteed Tier 4+ unit** in that set (minimum one high-rarity pull per 10).

### Rarity Tiers

Units are ranked by rarity from Oath-Called to Oath-Eternal:

| Tier | Name | Drop Rate | Notes |
|------|------|-----------|-------|
| 1 | Oath-Called | 15% | Gray/white border |
| 2 | Oath-Sworn | 30% | Green border |
| 3 | Oath-Tempered | 37% | Blue border |
| 4 | Oath-Forged | 15% | Purple border |
| 5 | Oath-Eternal | 3% | Gold border |

On **featured banners**, the Tier 5 rate splits:
- **1.5% Oath-Eternal (featured unit)** — the new unit being promoted
- **1.5% Oath-Eternal (pool)** — any other Tier 5 unit

---

## Pity System

Pity guarantees prevent endless bad luck. There are two types:

### Soft Pity (Pull 61–79)

After 60 pulls without a Tier 5 unit, your chances increase:
- Pull 61: +0.5% to Tier 5 rate
- Pull 62: +1.0% total boost
- Pull 70: +5.0% total boost
- Pull 79: +9.5% total boost

**You should expect your Tier 5 between pulls 61–75 on average.**

### Hard Pity (Pull 80)

If you haven't gotten a Tier 5 by pull 80, **pull 81 is guaranteed to be Tier 5**. This is your safety net.

On featured banners, hard pity at pull 80 gives you **50% chance of the featured unit**, 50% chance of any other Tier 5.

---

## Spark System (Featured Banners Only)

Spark is a **guaranteed featured unit system** that prevents infinite bad luck on new banners.

### How Spark Works

- **Every pull gives 1 Spark Point**, regardless of result.
- **At 160 Spark Points**, you can claim the featured unit directly.
- Spark is **per-banner**: if you switch banners, your spark resets (but converts to Archive Tokens).
- Spark **does NOT reset** when you pull a featured unit normally (you keep accumulating toward the next one).

### Example: Featured Banner Timeline

```
Pull 1–30:   30 spark accumulated
Pull 31:     Get featured unit via luck!
Pull 32–80:  50 more spark (80 total)
Pull 81–160: Accumulate remaining 80 spark
At 160 spark: CLAIM featured unit guaranteed
```

Even though you got the featured unit at pull 31, your spark counter kept going, and at 160 you earn a second copy.

### Spark Threshold by Banner

- **Featured Banners**: 160 spark = claim featured unit
- **Standard Banner**: No spark system (no featured unit to claim)
- **Reruns**: Same as featured (160 spark)

---

## High Compact Seals

High Compact Seals are **permanent premium currency** earned when banners end.

### How to Get High Compact Seals

1. **Banner Rotation**: When a featured banner ends, unspent spark points convert to High Compact Seals (1 spark = 1 seal by default).
2. **Event Rewards**: Special events sometimes grant High Compact Seals as milestone rewards.

### What to Buy with High Compact Seals

High Compact Seals are spent in the **Archive Shop** to purchase:

| Item | Cost | Rarity |
|------|------|--------|
| Random Tier 4 Unit Ticket | 120 tokens | Oath-Forged |
| Universal Ascension Core | 300 tokens | Mid-tier material |
| Tier 5 Standard Selector | 600 tokens | Any non-featured Tier 5 |
| Previous Featured Unit Selector | 900 tokens | Featured unit from 8+ weeks ago |

**High Compact Seals never expire.** Save them for units you really want.

---

## Duplicate System

Getting a unit you already own isn't wasted — it converts into **Prism Shards** tied to that specific unit.

### How Shards Work

Shards are **per-unit**. Shards earned from a Lyria duplicate can only be used to upgrade Lyria — they're not pooled across units. Every duplicate of a unit you're building is a direct step toward unlocking their full potential.

### Shard Payout by Rarity

| Tier | Standard Unit | Featured Unit |
|------|---------------|---------------|
| 1 (Oath-Called) | 1 shard | 1 shard |
| 2 (Oath-Sworn) | 3 shards | 3 shards |
| 3 (Oath-Tempered) | 10 shards | 10 shards |
| 4 (Oath-Forged) | 25 shards | 25 shards |
| 5 (Oath-Eternal) | 80 shards | **120 shards** |

Featured Tier 5 dupes give a **+40 shard bonus** (total 120).

### What to Do with Shards — Rarity Tier Upgrades

Spend unit-specific shards in the Unit Enhancement screen to upgrade that unit's rarity tier:

| Upgrade | Shards Required |
|---------|----------------|
| Oath-Called → Oath-Sworn (1→2) | 20 |
| Oath-Sworn → Oath-Tempered (2→3) | 40 |
| Oath-Tempered → Oath-Forged (3→4) | 60 |
| Oath-Forged → Oath-Eternal (4→5) | 80 |

Upgrading rarity increases base stats and unlocks new skills at the higher tier. Each unit has a maximum rarity cap defined by their data — not all units can reach Oath-Eternal. **Full upgrade from tier 1 to 5 costs 200 shards total.**

---

## Compact Seal Income (F2P Expectations)

Free players typically earn **11,000–12,000 Compact Seals per 4-week cycle**:

- **Daily login**: 80 × 28 days = 2,240 Compact Seals
- **Weekly challenges**: 500 × 4 weeks = 2,000 Compact Seals
- **Story missions**: ~3,000 Compact Seals (first clear bonuses)
- **Event milestones**: ~2,200 Compact Seals
- **Special gifts**: ~1,600 Compact Seals

**Total per cycle (approximate pulls)**:

| Banner | Pulls per cycle |
|--------|----------------|
| Standard (100/pull) | ~110–120 pulls |
| Featured (200/pull) | ~55–60 pulls |

### What This Means

- **Dedicated Free Player (Featured)**: Can approach hard pity (~80 pulls) in roughly 5–6 weeks on a featured banner
- **Standard Banner**: Many more pulls per Compact Seal, good for building unit depth
- **Budget Spenders**: Consider splitting Compact Seals — standard for breadth, featured for target units

---

## Featured Banner Rotation (Weekly Schedule)

Each 4-week cycle:

1. **Week 1**: New featured banner launches
   - New mission area + new featured unit
   - First discounted 10-pull available
   
2. **Week 2**: Challenge stages open
   - Trial stages let you test-drive the featured unit
   - Bonus currency rewards
   
3. **Week 3**: Side story + rerun
   - Second featured unit banner (support/antagonist from story)
   - Week 1 featured banner still active for 5 more days
   
4. **Week 4**: Archive shop refresh
   - Finalize spark claims
   - Convert remaining spark to Archive Tokens
   - Teaser for next chapter

---

## Tips for New Players

1. **Don't pull blindly on Standard Banner** — Featured banners guarantee the new unit within 160 pulls. Standard banner doesn't.

2. **Plan your pulls** — Check the featured unit's abilities before pulling. New units are tailored to new content but not required to clear it.

3. **Save your spark** — If you're not interested in the current featured unit, skip pulling and wait for the next banner.

4. **Use Archive Tokens wisely** — They never expire, so you can wait for a unit you really want across multiple cycles.

5. **Soft pity at 61** — You don't HAVE to pull all 80. Once you're at pull 61, each additional pull has a much higher chance of Tier 5. Consider stopping if budget is tight.

6. **Duplicate Tier 5s are valuable** — Getting a second copy isn't bad luck; it gives you 120 shards toward premium items in the Archive Shop.

---

## FAQ

**Q: Can I lose my pity counter if I switch banners?**
A: No. Pity counters are **per-banner-type** (featured vs. standard). Switching between two featured banners resets pity (but converts spark to Archive Tokens).

**Q: Does spark reset when I claim a featured unit?**
A: No. Spark keeps accumulating. You can earn two featured units from one banner if you reach 160 spark.

**Q: What happens to my spark if the banner ends?**
A: It converts to Archive Tokens at a 1:1 rate. No loss.

**Q: Can I get a Tier 3 in my 10-pull guarantee?**
A: No. The "10-pull guarantee" means **at least one Tier 4 or higher** in every 10 pulls.

**Q: Are featured banner rates better than Standard?**
A: Yes. Featured banners split the 3% Tier 5 rate (1.5% featured, 1.5% others). Standard has 3% any Tier 5, but no featured unit to target.

**Q: Do I need the featured unit to complete new content?**
A: No. New missions are balanced so both new and old units can clear them. Featured units are faster/easier but optional.

**Q: Why do I see different summon terms like Prism, Oath-Prism, and Covenant Prism?**
A: They refer to the same summon infrastructure at different presentation tiers. Standard UI commonly uses Oath-Prism naming, while featured ceremonial presentation can use Covenant Prism wording.

---

*Last Updated: April 8, 2026*
