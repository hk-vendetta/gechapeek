# LiveOpsRuleLab — Event & Offer Simulator

**Ship live events with confidence before they ever hit production.**

LiveOpsRuleLab is an Editor tool for Unity teams building timed events, segmented offers, login campaigns, flash sales, progression gates, and other live-ops content. It lets you define a rule, simulate a player profile against it, and see instantly whether that player would receive the content.

The problem it solves is simple: most teams configure live-ops rules in spreadsheets, JSON, remote-config dashboards, or ad hoc ScriptableObjects, then hope the targeting behaves as expected in production. That is where costly mistakes happen — wrong region, wrong segment, wrong level band, expired window, weekend-only content showing on Monday, or a disabled rule that looked fine in review.

LiveOpsRuleLab gives designers and engineers a safe validation layer before the push.

---

## What it does

For each live-ops rule, the tool validates:

- Activation window (`startUtc`, `endUtc`)
- Enabled / disabled state
- Player level range
- Allowed region list
- Included and excluded audience segments
- Weekend gating

Then it simulates a preview player profile and returns a simple answer:

- **PASS** — this player would see the event or offer
- **BLOCKED** — this player would not receive it

It also explains *why* with human-readable messages.

---

## Key features

| Feature | Description |
|---|---|
| Rule authoring window | Define a rule directly in a Unity Editor window |
| Audience simulation | Enter a sample player and test eligibility instantly |
| Validation checks | Catch broken timestamps, inverted level ranges, or missing required fields |
| Segment targeting | Include / exclude player segments with simple comma-separated lists |
| Region gating | Restrict rules by country or territory code |
| Weekend block rule | Simulate rules that allow or disallow weekend activation |
| Save / load rule JSON | Persist rules to `Assets/LiveOpsRuleLabData/` for review and iteration |
| Pass / Block result | Clear color-coded output with an audit message list |

---

## Who it is for

- **Free-to-play teams** shipping timed events and segmented offers
- **Live-ops designers** who want to validate targeting before publishing content
- **Gameplay engineers** who need a quick sanity check during implementation
- **Producers and QA** reviewing event configurations before launch

---

## What it does NOT do

LiveOpsRuleLab is a simulation and validation tool. It does not connect to a live backend, publish remote config, push offers, or sync with third-party live-ops providers. It is intentionally local, fast, and review-friendly.

---

## Assets created by this tool

Saved rule files are written to `Assets/LiveOpsRuleLabData/` as JSON. No prefabs, scenes, or runtime content are generated.

---

## Suggested retail price: $24.99
