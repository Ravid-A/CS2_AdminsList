# CS2_AdminsList

WeaponsAllocator plugin for retakes written in C# (.Net) for CounterStrikeSharp.

## Description

The CS2_AdminsList plugin is designed to provide a list of admins for Counter-Strike 2 gameservers. It allows players to view a list of the connected admins, admins can hide themselves from the list by using `/admins 0/1` command.

## Dependencies

This plugin is made to run alongside [CSSPanel](https://github.com/CSSPanel)

## Installation

To install the CS2_AdminsList plugin for Counter-Strike 2 Servers, follow these steps:

1. Download the latest release from the [Releases](https://github.com/Ravid-A/CS2_AdminsList/releases) section of this repository.
2. Extract the downloaded ZIP file.
3. Copy all the extracted files to the `addons/cs2/plugins/CS2_AdminsList` directory in your Counter-Strike 2 server.

Make sure you have the necessary dependencies and the Counter-Strike 2 server set up and running properly.

## Building

To build the CS2_AdminsList plugin from source, follow these steps:

1. Clone the repository: `git clone https://github.com/Ravid-A/CS2_AdminsList.git`
2. Navigate to the project directory: `cd CS2_AdminsList`
3. Open a command prompt or terminal in the project directory.
4. Run the following command to build the project: `dotnet build`

After running the `dotnet build` command, the project will be built, and you will find the compiled plugin files in the `./bin/Debug/net8.0` directory.

## Config

The config file will be generated automaticly by the plugin and will be located where the plugin is.

The connection data for the database will be generated empty and the plugin will raise an exception, make sure to update it or update the one provided with the release.

You can use `/admins_reloadconfig` to update the groups colors but no new sql connection will be made.

### Example Config

```
{
  "Host": "<HOST>",
  "Database": "<DB>",
  "User": "<USER>",
  "Password": "<PASSWORD>",
  "Port": 3306,
  "Groups": {
    "Management": "red",
    "Admin": "green"
  }
}

```

## Contributing

Contributions to the CS2_AdminsList project are welcome. If you find any issues or have any suggestions, please open an issue or submit a pull request.
