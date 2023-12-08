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

Follow the steps below to run the Migration toolkit:

1. Download the package from this repository.
2. Install the package on your Sitecore instance.
3. Configure the options in the `App_Config/Include/Migration.Toolkit/Migration.Toolkit.Sitecore.config` configuration file.
4. Navigate to the URL `/sitecore/MigrationToolkit.aspx`.
5. Fill in the channel details, select root paths and types of data you would like to migrate.
6. Click the button and wait for the process to run. Generated JSON files are stored in the folder `App_Data/migration/` by default.
7. Download the generated package and extract it to the folder with the `Migration.Toolkit.CLI.exe` tool.
7. Run the `Migration.Toolkit.CLI.exe migrate` command.
    * The following example shows the command with all parameters for complete migration:

        ```powershell
        Migration.Toolkit.CLI.exe  migrate --siteId 1 --culture en-US --sites --users --settings-keys --page-types --pages --attachments --contact-management --forms --media-libraries --data-protection --countries
        ```

8. Observe the command line output. The command output is also stored into a log file (`logs\log-<date>.txt` under the output directory by default), which you can review later.
9. Review the migration protocol, which provides information about the result of the migration, lists required manual steps, etc.

    * You can find the protocol in the location specified by the `MigrationProtocolPath` key in the `appsettings.json` configuration file.
    * For more information, see [`Migration.Toolkit.CLI/MIGRATION_PROTOCOL_REFERENCE.md`](./Migration.Toolkit.CLI/MIGRATION_PROTOCOL_REFERENCE.md).

Data is now migrated to the target Xperience by Kentico instance according to your configuration. See [`Migration.Toolkit.CLI/README.md`](./Migration.Toolkit.CLI/README.md) for detailed information about the migration CLI, configuration options, instructions related to individual object types, and manual migration steps.

<!-- CONTRIBUTING -->
## Contributing

See [`CONTRIBUTING.md`](./CONTRIBUTING.md) to learn how to file issues, start discussions, and begin contributing.

When submitting issues, please provide all available information about the problem or error. If possible, include the command line output log file and migration protocol generated for your `Migration.Toolkit.CLI.exe migrate` command.

<!-- LICENSE -->
## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Questions & Support

See the [Kentico home repository](https://github.com/Kentico/Home/blob/master/README.md) for more information about the products and general advice on submitting questions.
