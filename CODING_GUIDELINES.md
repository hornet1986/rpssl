# Coding Guidelines

Consistent conventions make the codebase easier to navigate, review, and evolve. These guidelines extend the default .NET / C# recommendations (see: Microsoft C# Coding Conventions) and focus on domain-driven, layered architecture concerns specific to this repository.

## Table of Contents
1. General Principles
2. Solution & Layering
3. Naming Conventions
4. Project Structure
5. C# Style Rules
6. Nullability & Defensive Coding
7. Exceptions & Error Handling
8. Domain & Application Patterns
9. Validation
10. Logging
11. Configuration & Options
12. Async & Performance
13. Testing Guidelines
14. Git, Branching & Commits
15. Pull Requests & Reviews
16. Documentation

---
## 1. General Principles
- Prefer clarity over cleverness; optimize for the reader.
- Keep public surface area minimal; internal where appropriate.
- Make illegal states unrepresentable (leverage types & value objects).
- Fail fast with explicit validation, then operate on valid objects.
- Prefer immutability; mutate state in narrow, controlled contexts.
- Avoid premature abstraction; follow Rule of Three before extracting.

## 2. Solution & Layering
- Layers: Domain -> Application -> Infrastructure -> Api.
- Domain has ZERO dependencies on other solution projects.
- Application references Domain & SharedKernel. Coordinates use cases.
- Infrastructure references Application + Domain for EF configs & external integrations.
- Api references Application & Infrastructure only.
- SharedKernel: only stable abstractions (Result, base interfaces, constants). Keep it lean.

## 3. Naming Conventions
- Projects: `Rpssl.{Layer}` (already applied).
- Namespaces mirror folder paths: `Rpssl.Application.Gameplay`.
- Classes: PascalCase (`GameOutcomeService`).
- Interfaces: prefix with `I` only for behavior contracts (`IOutcomeCalculator`). Avoid marker interfaces.
- DTOs / Requests / Responses: suffix with `Request`, `Response`, or `Dto` consistently.
- EF entities: domain model classes (avoid `Entity` suffix unless it clarifies ambiguity).
- Async methods end with `Async`.
- Private fields: `_camelCase` (backing fields only when needed). Otherwise prefer auto-properties.
- Constants: `PascalCase`. Avoid ALL_CAPS unless truly a compile-time `const` that is widely recognized (still PascalCase is fine).
- Enums: singular, PascalCase values (`MoveType.Rock`).
- Unit test classes: `{TypeName}Tests`; method names: `MethodName_ShouldExpectedBehavior_WhenCondition`.
- Branch names: `feat/`, `fix/`, `chore/`, `refactor/`, `perf/`, `test/`, `docs/`, `ci/`, `build/`, `revert/` prefixes; use short-kebab-case descriptor (`feat/add-round-resolver`).

## 4. Project Structure
- Group related concepts by feature inside `Application` when cohesive (feature folders) rather than strict technical slices.
- Domain: Entities, ValueObjects, Enumerations, DomainEvents, Services (pure domain logic), Specifications.
- Application: Commands, Queries, Handlers, Validators, Orchestrators, Mappers.
- Infrastructure: EF (DbContext, Configurations), Repository implementations, External Client adapters, Polly policies.
- Api: Controllers / Endpoints, Filters, DI wiring, Auth, Versioning, Swagger config.
- Avoid circular references; if something feels shared ask: "Is it truly stable?" If not, keep it in the higher layer.

## 5. C# Style Rules
- Enable nullable reference types in all projects (`<Nullable>enable</Nullable>`).
- Use `file-scoped namespaces`.
- Use `var` when the RHS type is obvious; otherwise be explicit.
- Expression-bodied members for trivial one-liners; otherwise favor readable blocks.
- Prefer records for immutable data-carrier types.
- Order members: constants, fields, constructors, properties, methods, private helpers, nested types.
- Use `readonly` for fields that never change after construction.
- Prefer pattern matching over complex `if` / `switch` chains.

