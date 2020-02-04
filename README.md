# Robot Components
Robot Components is a Plugin for intuitive Robot Programming for ABB robots inside of Rhinos Grasshopper.

PLUGIN DESCRIPTION...

## Getting Started
You can download the latest release directly from this repository's [Releases page](https://github.com/EDEK-UniKassel/RobotComponentsSource/releases). Unzip the donwloaded archive and copy all files in the Grasshopper Components folder (in GH, File > SPecial Folders > Components Folder). Right click on all files to make sure they are not blocked. Restart Rhino and you are ready to go!

You can find a collection of example files demonstrating the main features of Robot Components in this repository in the folder [Example Files](https://github.com/EDEK-UniKassel/RobotComponentsSource/tree/master/ExampleFiles).
A release on Food4Rhino is planned in the next months.

## Credits
![EDEK_logo](https://github.com/EDEK-UniKassel/RobotComponentsSource/blob/master/Utility/181101_EDEK-LOGO-01.png)
The Plugin is a development from the [Department of Digital and Experimental Design and Construction of the University of Kassel](https://edek.uni-kassel.de/). Supervised by the head of the department Prof. Eversmann. The technical development is executed by student assistant Gabriel Rumpf and research associates Benedikt Wannemacher, Arjen Deetman, Mohamed Dawod, Zuardin Akbar and Andrea Rossi.

RobotComponents uses the ABB PC SDK for real-time connection to ABB Robots, you can find the .dll used in this project here: http://developercenter.robotstudio.com/landing

## Known Issues
**Object manager:** RobotComponents uses an object manager to check for duplicate namings in created tools, targets and speeddatas. The naming is checked based on the unique document number (DocumentID) of the Grasshopper file. The DocumentID is a static readonly property that will be assigned when the user makes a new Grasshopper file. If you copy a Grasshopper file in the explorer or you save a new Grasshopper file by using 'save as' a copy of the Grasshopper file wil be created with the same DocumentID. This causes the problem that a warning can be raised that certain names are already in use when two Grasshopper files are open with the same DocumentID when in both Grasshopper files equal namings are used. **solved in version 0.05.000**

## Roadmap
HERE SHOULD GO WHAT COMES NEXT

## Contribute
HERE SHOULD GO CONTRIBUTIONS GUIDELINES

## References
Dawod M. et al. (2020) Continuous Timber Fibre Placement. In: Gengnagel C., Baverel O., Burry J., Ramsgaard Thomsen M., Weinzierl S. (eds) Impact: Design With All Senses. DMSB 2019. Springer, Cham

## License
Robot Components

Copyright (c) 2020, WHO HAS THE COPYRIGHT???

Robot Components is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation. 

Robot Components is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Wasp; If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl.html>
