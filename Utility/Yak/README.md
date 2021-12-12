# Yak

## Create the package

- Replace the .gha and .dll files with the files from the new version
- Update the manifest file
- Navigate with the command prompt to the directory with the manifest file
- Build the package with `"C:\Program Files\Rhino 7\System\Yak.exe" build`
- Rename the package to `robot-components-x.x.x-any-win.yak` (replace `x.x.x.` with the correct version number) to make it compatible with all rhinoceros versions and windows only

## Push the package to the server

- Login via the command prompt with `"C:\Program Files\Rhino 7\System\Yak.exe" login`
- Push the package with `"C:\Program Files\Rhino 7\System\Yak.exe" push robot-components-x.x.x-any-win.yak` (replace `x.x.x.` with the corect version number)
- Check if the package has been successfully pushed `"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease robot-components`