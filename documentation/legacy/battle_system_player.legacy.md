# Battle System — Player Guide

## Overview

Every dungeon mission puts your active party of up to **6 units** against enemy waves. Combat is turn-based, but your player phase is now **manual and order-driven**:

- First, queue actions for units.
- Then, execute those queued actions one by one in the order you choose.
- Enemies only take their phase after your living units have finished acting (or are intentionally skipped).

This rewards timing, chaining, smart target selection, and now **Sigil Weaving**.

---

## Your Party

### Building a Party

- You have up to **3 named party slots**.
- Each party can now hold up to **6 units**.
- Pick your active party before launching a mission.

### Unit Stats

| Stat | What It Does |
|------|-------------|
| **HP** | Total health. Reaching 0 knocks the unit out. |
| **MP** | Resource used by many magic and skill actions. |
| **ATK** | Physical offense for attacks and physical skills. |
| **DEF** | Reduces incoming physical damage. |
| **MAG** | Magic offense for magical skills/spells. |
| **SPR** | Reduces incoming magic damage and scales healing power received from SPR-based healing formulas. |
| **SPD** | Reserved for future initiative systems. |
| **CRIT** | Critical chance stat (future expansion). |

Equipment contributes directly to these effective combat stats.

---

## Turn Flow (Current)

## 1) Queue Phase

For each unit card, you choose an action and queue it:

1. **Attack**
2. **Skill**
3. **Magic**
4. **Weave** (Oath-Weaver only, if mission charges remain)
5. **Defend**

Queued cards show a queued visual state so you can see who is ready.

## 2) Execute Phase

Tap queued cards again to execute actions deliberately, one by one. This is where pacing and input timing matter for chain optimization.

## 3) Enemy Phase

After your eligible units have acted, enemies perform their turn.

Important behavior:

- A unit blocked by an action-preventing status does **not** automatically lose its turn.
- If you cleanse that status during the same player phase, that unit can still act afterward.

---

## Targeting and Battlefield Controls

- Battles use fixed battlefield slot positions for both sides.
- You can tap enemy sprites directly to set target selection.
- Small above-head indicators show the currently selected target.
- Target indicators are cleared at appropriate action/turn transitions to keep targeting readable.
- Player movement before attack is now skill-dependent: some skills move the unit in, others stay in place.
- Enemy visuals remain stationary in the current presentation.
- For single-target weaves, selecting an enemy queues the weave and closes the weave panel automatically.

---

## Sigil Resonance and Oath-Weaving

The Sigil system is now active in battle and has two player-facing layers:

### 1) Passive Resonance (Automatic)

- At battle start, compatible sigil pairings in your party grant **small flat stat bonuses** (ATK/DEF/MAG/SPR).
- This is automatic and mission-scoped.
- Incompatible sigil pairings currently do nothing.

### 2) Weave Actions (Active)

- Oath-Weavers can access a dedicated **Weave** panel from the skill menu.
- Weaves are **mission-limited** (base 1 per mission, plus future progression bonuses).
- Weave entries show preview data before queueing:
	- Target type
	- Physical or magical classification
	- Power profile
	- Status payload (if any)

Single-target behavior:
- Pick the weave.
- Tap an enemy.
- The weave queues and the panel closes.

AoE/all-target behavior:
- Queues immediately with no target selection step.

### Codex Unlock

- The Resonance Codex unlocks after Chapter 1 Mission 006 story progression (Scene 6B timing).
- Discovered weaves and codex unlock state are saved to profile data.

---

## Chain Combo System (Frame-Based)

Chain progression is now based on **actual hit timing** rather than broad per-action windows.

- Multi-hit attacks can add multiple chain steps quickly.
- Tight hit spacing between different units improves chain carry.
- Chain family is treated as a frame/timing compatibility identity.
- There are no separate family bonus layers; timing execution is what matters.

Practical takeaway: fast, clean execution and compatible hit timing produce stronger chains.

---

## Damage and Healing

### Damage

- Physical damage scales primarily from attacker ATK versus target DEF.
- Magical damage scales primarily from attacker MAG versus target SPR.
- Chain multiplier amplifies hit damage as chain count rises.

### Healing

- Healing skills and healing magic are SPR-aware.
- Effective SPR contributes to healing outcome, making SPR-focused builds stronger sustain options.

---

## Status Effects

Status effects are now active in battle and can be:

- Inflicted by abilities or magic.
- Inflicted by equipped weapons (chance-based on hit).
- Cleansed by eligible skills, magic, or items.

Effects include action denial and other buffs/debuffs depending on effect type.

UI support:

- Unit cards display status badges.
- Hovering a badge shows the full effect name for quick readability.

---

## Items in Battle

Combat item effects now support:

- Healing HP
- Revive behaviors (where defined)
- Status cleanse effects

This allows tactical recovery and cleanse sequencing inside the same player phase.

---

## Victory and Defeat

- **Victory**: all enemies are defeated.
- **Defeat**: all player units are down.

As long as at least one unit is still standing, the battle continues.

---

## Practical Tips

- Queue your whole plan first, then execute in the exact order that best extends chain timing.
- Use multi-hit units early to establish chain rhythm for heavier finishers.
- Prioritize cleanse actions if key units are action-locked; those units can still act later in that same phase.
- Build at least one SPR-focused support unit to improve healing efficiency.
- Use direct sprite targeting to avoid accidental target drift.
- Save your weave charge for high-impact turns (boss threshold, emergency cleanse/heal swing, or turn-control setup).
- If a single-target weave does not queue, make sure an enemy is selected first.

---

## FAQ

**Q: Is battle still strictly one action then enemy turn?**
No. You now queue and manually execute player actions first; enemy phase comes after player phase completes.

**Q: Are weaves unlimited each battle?**
No. Weaves are mission-limited. Current baseline is one weave per mission, with room for progression-based extra charges.

**Q: Do all units get Weave?**
No. Only Oath-Weaver units can weave.

**Q: Can statuses be removed in combat now?**
Yes. Status cleanse is supported via configured skills, magic, and items.

**Q: Can healing scale from SPR now?**
Yes. Healing logic now includes SPR scaling support.

**Q: Is party size still 5?**
No. Active party battle capacity is now 6.
