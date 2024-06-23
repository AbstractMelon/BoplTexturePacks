![TexturePackLoader](background.png)

# TexturePackLoader

The assets for bopl texture pack loader & creator!

## Overview

The bopl texturepackloader is a BepInEx plugin that allows you to load and replace textures in your Unity game using texture packs. These texture packs are stored as ZIP files and contain a mapping file to specify which textures to replace.

## Features

- Load texture packs from ZIP files.
- Replace in-game textures based on mappings defined in a JSON file.
- Configurable texture pack directory path.

## Installation

1. **Download and install BepInEx** if you haven't already. Follow the [BepInEx installation guide](https://bepinex.github.io/bepinex_docs/master/articles/user_guide/installation/index.html).
2. **Place the plugin in the plugins folder** Put the plugin inside `BepInEx/plugins`

## Usage

1. **Create a `TexturePacks` directory** inside `BepInEx/plugins`.
2. **Place your texture pack ZIP files** inside the `TexturePacks` directory. Each ZIP file should contain a `mapping.json` file and the corresponding texture files.

### Example `mapping.json`

```json
{
    "grenade3": "Icon.png"
}
```

### Example ZIP Structure

```plaintext
MyTexturePack.zip
|-- mapping.json
|-- image.png
```

## Configuration

The plugin's configuration file can be found in the `BepInEx/config` directory after the first run. The configuration file allows you to set the path to the texture packs directory.

### Default Configuration

```ini
[General]
## The directory where texture packs are stored.
# Setting type: String
# Default value: TexturePacks
TexturePackPath = TexturePacks
```

## Contributing

If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request.

## License

This project is licensed under the GNU v3 License. See the `LICENSE` file for details.
