[//]: # "[![Contributors][contributors-shield]][contributors-url]"
[//]: # "[![Forks][forks-shield]][forks-url]"
[//]: # "[![Stargazers][stars-shield]][stars-url]"
[//]: # "[![Issues][issues-shield]][issues-url]"
[//]: # "[![MIT License][license-shield]][license-url]"
[//]: # "[![Discord][discussion-shield]][discussion-url]"

<!-- ABOUT THE PROJECT -->
# Migration toolkit for Sitecore

The Universal Migration Toolkit (UMT) for Sitecore automates data export from Sitecore to be imported into Xperience by Kentico.

## Getting Started

### Source

* The source of the migration data must be a Sitecore XM or XP instance, version 9.0 (Initial Release) or newer.
* The tool runs in the context of a Sitecore application, therefore it should be deployed to a Sitecore CM or Standalone instance that has access to a database with the content you want to export. For the best performance it is recommended to take a backup of Sitecore `master` database and connect your local Sitecore instance to it. However, it is also possible to install the UMT package in a cloud environment and run the export process there.

### Installation

Follow the steps below to install the Universal Migration Toolkit for Sitecore:

1. Download the package from the repository [Releases](https://github.com/Kentico/sitecore-migration-toolkit/releases).
2. Install the package to your Sitecore instance by going to the **Control Panel** → **Install a package** and following the installation wizard steps. The package will add `.config` and `.dll` files so it will require a restart of the Sitecore application. 
3. Configure the options in the `App_Config/Include/UMT/UMT.Sitecore.config` configuration file. See [Configuration](#configuration) section below for reference.

### Configuration

<table>
    <thead>
        <tr>
            <th>Configuration section</th>
            <th>Configuration element</th>
            <th>Description</th>
            <th>Examples</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td rowspan=4>settings</td>
            <td>UMT.Database</td>
            <td>Database name that will be used for extracting the data from. It should be a Sitecore database name linked to a valid connection string.</td>
            <td>The default value is <code>master</code></td>
        </tr>
        <tr>
            <td>UMT.DataFolder</td>
            <td>Folder path on the file system that will be used for storing the generated output.</td>
            <td>The default value is <code>$(dataFolder)/UMT</code></td>
        </tr>
    </tbody>
</table>


## Usage

### Exporting Data

1. Navigate to the URL `/sitecore/UMT.aspx`.
2. Select a channel from the list, enter root paths and languages you would like to export. 
3. Optionally, select a media library and enter media folder paths you would like to export. Content and media exports can be created in one run or separately.
4. Click the button **Run export** and wait for the process to finish. The current status and progress of the export job will be updated automatically and shown on the same page.
5. Once the process is finished, you can copy the generated JSON files and use them for data import into your Xperience by Kentico instance. Generated JSON files are stored in the folder `App_Data/UMT/` by default.
