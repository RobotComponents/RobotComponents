<p align="center">
  <picture>
   <source media="(prefers-color-scheme: dark)" srcset="Utility/Logo/rc_logo_white.png">
   <img alt="Light color mode" src="Utility/Logo/rc_logo_black.png">
  </picture>
</p>

<p align="center">
  <img src="https://img.shields.io/github/v/release/RobotComponents/RobotComponents?label=stable">
  <img src="https://img.shields.io/github/v/release/RobotComponents/RobotComponents?label=latest&include_prereleases">
  <img src="https://img.shields.io/github/license/RobotComponents/RobotComponents">
  <a href="https://doi.org/10.5281/zenodo.5773814"><img src="https://zenodo.org/badge/DOI/10.5281/zenodo.5773814.svg" alt="DOI"></a>
</p>

---

Robot Components is a plugin for intuitive robot programming for ABB robots inside Rhinoceros Grasshopper. Robot Components offers a wide set of tools to create toolpaths, simulate robotic motion and generate RAPID code within Grasshopper. Some of the main features include:

- 40+ predefined ABB robot models
- Possibility to add your own robot models
- Support for external axes (both linear and rotational)
- Possibility to define custom strategies for all external axis values
- Support for work objects (including movable work objects)
- Efficient forward and inverse kinematics
- Possibility to add your own custom code lines
- Real-time connection with IRC5 and OmniCore controllers
- [Robot Components API](https://robotcomponents.github.io/RobotComponents-API-Documentation/index.html) to develop your custom components using either Python or C#

## Getting Started
### Installation
If you are using **Rhino 7 or higher**, you can install Robot Components via the **Package Manager**. For other versions, download the latest release directly from the repository's [releases page](https://github.com/RobotComponents/RobotComponents/releases) or from [Food4Rhino](https://www.food4rhino.com/app/robot-components). 

After downloading, unzip the archive and copy all files into the Grasshopper Components folder (in Grasshopper, navigate to *File > Special Folders > Components Folder*). Ensure that all files are unblocked by right-clicking each file, selecting *Properties*, and clicking *Unblock* on the *General* tab. Restart Rhino, and you're ready to go!

Robot Components supports the following Rhino versions:  

- **Version 3 (v3):** Compatible with Rhino 7 or higher.  
- **Version 2 (v2):** Compatible with Rhino 6 or higher.  
- **Version 1 (v1):** Compatible with Rhino 5 or higher.

With each latest release, we aim to support the latest two major Rhino versions. 

### Example files and documentation
Explore a collection of example files demonstrating Robot Components' main features in the repository's [Example Files](https://github.com/RobotComponents/RobotComponents/tree/master/ExampleFiles) folder. Comprehensive documentation for Grasshopper is available on our [documentation website](https://robotcomponents.github.io/RobotComponents-Documentation/) and API documentation can be found [here](https://robotcomponents.github.io/RobotComponents-API-Documentation/index.html).

### Controller utility components
To use the components in the **Controller Utility** section, you must also install [Robot Studio](https://new.abb.com/products/robotics/robotstudio) or the **ABB Robot Communication Runtime**. You can download the runtime directly from [this link](https://github.com/RobotComponents/RobotComponents/raw/main/Utility/ABB%20Communication%20Runtime/ABB%20Robot%20Communication%20Runtime%202025.3.zip).

The latest release of Robot Components is built and tested against **ABB PC SDK version 2025.3** (ABB Robot Communication Runtime 2025.3). Compatibility with older versions of the runtime is not guaranteed. Additionally, the **Controller Utility** components are supported only on **Windows operating systems**.

If you encounter issues with establishing a real-time connection from Grasshopper, please contact us for assistance.

### Quick Links
For easy sharing of the download link and documentation (e.g., with students), you can also use our [Linktree](https://linktr.ee/RobotComponents).

## Credits
<picture>
  <source media="(prefers-color-scheme: dark)" srcset="Utility/Logo/edek_logo_white.png">
  <img alt="Light color mode" src="Utility/Logo/edek_logo_black.png">
</picture>
<picture>
  <source media="(prefers-color-scheme: dark)" srcset="Utility/Logo/logo_arjen_white.png">
  <img alt="Light color mode" src="Utility/Logo/logo_arjen_black.png">
</picture>

Robot Components is an open-source project that was initiated by the chair of [Experimental and Digital Design and Construction of the University of Kassel](https://www.uni-kassel.de/fb06/institute/architektur/fachgebiete/experimentelles-und-digitales-entwerfen-und-konstruieren/home). The plugin is currently being actively developed and maintained by [Arjen Deetman](http://www.arjendeetman.nl). A complete list of developers and contributors is available [here](https://github.com/RobotComponents/RobotComponents/blob/master/AUTHORS.md).

Robot Components integrates the ABB PC SDK for real-time communication with ABB robots. You can access the SDK used in this project [here](https://developercenter.robotstudio.com/pc-sdk).

Robot Components utilizes the OPW kinematics solver, as described in the paper ['_An Analytical Solution of the Inverse Kinematics Problem of Industrial Serial Manipulators with an Ortho-parallel Basis and a Spherical Wrist_'](https://www.researchgate.net/publication/264212870_An_Analytical_Solution_of_the_Inverse_Kinematics_Problem_of_Industrial_Serial_Manipulators_with_an_Ortho-parallel_Basis_and_a_Spherical_Wrist) by Mathias Brandstötter, Arthur Angerer, and Michael Hofbaur.

We would like to acknowledge [Jose Luis Garcia del Castillo](https://github.com/garciadelcastillo) and [Vicente Soler](https://github.com/visose) for their Grasshopper plugins [RobotExMachina](https://github.com/RobotExMachina) and [Robots](https://github.com/visose/Robots). While our approach differs, it was helpful for us to see how they implemented certain functionalities and approached certain issues. 

## Known Issues
Known issues are listed [here](https://github.com/RobotComponents/RobotComponents/issues). If you find a bug, please help us solve it by filing a [report](https://github.com/RobotComponents/RobotComponents/issues/new).

## Roadmap
Please have a look at the open [issues](https://github.com/RobotComponents/RobotComponents/issues) and [projects](https://github.com/RobotComponents/RobotComponents/projects) to know what we are currently developing and what we want to add and change in the future.  

## Contribute
**Bug reports**: Please report bugs at our [issue page](https://github.com/RobotComponents/RobotComponents/issues). 

**Feature requests**: Feature requests can be proposed on our [issue page](https://github.com/RobotComponents/RobotComponents/issues). Please include how this feature should work by explaining it in detail and if possible by adding relevant documentation (from e.g. ABB). 

**Code contributions**: We accept code contributions through [pull requests](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests). For this you have to [fork](https://help.github.com/en/github/getting-started-with-github/fork-a-repo) or [clone](https://help.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) this repository. To compile the code all necessary references are placed in the folder [DLLs](https://github.com/RobotComponents/RobotComponents/tree/master/DLLs). We only accept code contributions if they are commented. We use XML comments to auto-generate our API documentation. You can read more about this topic [here](https://docs.microsoft.com/en-us/dotnet/csharp/codedoc) and [here](https://blog.rsuter.com/best-practices-for-writing-xml-documentation-phrases-in-c/). If you want to make a significant contribution, please let us know what you want to add or change to avoid doing things twice. For questions or if you want to discuss your contribution you can reach out to one of the [developers](https://github.com/RobotComponents/RobotComponents/blob/master/AUTHORS.md). Feel free to add your name to the list with [contributors](https://github.com/RobotComponents/RobotComponents/blob/master/AUTHORS.md) before you make a pull request.

**Adding support for other brands**: Robot Components is developed to intuitively program ABB robots inside Grasshopper. At the moment we have no plans to implement the support for other robot brands. However, we have a few ideas about how to implement this and since we want to keep Robot Components as intuitive as possible we kindly ask you to contact one of the [developers](https://github.com/RobotComponents/RobotComponents/blob/master/AUTHORS.md) first in case if you want to implement other robot brands. We are happy to contribute to and support this development.

## How to cite
Robot Components is a free-to-use Grasshopper plugin, and we kindly ask you to cite it if used. By citing the software, you recognize the work that went into its development and allow us to track its usage, making it easier to secure funding for further improvements. More importantly, citing the software and providing details on the tools used ensures that results can be reproduced. See [How to cite and describe software](https://software.ac.uk/how-cite-software) for more details and an in-depth discussion.

To cite all versions of Robot Components - as a reference to the whole project - in publications, use:

```
Arjen Deetman, Gabriel Rumpf, Benedikt Wannemacher, Mohamed Dawod, Zuardin Akbar, & Andrea Rossi (2018). 
Robot Components: Intuitive Robot Programming for ABB Robots inside of Rhinoceros Grasshopper.
Zenodo. https://doi.org/10.5281/zenodo.5773814
```

On our [Zenodo page](https://doi.org/10.5281/zenodo.5773814) you can find how to cite specific versions.

## Version numbering
Robot Components uses the following [Semantic Versioning](https://semver.org/) scheme: 

```
0.x.x ---> MAJOR version when you make incompatible API changes
x.0.x ---> MINOR version when you add functionality in a backward-compatible manner
x.x.0 ---> PATCH version when you make backward-compatible bug fixes
```
Versions that were not released on [Food4Rhino](https://www.food4rhino.com/app/robot-components) are marked as pre-release. 

## Used by
An overview of the projects wherein the software is used can be found [here](USED_BY.md). 

## License
Copyright (c) 2018-2020 EDEK Uni Kassel\
Copyright (c) 2020-2025 Arjen Deetman

Robot Components is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation. 

Robot Components is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Robot Components; If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.html>
