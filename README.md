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
            <th>Notes</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td rowspan="10">&lt;settings&gt;</td>
            <td>UMT.Database</td>
            <td>Database name that will be used for extracting the data from. It should be a Sitecore database name linked to a valid connection string.</td>
            <td>The default value is <code>master</code></td>
        </tr>
        <tr>
            <td>UMT.DataFolder</td>
            <td>Folder path on the file system that will be used for storing the generated output.</td>
            <td>The default value is <code>$(dataFolder)/UMT</code></td>
        </tr>
        <tr>
            <td>UMT.DataFolderDateFormat</td>
            <td>Date-based name format for the folder that will be created for each run.</td>
            <td>The default value is <code>yyyy-MM-dd HH-mm-ss</code></td>
        </tr>
        <tr>
            <td>UMT.ExportMediaAsUrls</td>
            <td>This setting allows switching between file-based and URL media extracts. When `true`, media binary files will not be created on the file system and instead they will be created as URLs, otherwise each file will be saved to the output folder. Consider setting this to `true` if you, for example, run exports in a cloud environment and have issues downloading a large media library with many files.</td>
            <td>The default value is <code>false</code></td>
        </tr>
        <tr>
            <td>UMT.ExportMediaAsUrls.ServerUrl</td>
            <td>This setting allows overriding the hostname for generated media URLs when using the option `UMT.ExportMediaAsUrls`. Leave it empty to use the current Sitecore instance settings.</td>
            <td>The default value is <code>empty</code></td>
        </tr>
        <tr>
            <td>UMT.MaxFilePathLength</td>
            <td>Maximum allowed file path on the file system, the export will truncate paths and file names longer than that when <code>UMT.TrimLongMediaFolderPaths</code> is set to <code>true</code>.</td>
            <td>The default value is <code>256</code></td>
        </tr>
        <tr>
            <td>UMT.MediaLocationForExport</td>
            <td>Folder path for the exported media files when <code>UMT.ExportMediaAsUrls</code> is set to <code>false</code>.</td>
            <td>The default value is <code>{outputFolder}\Files</code></td>
        </tr>
        <tr>
            <td>UMT.MediaLocationForJson</td>
            <td>Relative or absolute folder path that will be used for files in the generated JSON when <code>UMT.ExportMediaAsUrls</code> is set to <code>false</code>. Leave it empty to use the automatically generated absolute path of exported files.</td>
            <td>The default value is <code>.\Files</code></td>
        </tr>
        <tr>
            <td>UMT.RichTextMediaLinkFormat</td>
            <td>Format for transforming media URLs that are refenreced in Rich Text fields.</td>
            <td>The default value is <code>~/getmedia/{0}/{1}.{2}</code> where <code>{0}</code> is media item ID, <code>{1}</code> is file name, and <code>{2}</code> is file extension.</td>
        </tr>
        <tr>
            <td>UMT.TrimLongMediaFolderPaths</td>
            <td>Enables truncation of long media paths on the file system.</td>
            <td>The default value is <code>true</code></td>
        </tr>
        <tr>
            <td>&lt;pipelines&gt;</td>
            <td>&lt;umt.ExtractContent&gt;</td>
            <td>The pipeline responsible for reading content from Sitecore, mapping it to UMT models and serializing as JSON files to the file system. <br/>If you would like to customize any of the processors in this pipeline or introduce a new processor to extract additional data or do custom transformations, you can patch processors in this pipeline.</td>
            <td>A custom processor can be added as a new element under <code>&lt;umt.ExtractContent&gt;</code>. Processors run in the same order as they are listed in the config file. </td>
        </tr>
        <tr>
            <td rowspan="5">&lt;umt&gt;</td>
            <td>&lt;channelMapping&gt;</td>
            <td>List of channels that will be available for selection when running an export. Each channel has a list of attributes corresponding to [channel fields in Xperience by Kentico](https://docs.xperience.io/xp/developers-and-admins/configuration/website-channel-management)</td>
            <td>Channel is a required field as it will be used for linking content types and content items to it.</code></td>
        </tr>
        <tr>
            <td>&lt;languageMapping&gt;</td>
            <td>List of languages for mapping between Sitecore languages (sourceId) and Xperience by kentico languages (targetId). If you have any existing languages in the target Xperience by Kentico instance, add them to this list.</td>
            <td></td>
        </tr>
        <tr>
            <td rowspan="2">&lt;contentMapping&gt;</td>
            <td>&lt;pageRoots&gt; is the list of page sub-tress in your Sitecore instance. This list is used for reference field mapping and automatic resolving of Content Item vs Page Item reference fields. If a linked item in a reference field is not under one of these roots, the reference field will use Content Item reference format.</td>
            <td></td>
        </tr>
        <tr>            
            <td>&lt;excludedSubtrees&gt; is a list of content paths that will be skipped and excluded from the export.</td>
            <td>For example, you may want to exclude some non-content config items such as sitemaps or Sitecore-specific settings.</td>
        </tr>
        <tr>
            <td>&lt;mediaMapping&gt;</td>
            <td>List of media libraries available for selection when running a media export. </td>
            <td>Media library is required when running a media export because the exported media items will be linked to it.</code></td>
        </tr>
        <tr>
            <td>&lt;log4net&gt;</td>
            <td>&lt;appender&gt; and &lt;logger&gt;</td>
            <td>Standard &lt;log4net&gt; configuration for UMT log files.</td>
            <td>By default UMT logs will be written to <code>$(dataFolder)/logs/UMT.log.{date}.txt</code></td>
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
