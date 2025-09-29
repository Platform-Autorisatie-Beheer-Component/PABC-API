# Decision Record: Database-Specific Duplicate Key Exception Handling

**Date:** 2025-09-29

## Context
We need to handle duplicate key violations when saving entities through Entity Framework Core. Our application currently uses PostgreSQL as the database provider.

## Decision
We have implemented duplicate key exception handling specifically for PostgreSQL by catching `DbUpdateException` and checking for:
1. `PostgresException` with SQL state `23505` (unique constraint violation)
2. Fallback: Exception messages containing "unique" (case-insensitive)

The implementation uses an extension method:
```csharp
public static bool IsDuplicateKeyException(this DbUpdateException ex)
{
    return ex.InnerException switch
    {
        PostgresException pgEx => pgEx.SqlState == "23505",
        _ => ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true
    };
}
```

Additionally, we've configured case-insensitive collation for string columns using:
- Locale: `nl-NL-u-ks-primary` (Dutch, case-insensitive)
- Provider: `icu`
- Deterministic: `false`

## Consequences

### Positive

- Clean, reusable exception handling for duplicate keys
- Primary check uses PostgreSQL-specific error code (reliable)
- Fallback message check provides additional safety net
- Case-insensitive unique constraints match business requirements
- Simple implementation that works well with PostgreSQL

### Negative

- Database coupling: Code is currently PostgreSQL-specific
- Fallback message check may catch unintended exceptions (less precise)
- If we switch to SQL Server, MySQL, or other databases in the future, the exception handling will need to be extended with provider-specific checks