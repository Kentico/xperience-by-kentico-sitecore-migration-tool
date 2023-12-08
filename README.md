[//]: # "[![Contributors][contributors-shield]][contributors-url]"
[//]: # "[![Forks][forks-shield]][forks-url]"
[//]: # "[![Stargazers][stars-shield]][stars-url]"
[//]: # "[![Issues][issues-shield]][issues-url]"
[//]: # "[![MIT License][license-shield]][license-url]"
[//]: # "[![Discord][discussion-shield]][discussion-url]"

<!-- ABOUT THE PROJECT -->
# Migration toolkit for Sitecore

The Migration toolkit transfers content and other data from **Sitecore** to **Xperience by Kentico**.

## Prerequisites & Compatibility

### Source

* The source of the migration data must be a Sitecore XM or XP instance, version 9.0 (Initial release) or newer.
* The tool runs in the context of a Sitecore application, therefore it should be deployed to a Sitecore CM or Standalone instance that has access to a database with the content you want to migrate.

### Target

* TBD

## Supported data and limitations


### Unsupported data


<!-- GETTING STARTED -->
## Get started

Follow the steps below to run the Universal Migration Toolkit for Sitecore:

1. Download the package from this repository.
2. Install the package on your Sitecore instance by going to the Control Panel → Install a package and following the installation wizard steps.
3. Configure the options in the `App_Config/Include/UMT/UMT.Sitecore.config` configuration file.
4. Navigate to the URL `/sitecore/UMT.aspx`.
5. Select a channel from the list, select root paths and types of data you would like to migrate.
6. Click the button and wait for the process to run. Generated JSON files are stored in the folder `App_Data/UMT/` by default.