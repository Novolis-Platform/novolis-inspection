# Design

Formatters turn source values into a common immutable tree:

```text
JSON / XML / HTTP / object graph → InspectionDocument → host renderer
```

The core package contains no reflection formatter policy and no UI dependency. Object graph traversal limits depth, detects cycles, skips indexers, and captures getter failures as nodes.
