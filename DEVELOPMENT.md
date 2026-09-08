# Development

Inspired by <https://github.com/trinodb/trino/blob/master/.github/DEVELOPMENT.md>.

## Commits and pull requests

### Format Git commit messages

We format commit messages to adhere to the [conventional commit standard](https://www.conventionalcommits.org/en/v1.0.0/).
The commit messages are also used to automatically create and version releases using release-please.

### Git merge strategy

Pull requests are usually merged into `master` using the [`squash and merge`](https://docs.github.com/en/pull-requests/reference/pull-request-merges#squash-and-merge-your-commits) strategy.

A typical pull request should strive to contain a single logical change (but not
necessarily a single commit). Unrelated changes should generally be extracted
into their own PRs.

If a pull request contains a stack of more than one commit, then
popping any number of commits from the top of the stack, should not
break the PR, ie. every commit should build and pass all tests.

Commit messages and history are important as well, because they are
used by other developers to keep track of the motivation behind
changes. Keep logical diffs grouped together in separate commits and
order commits in a way that explains by itself the evolution of the
change. Rewriting and reordering commits is a natural part of the
review process. Mechanical changes like refactoring, renaming, removing
duplication, extracting helper methods, static imports should be kept
separated from logical and functional changes like adding a new feature
or modifying code behaviour. This makes reviewing the code much easier
and reduces the chance of introducing unintended changes in behavior.

Whenever in doubt on splitting a change into a separate commit, ask
yourself the following question: if all other work in the PR needs to
be reverted after merging to master for some objective reason (eg. a
bug has been discovered), is it worth keeping that commit still in
master.

## Code Style

The code style is automatically enforced using [csharpier](https://csharpier.com/).

To run it before opening a PR: `dotnet tool restore && dotnet csharpier format .`

In addition to those you should also adhere to the following:

### Readability

The purpose of code style rules is to maintain code readability and developer
efficiency when working with the code. All the code style rules explained below
are good guidelines to follow but there may be exceptional situations where we
purposefully depart from them. When readability and code style rule are at odds,
the readability is more important.

### Consistency

Keep code consistent with surrounding code where possible.

### Alphabetize

Alphabetize sections in the documentation source files (both in the table of
contents files and other regular documentation files).

### Avoid ternary operator

Avoid using the ternary operator except for trivial expressions.

### Maintain production quality for test code

Maintain the same quality for production and test code.

### Avoid abbreviations

Please avoid abbreviations, slang or inside jokes as this makes harder for
non-native english speaker to understand the code. Very well known
abbreviations like `max` or `min` and ones already very commonly used across
the code base like `ttl` are allowed and encouraged.

## Releases

The project aims for frequent releases. We achieve this using release-please, where
each merged PR is added to a release PR. This allows users of the application to quickly
receive bug fixes without waiting for arbitrary release cycles. This only works if the
quality of the code and especially the tests is up-to-par.
