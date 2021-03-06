![Icon](http://www.gridprotectionalliance.org/images/products/icons%2064/openXDA.png)![openXDA](http://www.gridprotectionalliance.org/images/products/openXDAW.png)

**eXtensible Disturbance Analytics**

# About
openXDA is an extension of work conducted in 2012 on openFLE for the Electric Power Research Institute (EPRI) which is posted under the project http://EPRIopenFLE.codeplex.com/.  The openFLE project included an open source C# parser for PQDIF formatted power quality files (IEEE 1159.3-2003).

openXDA forms the data layer for the 2014 EPRI project, EPRI Open PQ Dashboard http://sourceforge.net/projects/epriopenpqdashboard.

# Overview
openXDA is an extensible platform for processing event and trending records from disturbance monitoring equipment such as digital fault recorders (DFRs), relays, power quality meters, and other power system IEDs.  It includes a parser for COMTRADE and PQDIF formatted records, and demonstrations have been conducted using Schweitzer Engineering Laboratories (SEL) .eve files. openXDA can be used as a data integration layer and can facilitate the development of automated analytic systems.  openXDA has been deployed in a number of major US utilities to perform automated fault distance calculations based on disturbance waveform data combined with line parameters. openXDA determines the fault presence and fault type, and uses 6 different single ended fault distance calculation methods to determine the line-distance to the fault. For more information see: [The BIG Picture - Open Source Software (OSS) for Disturbance Analytic Systems](http://www.slideshare.net/FredElmendorf/2014-georgia-tech-fda-pres-asda-using-oss-37239423) and [openFLE overview](http://www.gridprotectionalliance.org/pdf/openFLE_Overview_Landscape.pdf).

# How it Works

openXDA is a platform comprised of a back office service designed to consume all disturbance records that conform to either the IEEE PQDIF or IEEE COMTRADE formats, and a configuration manager that controls the operation of the service and the automated analytics

## Two Major Components

* **The openXDA service**
  * Discovers PQDIF or COMTRADE even files.
  * Parses files into timestamp-value sets.
  * Calculates frequency domain values.
    * *True RMS voltage and current magnitudes.*
    * *Sine wave curve fit to determine phase angles.*
  * Determines fault presence.
  * Calculates fault type and fault distance.
  * Populates results folder and/or Transmission Event Database.
  * Emails results to selected users.
* **The openXDA Manager**
  * Configures the openXDA service.

## Two Major Layers
* **Get Event Data**
  * Reliably frees developers from the chore of parsing and positioning event data for analysis.
  * Automatically converts time domain data from IEDs into frequency domain values.
* **Perform Calculations**
  * openXDA can be extended with new algorithms.
  * Additional analytics can be included as new modules in the platform.


![openXDA Overview](https://raw.githubusercontent.com/GridProtectionAlliance/openXDA/master/Source/Documentation/Readme%20files/XDA-Overview.png)

**Where It Fits In:**
![Where-It-Fits-In](http://www.gridprotectionalliance.org/images/products/PQ%20Tool%20Suite.png)


# Why openXDA?

Traditional disturbance analytic processes typically require manual retrieval of the data from a variety of substation devices, and a thorough knowledge of many different proprietary vendor software packages.  Ultimately, the process requires a significant investment of staff time to retrieve, analyze, and notify, or publish results of the analysis.  The openXDA provides a platform that is designed for easy extension and integration into existing data processes, to facilitate the development and deployment of fully automated analysis systems.  An example system could produce automated fault recognition, type determination, and distance to fault calculations to be included in notifications, emails, or an event database.  Systems built on the openXDA platform can produce analytic results as quickly as files can be retrieved from the remote devices.  The openXDA platform is suitable for developing systems to automatically perform any analysis that is appropriate for data contained in disturbance records from IEDs.


# Documentation and Support
* Documentation for openXDA can be found [here](https://github.com/GridProtectionAlliance/openXDA/tree/master/Source/Documentation).
* Get in contact with our developement team on our new [discussion boards](http://discussions.gridprotectionalliance.org/c/gpa-products/openxda).
* Check out the [wiki](https://gridprotectionalliance.org/wiki/doku.php?id=openxda:overview).

# Deployment

1. Make sure your system meets all the [requirements](#requirements) below.
* [Download](#downloads) a version below.
* Unzip if necessary.
* Run "openXDASetup.msi".
* Follow the wizard.
* [Configure the database with your devices](#device-configuration)
* Enjoy.

## Requirements
### Operating System
* 64-bit Windows 7 or Windows Server 2008 R2 (or newer).

### Minimum Hardware
* 2.0 GHz CPU.
* 2.0 GB RAM.
* 50 GB of available disk space for installation and testing. Operational disk space requirements will be proportional to the volume of input data.

### Software
* openHistorian 2.0
* .NET 3.5 SP1 (required by SQL Server 2012).
* .NET 4.6.
* SQL Server 2012 with management tools.
  * Free Express version is fine, but has a 10GB limit.
  * Mixed mode authentication must be enable on the SQL Server.

### Device Configuration
The openXDA database needs to be configured with information on the devices that provide it data. 
The steps to do this are:

1. Create a DeviceDefinitions.xml file. This is done by:
  * Manually creating a DeviceDefinitions.xml file. Check out the example xml files for [DFR Relays](https://raw.githubusercontent.com/GridProtectionAlliance/openXDA/master/Source/Documentation/Device%20Definitions%20Examples%20%26%20Templates/DeviceDefinitionsExample-DFR-Relay.xml) and [PQ Devices](https://raw.githubusercontent.com/GridProtectionAlliance/openXDA/master/Source/Documentation/Device%20Definitions%20Examples%20%26%20Templates/DeviceDefinitionsExample-PQ.xml) to see what fields are necessary.
  * OR
  * Filling out the Excel template for [DFR Relays](https://github.com/GridProtectionAlliance/openXDA/blob/master/Source/Documentation/Device%20Definitions%20Examples%20&%20Templates/openXDA%20Configuration%20Template.xlsx?raw=true) or [PQ Devices](https://github.com/GridProtectionAlliance/openXDA/blob/master/Source/Documentation/Device%20Definitions%20Examples%20&%20Templates/openXDA%20Configuration%20Template%20-%20PQ.xlsx?raw=true) and using the [the DeviceDefinitionParser](https://github.com/GridProtectionAlliance/DeviceDefinitionParser/releases) to convert the files to the DeviceDefinitions.xml file.
* Save the DeviceDefinitions.xml file in the folder where openXDA is installed.
* Run the ldconfig.bat script included in openXDA and found in the program directory to configure the database from the DeviceDefinitions.xml file.

## Downloads
* Download a stable release from the releases page [here](https://github.com/GridProtectionAlliance/openXDA/releases).
* Download the nightly build [here](http://www.gridprotectionalliance.org/nightlybuilds/openXDA/Beta/Applications/openXDA/openXDASetup.msi).

# Contributing
If you would like to contribute please:

1. Read the [styleguide.](https://www.gridprotectionalliance.org/docs/GPA_Coding_Guidelines_2011_03.pdf)
* Fork the repository.
* Work your magic.
* Create a pull request.
