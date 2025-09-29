# Decision Record


## Database-Specific Duplicate Key Exception Handling

### Context
We need to handle duplicate key violations when saving entities through Entity Framework Core. Providing proper feedback for this would require PostgreSQL specific code, but that would make the application dependent of this specific database.

### Decision
We have implemented duplicate key exception handling specifically for PostgreSQL. The benefits of clear feedback on duplicate key violations outweighs the disadvantages of PostgreSQL specific code. 

### Consequences
The code is currently PostgreSQL-specific, but we consider it unlikely that the need to support other databases will be requested in the future.   
However if that would happen, extending the exception handling to support other databases as well would have little impact.
