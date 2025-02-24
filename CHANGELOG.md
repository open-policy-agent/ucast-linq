# Change Log

All notable changes to this project will be documented in this file. This
project adheres to [Semantic Versioning](http://semver.org/).

## 0.2.0

This release includes numerous small cleanups and refactors, and adds initial support for column masking of datasets!

### Column masking

To mask a particular field out of a collection, there is a new `EnumerableExtensions.MaskElements<T>` method, which takes a collection, a Dictionary of column masking rules, and a `MappingConfiguration`, and produces a shallow-cloned copy of the collection where every object's fields have been masked according to the masking rules.

### Breaking changes

The `Dictionary<string, Func<ParameterExpression, Expression>>` type used for LINQ mappings has been switched out for a more powerful `MappingConfiguration` class, which allows reusing the name mappings for both data filtering, and column masking.


## 0.1.0

This release is an release engineering tests, aimed at sorting out automated publishing of a Github Release and NuGet package.
