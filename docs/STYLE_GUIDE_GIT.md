# Git & Commit Standards

**Version Control:** Git
**Hosting:** GitHub
**Conventional Commits:** Yes
**Last Updated:** 2026-01-03

---

## Table of Contents

1. [Conventional Commits](#conventional-commits)
2. [Commit Message Format](#commit-message-format)
3. [Branching Strategy](#branching-strategy)
4. [Pull Request Process](#pull-request-process)
5. [Code Review Guidelines](#code-review-guidelines)
6. [Merge Strategy](#merge-strategy)
7. [Common Commands](#common-commands)
8. [Best Practices](#best-practices)

---

## Conventional Commits

BenchLibrary uses **Conventional Commits v1.0.0** specification.

### Commit types
```
feat:     A new feature
fix:      A bug fix
docs:     Documentation only changes
style:    Changes that don't affect code meaning (formatting, etc.)
refactor: Code change that neither fixes a bug nor adds a feature
perf:     Code change that improves performance
test:     Adding or updating tests
chore:    Changes to build process, dependencies, or other non-code changes
ci:       Changes to CI/CD configuration files and scripts
```

### Commit scope
Scope is optional but highly recommended:
```
feat(auth):      Add user login functionality
fix(database):   Fix null reference in migration
docs(readme):    Update installation instructions
test(services):  Add unit tests for ProcessService
```

### Examples of commit types
```
feat(blazor):     Add real-time dashboard updates
fix(ef-core):     Correct foreign key cascade delete behavior
refactor(core):   Extract common validation logic to BaseValidator
perf(api):        Add result caching for frequently queried endpoints
test(core):       Increase ProcessAnalyzer test coverage to 95%
docs(api):        Add OpenAPI/Swagger documentation
style(csharp):    Apply formatting per style guide
chore(deps):      Update NuGet packages to latest versions
ci(github):       Add code coverage reporting to GitHub Actions
```

---

## Commit Message Format

### Structure
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Subject line
- Use imperative mood: "add" not "added" or "adds"
- Don't capitalize first letter
- No period (.) at the end
- Limit to 50 characters max
- Be specific and descriptive

```
# ✓ Good
feat(core): add process capability calculation algorithm
fix(database): correct timestamp conversion for SQLite
refactor(web): simplify measurement chart component

# ✗ Avoid
Added feature
Fixed bug
Updates
feat: implement stuff
fix: this was broken
```

### Body (optional but recommended for non-trivial commits)
- Explain **what** and **why**, not how
- Wrap at 72 characters
- Separate from subject with blank line
- Use present tense

```
feat(auth): implement two-factor authentication

Two-factor authentication enhances security by requiring users to provide
a second verification method. This implementation supports:
- Email-based verification codes
- Authenticator app (TOTP) support
- Recovery codes for account recovery

Users are prompted for 2FA on login and can configure it in account settings.
```

### Footer (optional)
Use for breaking changes, issue references:
```
feat(api): redesign measurement endpoint response format

BREAKING CHANGE: The /api/measurements endpoint now returns data in a new format.
Old clients should update to handle the new response structure.

Fixes #123
Closes #456
Related to #789
```

---

## Branching Strategy

### Branch naming
Use **feature branch workflow** with descriptive names:

```
# Format: <type>/<description>
# Types: feature, fix, hotfix, refactor, docs, chore

feature/user-authentication      # New feature
feature/spc-dashboard-ui         # Feature with component focus
fix/null-reference-error         # Bug fix
fix/measurement-rounding-issue   # Bug with clear context
hotfix/security-vulnerability    # Critical production fix
refactor/database-queries        # Refactoring
docs/api-documentation           # Documentation
chore/dependency-upgrade         # Maintenance

# ✗ Avoid
feature/new-stuff
fix/bug
dev
master-fix
bugFix123
my-changes
```

### Main branches

**`main` branch**
- Production-ready code
- Tagged with version numbers (v1.0.0, v1.1.0, etc.)
- Protected: requires PR review and passing checks
- Only merged via PR

**`develop` branch** (if used)
- Integration branch for features
- Pre-release testing
- Protected: requires PR review
- Only merged via PR

### Branching from
```bash
# Create feature branch from latest develop
git checkout develop
git pull origin develop
git checkout -b feature/my-feature

# Or create and switch in one command
git checkout -b feature/my-feature origin/develop
```

### Deleting branches
Delete local and remote branches after merging:
```bash
# Delete local branch
git branch -d feature/my-feature

# Delete remote branch
git push origin --delete feature/my-feature

# Or in one command
git push origin --delete feature/my-feature && git branch -d feature/my-feature
```

---

## Pull Request Process

### PR title format
Follow conventional commits for PR title:
```
feat(core): add measurement statistics calculator
fix(database): correct entity mapping for Process relationships
docs(readme): update installation instructions
```

### PR description template
```markdown
## Summary
Brief description of changes (1-3 sentences)

## Changes
- Specific change 1
- Specific change 2
- Specific change 3

## Testing
Describe how to test these changes:
- [ ] Run unit tests
- [ ] Test in development environment
- [ ] Verify with sample data

## Checklist
- [ ] Code follows style guide (STYLE_GUIDE_CSHARP.md)
- [ ] Tests added/updated with 80%+ coverage
- [ ] Commit messages follow conventional commits
- [ ] No breaking changes (or documented if required)
- [ ] Documentation updated (if applicable)

## Related Issues
Fixes #123
Related to #456
```

### Before submitting PR
```bash
# 1. Update with latest changes from develop
git fetch origin
git rebase origin/develop

# 2. Run tests locally
dotnet test

# 3. Build to check for errors
dotnet build

# 4. Verify code coverage
dotnet test /p:CollectCoverage=true

# 5. Push to remote
git push origin feature/my-feature
```

### PR guidelines
- **One concern per PR**: Don't mix unrelated changes
- **Reasonable size**: Aim for <400 lines changed
- **Clear title & description**: Help reviewers understand intent
- **Link related issues**: Use "Fixes #123" syntax
- **Respond to feedback**: Address all review comments
- **Keep updated**: Rebase on develop if stale

---

## Code Review Guidelines

### For PR authors
- Make commits atomic and logical
- Respond to feedback professionally
- Don't take comments personally
- Ensure all tests pass before requesting review
- Keep PRs focused (single responsibility)

### For reviewers
- Review for correctness, style, and performance
- Suggest improvements constructively
- Ask clarifying questions
- Acknowledge good code patterns
- Check for security vulnerabilities
- Verify test coverage is adequate

### Review checklist
- [ ] Code follows style guide
- [ ] No obvious bugs or logic errors
- [ ] Tests are comprehensive and passing
- [ ] No hardcoded values or credentials
- [ ] Performance is acceptable
- [ ] Documentation is updated
- [ ] Commit messages are clear
- [ ] No unnecessary dependencies added

---

## Merge Strategy

### Squash commits
Squash related commits into logical units before merging:
```bash
# Squash last 3 commits
git rebase -i HEAD~3

# In editor, change 'pick' to 'squash' for commits to combine
# pick abc1234 First commit
# squash def5678 Second commit  <- will squash into first
# squash ghi9012 Third commit   <- will squash into first

# Then force-push (only if not yet pushed, or use caution)
git push -f origin feature/my-feature
```

### Merge to main
Always use **squash merge** to keep main branch clean:
```bash
# Squash merge (creates single commit on main)
git checkout main
git pull origin main
git merge --squash feature/my-feature
git commit -m "feat(core): add measurement statistics calculator"
git push origin main
```

### GitHub UI merge
- Use "Squash and merge" option
- Ensure commit message follows conventional commits
- Delete branch after merging

---

## Common Commands

### Creating and updating branches
```bash
# Create feature branch
git checkout -b feature/my-feature origin/develop

# Update branch with latest changes
git fetch origin
git rebase origin/develop

# Or merge if rebase causes conflicts
git merge origin/develop

# Push changes
git push origin feature/my-feature

# Push with upstream set
git push -u origin feature/my-feature
```

### Viewing commits
```bash
# View commit history (one line each)
git log --oneline -10

# View with graph
git log --graph --oneline --all

# View detailed commit
git show abc1234

# View files changed in commit
git show --stat abc1234
```

### Undoing changes
```bash
# Undo uncommitted changes
git restore <file>

# Undo staged changes
git restore --staged <file>

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1

# Undo pushed commit (creates new commit)
git revert abc1234
```

### Stashing work
```bash
# Save uncommitted changes
git stash

# List stashed changes
git stash list

# Restore stashed changes
git stash pop

# Clear all stashes
git stash clear
```

---

## Best Practices

### Commit frequently
```bash
# Good - logical, atomic commits
git add src/Core/ProcessAnalyzer.cs
git commit -m "feat(core): add capability index calculation"

git add tests/Core.Tests/ProcessAnalyzerTests.cs
git commit -m "test(core): add tests for capability calculation"

# Avoid - large, unfocused commits
git add .
git commit -m "updates"
```

### Write meaningful messages
```bash
# ✓ Good - explains what and why
git commit -m "fix(database): add missing index on Measurement.ProcessId

Performance analysis revealed slow queries on the measurements table
when filtering by process. Adding composite index on (ProcessId, MeasuredAt)
improves query performance by 60% for typical workloads."

# ✗ Avoid - vague messages
git commit -m "fix stuff"
git commit -m "update database"
```

### Keep history clean
```bash
# Before pushing, review commits
git log origin/develop..HEAD --oneline

# Squash if needed
git rebase -i origin/develop

# Force push ONLY if not yet merged to main
git push -f origin feature/my-feature
```

### Don't commit secrets
```bash
# ✓ Good - use secrets management
export DATABASE_PASSWORD=$(aws secretsmanager get-secret-value ...)
dotnet run

# ✗ Never commit
git add appsettings.Production.json  # contains password
git add .env  # contains secrets
```

### .gitignore standards
```
# Build outputs
bin/
obj/
dist/
*.o
*.a
*.lib
*.dll
*.exe

# IDE files
.vs/
.vscode/
.idea/
*.sln.iml
*.user

# Dependencies
node_modules/
packages/
.nuget/

# Secrets
appsettings.*.json
.env
.env.local
secrets.json

# Test/coverage
coverage/
.nyc_output/
*.trx

# OS files
.DS_Store
Thumbs.db
```

### Handling conflicts
```bash
# Rebase and resolve conflicts
git fetch origin
git rebase origin/develop

# View conflicts
git status

# Edit conflicted files, mark resolved
git add resolved-file.cs

# Continue rebase
git rebase --continue

# Or abort if needed
git rebase --abort
```

---

## Workflow Example

```bash
# 1. Create feature branch
git checkout -b feature/add-spc-calculator origin/develop

# 2. Make changes and commit
git add src/SixSigma/SpCCalculator.cs
git commit -m "feat(six-sigma): implement SPC calculator

Implements Statistical Process Control calculations including:
- Control limits (3-sigma)
- Process capability indices (Cp, Cpk)
- Out-of-control detection"

# 3. Add tests
git add tests/SixSigma.Tests/SpCCalculatorTests.cs
git commit -m "test(six-sigma): add comprehensive SPC calculator tests

Covers normal operation, edge cases, and error conditions
Achieves 95% code coverage"

# 4. Update with develop
git fetch origin
git rebase origin/develop

# 5. Push to remote
git push -u origin feature/add-spc-calculator

# 6. Create PR on GitHub with description

# 7. After PR approved, squash and merge
git checkout main
git pull origin main
git merge --squash feature/add-spc-calculator
git commit -m "feat(six-sigma): implement SPC calculator

Implements Statistical Process Control calculations including:
- Control limits (3-sigma)
- Process capability indices (Cp, Cpk)
- Out-of-control detection"
git push origin main

# 8. Delete feature branch
git branch -d feature/add-spc-calculator
git push origin --delete feature/add-spc-calculator
```

---

## Key Rules Summary

✓ **DO:**
- Use conventional commits format
- Create descriptive branch names
- Write atomic, logical commits
- Explain "why" in commit messages
- Squash related commits before merging
- Use squash merge to main
- Review PRs thoroughly
- Link related issues
- Keep branches updated with develop
- Delete merged branches

✗ **DON'T:**
- Use vague commit messages
- Mix unrelated changes in one commit
- Commit secrets or credentials
- Force push to main or develop
- Skip PR reviews
- Create enormous PRs
- Leave branches stale
- Use generic branch names (feature1, bug-fix)
- Merge without tests passing
- Rewrite history on shared branches

