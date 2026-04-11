# Build Health Intelligence

**Build Health Intelligence** is a Unity Editor tool that gives you a clear, at-a-glance view of your project's technical health over time.

As a Unity project grows, it quietly accumulates hidden problems — extra assets that never get cleaned up, shaders that multiply without warning, dependency chains that balloon between milestones, and build sizes that quietly balloon week over week. Most developers don't notice until the damage is already done.

Build Health Intelligence solves this by letting you take a **snapshot** of your project's key metrics at any point in time, save a **baseline** from a known-good state, and then instantly see the **drift** — what grew, what shrank, and by how much — the next time you check.

---

## What it measures

| Metric | What it tells you |
|---|---|
| Total Assets | How many files are inside your Assets folder |
| Total Scripts | How many C# scripts your project contains |
| Scenes In Build | How many scenes are registered in Build Settings |
| Enabled Scenes | How many of those scenes are switched on |
| Dependency Asset Count | How many unique assets your enabled scenes actually pull in |
| Dependency Estimated Size | The combined uncompressed size of those assets on disk |
| Texture Assets | Total texture files in the project |
| Material Assets | Total material files in the project |
| Shader Assets | Total shader files in the project |

---

## Who it is for

Any Unity developer who wants to catch project bloat early, track milestone-to-milestone changes, or simply understand what is growing inside their project before it becomes a problem.

No coding required to use it. Just open a window, press a button, and see your numbers.
