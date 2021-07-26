# Potential fix for `_queryCompiler` missing field

## Problem description

When using `Z.EntityFramework.Plus.EFCore` alongside `MockQueryable.Moq`, or even any `AsQueryable` other than `EntityFramework`, you get an exception:

```c#
Field '_queryCompiler' defined on type 'Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider' is not a field on the target object which is of type 'MockQueryable.EntityFrameworkCore.TestAsyncEnumerableEfCore`1[EntityFramework.Plus.Moq.Entity]'.
```

### Affected operations:

- BatchDelete
- BatchUpdate
- BatchInsert

## Problem insights

`EntityFramework.Plus` is using reflection to get `_queryCompiler` field from `Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider` class. However, the presence of `_queryCompiler` is not guaranteed by the `IQueryable` interface.

## Solution

The implementation of the method used by operations mentioned above has to return just before using a reflection to get `_queryCompiler`:

```c#
if (Regex.IsMatch(query.Expression.ToString(), "\\.Where\\(\\w+ => False\\)"))
  return 0;
```

So in order to enter this branch, the query expression has to contain a `Where` which in turn contains a lambda that filters out every element.

## Example solution code

The whole program contains an example app with a single method that calls `BatchDelete`, as well as unit tests of that method, thus presenting the problem alongside the solution.
