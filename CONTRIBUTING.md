# Contributing

## License

By contributing, you agree that your contributions will be licensed under the [MIT](LICENSE).

## Contribution process

This is the process we suggest for contributions. This process is designed to reduce the burden on project reviewers,
impact on other contributors, and to keep the amount of rework from the contributor to a minimum.

1. Start a discussion by creating a GitHub issue, or asking on the MII Zulip (unless the change is trivial, for example a spelling fix in the documentation).

   1. This step helps you identify possible collaborators and reviewers.
   1. Does the change align with technical vision and project values?
   1. Will the change conflict with another change in progress? If so, work with others to minimize impact.
   1. Is this change large? If so, work with others to break into smaller steps.

1. Implement the change

   1. Create or update your own fork of the repository.
   1. If the change is large, post a draft GitHub pull request.
   1. Include tests and documentation as necessary.
   1. Follow the commit message guidelines and other suggestions from the [development guidelines](DEVELOPMENT.md).

1. Create a GitHub pull request (PR).

   1. If you already have a draft PR, change it to ready for review.
   1. Refer to the GitHub documentation for more details about collaborating with PRs.
   1. Make sure the pull request passes the tests in CI.
   1. Code reviewers will be assigned.

1. Review is performed by one or more reviewers.

   1. This normally happens within a few days, but may take longer if the change is large, complex, or if a critical reviewer is unavailable. (feel free to ping the reviewer or team on the pull request).

1. Address concerns and update the pull request.

   1. Comments are addressed to each individual commit in the pull request, and changes should be addressed in a new fixup commit placed after each commit. This is to make it easier for the reviewer to see what was updated.
   1. After pushing the changes, add a comment to the pull-request, mentioning the reviewers by name, stating that the review comments have been addressed. This is the only way that a reviewer is notified that you are ready for the code to be reviewed again.

1. Maintainer merges the pull request after final changes are accepted.

1. Merging your improvements will usually trigger a new release once merged
