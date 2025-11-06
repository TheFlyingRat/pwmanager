# PWMan

## Information

### Description

PWMan is a CLI based password manager. It demonstrates OOP principles such as design patterns, inheritance, polymorphism, etc.

### Dependencies

PWMan uses `Konscious.Security.Cryptography.Argon2` to derive Argon2id keys. It is available on NuGet.

### Usage

#### Typical Flow

1. Run the application.
2. Type `help` to see all available commands
3. When finished, type `exit`.

#### Create a Vault and Add a Password

1. Run `create` and follow the steps. The password is the only required field here.
2. The created vault is unlocked as a quality of life thing.
3. Inspecting the save file (probably `vault.dat` if default) you'll see the metadata and encrypted values.
4. Run `add` and follow the steps to add an entry to your database. For the default entry type, no values are required.
5. When finished, the entry is automatically saved to the vault, and the vault saves to disk - inspect the save file again.

#### Unlocking and Retrieval

1. Assuming you've got a vault already, run `load <type> <path>`. Running without the parameters will show you the available values.
2. Enter the vault password or you'll be given an error.
3. Type `list` or `list details` to see all the entries in your vault.
4. Use `get <id>` to see all data about a particular entry from its ID.
5. `delete <id>` can be used to delete an entry from the vault.

## Gotchas

- The program is autosaving, but a save command exists anyways.
- Both `unload` and `lock` locks and saves the vault therefore you don't need to type lock or save before running it.
- You may have multiple vaults, though only one opened at a time as Vault is singleton.
- Most fields are optional unless the program asks you again.
- Depending on your computer's power, Argon2 might take forever - the default iteration count is 3.
- Caesar encryption is included to demonstrate polymorphism, by no means is it actually good.
- The in-memory repository is ephemeral therefore cannot be loaded, only created for each runtime.
- The program doesn't have security in mind, rather just object oriented programming principles.
- The `lv` command is a shortcut custom command that is equivalent to `load json vault.dat`.

## Acknowledgements

In some files I have credited articles including StackOverflow.
I also wish to thank the authors of Konscious