## 6. Nullability & Defensive Coding
- Avoid returning `null` for collections—return empty collection instead.
- Guard public method parameters early with clear `ArgumentException` / `ArgumentNullException`.
- Use option / result types instead of `null` to express absence/failure where suitable (e.g., `Result<T>`).
- Consider `required` properties on mutable records/classes (C# 11) to enforce invariants.

## 7. Exceptions & Error Handling
- Exceptions are for unexpected states, not control flow.
- Domain invariants failing should throw domain-specific exceptions OR (preferred) be prevented by types.
- Translate exceptions at boundaries (e.g., infrastructure) into `Result` failures where the caller expects a recoverable outcome.
- Wrap external calls with resilience (Polly) + enrich failures with context.

## 8. Domain & Application Patterns
- Entities contain behavior; avoid anemic models.
- Use Value Objects to bundle related primitives; validate on creation.
- Domain events: publish on entity methods that change significant state; handle in Application layer.
- Application services (use case orchestrators) should be thin; delegate logic to domain services/entities.
- Avoid static state except for pure functions / constants.
- Keep `DbContext` usage inside Infrastructure or thin persistence abstractions.

## 9. Validation
- Use FluentValidation in Application layer validators for requests/commands.
- Domain constructors/factories perform invariants enforcement independent of external validators.
- Do not duplicate validation logic; domain invariants trump external validation.

## 10. Logging
- Log context-rich events (correlation IDs, user, request IDs) at boundaries.
- Levels: Trace (diagnostics), Debug (developer insights), Information (business milestones), Warning (recoverable anomalies), Error (failed operation), Fatal (process terminating).
- Avoid logging sensitive data (tokens, credentials, PII). Mask where necessary.
- Use structured logging (`logger.LogInformation("Move {Move} beats {Other}", move, other)`).

## 11. Configuration & Options
- Bind configuration sections into options classes (`services.Configure<GameplayOptions>(...)`).
- Validate options on startup (use `ValidateDataAnnotations` or custom validators).
- Keep secrets external (user-secrets locally, environment/secret store in prod).

## 12. Async & Performance
- All IO-bound operations should be async end-to-end.
- Use `CancellationToken` on public async APIs.
- Avoid `async void` except for event handlers.
- Do not block on async (`.Result`, `.Wait()`); prefer `await`.
- Consider caching for deterministic, frequently requested reference data (MemoryCache or distributed).
- Benchmark hot paths before micro-optimizing (`BenchmarkDotNet`)—avoid premature optimization.

## 13. Testing Guidelines
- Strive for pyramid: many unit tests, fewer integration tests, minimal end-to-end (yet meaningful).
- Test names: `Method_ShouldExpectation_WhenCondition` (or BDD style: `Given_State_When_Action_Then_Result`).
- One assertion concept per test (multiple asserts okay if same concept).
- Use `AutoFixture` / builders to reduce arrange noise (consider adding if not present).
- Avoid hitting real external services; mock via interfaces.
- Use in-memory EF provider only for simple cases; prefer real DB integration tests for complex queries.
- Keep tests deterministic—avoid time-based flakiness.

## 14. Git, Branching & Commits
- Branch off `main` (or `develop` if introduced later).
- Small, focused commits; commit early, commit often.
- Conventional commits encouraged:
  - feat:, fix:, docs:, style:, refactor:, perf:, test:, chore:, build:, ci:, revert:
- Commit messages: imperative mood ("Add outcome calculator", not "Added").
- Rebase local feature branches before PR (avoid large merge commits) when feasible.

## 15. Pull Requests & Reviews
- Provide context: what/why, not just what changed.
- Keep PRs under ~400 lines diff when possible for reviewer efficiency.
- Checklist before opening PR:
  - All tests green (`dotnet test`).
  - No new analyzer warnings.
  - Updated docs / README sections if behavior changes.
  - Added/updated tests for new logic.
- Be gracious in reviews; critique code, not people.

## 16. Documentation
- Update `README.md` when adding new external dependencies, environment variables, or noteworthy architecture changes.
- Add XML docs only for public APIs with non-obvious behavior.
- Prefer high-level ADR (Architecture Decision Record) markdown files under a future `/docs/adr` directory for significant architectural decisions.

---
These guidelines will evolve; propose improvements via PRs labeled `docs`.
