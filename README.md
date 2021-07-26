# Potential fix for `_queryCompiler` missing field
## Problem description

When using `Z.EntityFramework.Plus.EFCore` alongside with `MockQueryable.Moq`, or even any `AsQueryable` other than `EntityFramework` implementation there is occuring exception:
```c#
Field '_queryCompiler' defined on type 'Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider' is not a field on the target object which is of type 'MockQueryable.EntityFrameworkCore.TestAsyncEnumerableEfCore`1[EntityFramework.Plus.Moq.Entity]'.
```
### Problematic operations:
- BatchDelete
- BatchUpdate
- BatchInsert

## Problem insights
`EntityFramework.Plus` is using reflection to get `_queryCompiler` field from `Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider` class.  
However any other implementation of `IQueryable` e.g. `new List<T>().AsQueryable()` does not have this field.

## Solution
Implementation of method used by mentioned above operations can return just before using reflection to get `_queryCompiler`:
```c#
if (Regex.IsMatch(query.Expression.ToString(), "\\.Where\\(\\w+ => False\\)"))
    return 0;
```
so to enter this branch query expression has to have `Where` containing lambda that filters out every element.

## Example solution code
Whole program contains example app with single method that calls `BatchDelete` and tests presenting problem and the fix.