# Robot Components
Robot Components is a Plugin for intuitive Robot Programming for ABB robots inside of Rhinoceros Grasshopper.

PLUGIN DESCRIPTION...

## Getting Started
You can download the latest release directly from this repository's [releases page](https://github.com/EDEK-UniKassel/RobotComponentsSource/releases). Unzip the donwloaded archive and copy all files in the Grasshopper Components folder (in GH, File > SPecial Folders > Components Folder). Right click on all files to make sure they are not blocked. Restart Rhino and you are ready to go!

You can find a collection of example files demonstrating the main features of Robot Components in this repository in the folder [Example Files](https://github.com/EDEK-UniKassel/RobotComponentsSource/tree/master/ExampleFiles).
A release on Food4Rhino is planned in the next months.

## Credits
![EDEK_logo](https://github.com/EDEK-UniKassel/RobotComponentsSource/blob/master/Utility/181101_EDEK-LOGO-01.png)

The project is a development from the [Department of Experimental and Digital Design and Construction of the University of Kassel](https://edek.uni-kassel.de/). Supervised by the head of the department Prof. Eversmann. The technical development is initiated and executed by the research associate and student assistants which are listed [here](https://github.com/EDEK-UniKassel/RobotComponentsSource/blob/master/AUTHORS.md).

RobotComponents uses the ABB PC SDK for real-time connection to ABB Robots, you can find the .dll used in this project here: http://developercenter.robotstudio.com/landing

## Known Issues
Known issues are listed [here](https://github.com/EDEK-UniKassel/RobotComponentsSource/issues). If you find a bug, please help us solve it by filing a [report](https://github.com/EDEK-UniKassel/RobotComponentsSource/issues/new).

## Roadmap
Planned developments:
- Adding the ABB IRB 6620, 6640, 6650S, 6700, 6790 and 7600 series as Robot Info presets. 
- Adding support for MultiMove.

## Contribute
**Bug reports**: Please report bugs at our [issue page](https://github.com/EDEK-UniKassel/RobotComponentsSource/issues). 

**Feature requests**: Feature request can be proposed on our [issue page](https://github.com/EDEK-UniKassel/RobotComponentsSource/issues). Please include how this feature should work by explaining it in detail and if possible by adding relevant documentation from e.g. ABB. 

**Code contributions**: We accept code contributions through [pull requests](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests). For this you have to [fork](https://help.github.com/en/github/getting-started-with-github/fork-a-repo) or [clone](https://help.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) this repository. To compile the code all necesarry references are placed in the folder [DLLs](https://github.com/EDEK-UniKassel/RobotComponentsSource/tree/master/DLLs). For questions you can contact one of the [developers](https://github.com/EDEK-UniKassel/RobotComponentsSource/blob/master/AUTHORS.md). 

**Adding support for other brands**: To a certain extent RobotComponents is prepared to implemented the support of other robot brands (e.g. Fanuc and KUKA). RobotComponent is splitted in multiple parts: RobotComponents.dll which contains all the base classes, RobotComponentsGoos.dll which contains the Grasshopper wrapper classses and RobotComponentsABB.gha which contains all the components and parameters. Since we splitted RobotComponents in multiple parts the support for other robot brands can be implemented in a few different ways. For that reason and that we want to keep RobotComponents as intiviute as possible we kindly ask you to contact one of the [developers](https://github.com/EDEK-UniKassel/RobotComponentsSource/blob/master/AUTHORS.md) if you want to implement other robot brands. We are happy to contribute to and support this development.

## Cite RobotComponents
RobotComponents is a free to use Grasshopper plugin and does not legally bind you to cite it. However, we have invested time and effort in creating RobotComponents, and we would appreciate if you would cite if you used. To cite RobotComponents in publications use:

```
NAME?, NAME?, NAME? OR NAME OF DEPARTMENT? (2020).  
RobotComponents v0.06.000: Intuitive Robot Programming for ABB Robots inside of Rhinoceros Grasshopper. 
URL https://github.com/EDEK-UniKassel/RobotComponents
```

Note that there are two reasons for citing the software used. One is giving recognition to the work done by others which we already addressed. The other is giving details on the system used so that experiments can be replicated. For this, you should cite the version of RobotComponents used. See [How to cite and describe software](https://software.ac.uk/how-cite-software) for more details and an in depth discussion.

## References
[Dawod M. et al. (2020) Continuous Timber Fibre Placement. In: Gengnagel C., Baverel O., Burry J., Ramsgaard Thomsen M., Weinzierl S. (eds) Impact: Design With All Senses. DMSB 2019. Springer, Cham](https://link.springer.com/chapter/10.1007/978-3-030-29829-6_36)

## License
Robot Components

Copyright (c) 2020, WHO HAS THE COPYRIGHT???

Robot Components is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation. 

Robot Components is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with RobotComponents; If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl.html>
