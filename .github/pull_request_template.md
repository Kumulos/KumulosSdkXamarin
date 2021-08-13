### Description of Changes

(briefly outline the reason for changes, and describe what's been done)

### Breaking Changes

- None

### Release Checklist

Prepare:

- [ ] Detail any breaking changes. Breaking changes require a new major version number
- [ ] Set Build target to Release -> Rebuild All
- [ ] Run `nuget pack` to create a bundle for nuget.org

Bump versions in:

- [ ] `Com.Kumulos.nuspec`
- [ ] `Com.Kumulos.Extension.nuspec`
- [ ] `Com.Kumulos.Abstractions\Consts.cs`

Release:

- [ ] Squash and merge to master
- [ ] Delete branch once merged
- [ ] Create release/tag from master matching chosen version
- [ ] Upload the output Com.Kumulos.[version].nupkg to nuget.org

Update changelog:

- [ ] https://docs.kumulos.com/developer-guide/sdk-reference/xamarin/#changelog

