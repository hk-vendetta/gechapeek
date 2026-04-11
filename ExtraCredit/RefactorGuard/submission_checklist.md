# RefactorGuard Submission Checklist

## Package quality
- [ ] Scripts compile cleanly in a fresh Unity 2021 LTS project
- [ ] Tool opens from Tools > RefactorGuard > Impact Analyzer
- [ ] Single-asset analysis works on common asset types
- [ ] Batch analysis works on multi-selection and folder scope
- [ ] Risk tier grouping and dashboard cards render correctly
- [ ] JSON and CSV exports succeed for single and batch reports

## UX sanity checks
- [ ] Empty states are clear when no scan has been run
- [ ] Empty states are clear for assets with no incoming references
- [ ] Preset buttons apply expected filter combinations
- [ ] Reset Filters returns all filters and toggles to default state
- [ ] Dashboard actions apply expected drilldowns

## Performance checks
- [ ] Scan progress bar appears during large project scans
- [ ] Window remains responsive after scan completion
- [ ] Batch view handles at least 1,000 analyzed assets without layout breakage

## Documentation checks
- [ ] description.md matches shipped feature set
- [ ] README.md reflects current buttons and workflow names
- [ ] asset_store_listing.md is finalized for publishing
- [ ] screenshot_plan.md is used to capture actual listing media

## Store readiness
- [ ] Product icon prepared at required resolutions
- [ ] 5 to 8 screenshots exported at store-compliant dimensions
- [ ] Price and launch discount strategy finalized
- [ ] Support contact and response SLA defined
- [ ] Changelog entry prepared for first release

## Final pre-upload
- [ ] Clean package export created from only required files
- [ ] Test import completed in a separate clean Unity project
- [ ] No debug logs left behind for standard workflow
- [ ] Final listing text proofread for spelling and clarity
