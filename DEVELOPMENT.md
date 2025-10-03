# ucast-linq Development

## Reference docs auto-publishing

On every commit merged to `main`, the docs will automatically be rebuilt using DocFX, and then published using Github Pages as a "Deployment", similar to how the `open-policy-agent/opa-csharp` repo publishes its reference docs.


## Release workflow

The release workflow for this repository is modeled on the other `open-policy-agent` C# projects, and relies 

To create a release:
 - Create a new branch/PR.
 - Ensure at least one commit on the branch matches the case-insensitive regex `^Release .*` (Example: `Release v0.0.1`)
 - Update `CHANGELOG.md` with an entry for that release, describing the changes.
 - Update the `.csproj` file's version tag.

The `pull-request` automation *should* automatically check each of those three places, and report the version numbers it finds in a PR comment.

After merging the release PR, push up a tag for that version. (Example: `git checkout main && git pull && git tag v0.0.1`)
Make sure you've tagged the right commit before pushing up the tag!

Once the tag is pushed, a draft Github Release will be created (requires human intervention to actually publish).
The package will be pushed up to NuGet automatically in a separate Github Actions job.