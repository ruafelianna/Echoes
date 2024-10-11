<p align="center">
  <img src="/img/icon.webp" width="100"/>
  <h1 align="center">Echoes</h1>
  <p align="center">
    Simple type safe translations for Avalonia
  </p>
</p>

### Getting Started
Add references to the following packages:
```xml
<PackageReference Include="Echoes" Version=".."/>
<PackageReference Include="Echoes.Generator" Version=".."/>
```

Specify translations files (Embedded Resources, Source Generator)
```xml
<ItemGroup>
    <!-- Include all .toml files as embedded resources (so we can dynamically load them at runtime) -->
    <EmbeddedResource Include="**\*.toml" />

    <!-- Specify invariant files that are fed into the generator (Echoes.Generator) -->
    <AdditionalFiles Include="Translations\Strings.toml" />
</ItemGroup>
```

### Translation Files
Translations are loaded from `.toml` files. The invariant file is **special** as it's structure included configuration data. 
Language files are identified by `_{lang}.toml` postfix. 

```
Strings.toml
Strings_de.toml
Strings_es.toml
```

You can split translations in multiple toml files. 

```
FeatureA.toml
FeatureA_de.toml
FeatureA_es.toml

FeatureB.toml
FeatureB_de.toml
FeatureB_es.toml
```

### File Format
#### Example: Strings.toml
```toml
[echoes_config]
generated_class_name = "Strings"
generated_namespace = "Echoes.SampleApp.Translations"

[translations]
hello_world = 'Hello World'
greeting = 'Hello {0}, how are you?'
```

#### Example: Strings_de.toml
```toml
hello_world = 'Hallo Welt'
greeting = 'Hallo {0}, wie geht es dir?'
```

### Why is it named "Echoes"?
The library is named after the Pink Floyd song [Echoes](https://en.wikipedia.org/wiki/Echoes_(Pink_Floyd_song)).
