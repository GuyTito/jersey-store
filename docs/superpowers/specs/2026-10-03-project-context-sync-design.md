# Project context synchronization design

## Goal

Bring the root README and root `agents.md` into agreement about the Jersey Store's current state and active milestone, while establishing reliable shared context for future AI-assisted work.

## Design

- Keep `docs/milestones.md` as the detailed roadmap and source of truth for milestone scope.
- Update `README.md` with the current full-stack project status, the current Milestone 3, and the existing local development instructions.
- Move `frontend/AGENTS.md` to the repository root as lowercase `agents.md`, retaining its Next.js-specific guidance.
- Add project-wide rules to `agents.md`: the project vision follows `docs/milestones.md`; plans belong in `.kilo/plans`; and project state must be kept synchronized between `README.md` and `agents.md`.
- Keep status descriptions concise and consistent between the two root documents, without copying the full milestone roadmap.

## Validation

Review the branch and changed-file list, confirm the old agent-guidance path is removed, and compare both root documents for consistent current status, milestone, vision, planning, and synchronization guidance.
