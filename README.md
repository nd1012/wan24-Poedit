# wan24-Poedit

**NOTE**: This project is not going to be maintenanced anymore and has no direct replacement. Instead of using gettext PO files for l10n in your .NET app, you may want to use [`wan24-I8NTool`](https://github.com/nd1012/wan24-I8NTool) to create i8n files for use with `wan24-I8N`.

This library contains an adapter for using Poedit PO files with `wan24-Core` 
translation helpers. [Karambolo.PO](https://github.com/adams85/po) is being 
referenced for that, 'cause no PO format parsing is implemented in 
`wan24-Poedit`.

## Usage

### How to get it

This library is available as 
[NuGet package "wan24-Poedit"](https://www.nuget.org/packages/wan24-Poedit/).

### Loading a translation from Poedit PO format

```cs
// From a PO file
PoeditTranslationTerms terms = PoeditTranslationTerms.FromFile("/path/to/file.po");
PoeditTranslationTerms terms = await PoeditTranslationTerms.FromFileAsync("/path/to/file.po");

// From a PO stream
PoeditTranslationTerms terms = PoeditTranslationTerms.FromStream(poStream);
PoeditTranslationTerms terms = await PoeditTranslationTerms.FromStreamAsync(poStream);

// From a PO string
PoeditTranslationTerms terms = PoeditTranslationTerms.FromString(poString);

// From a byte array (UTF-8 encoded PO string)
PoeditTranslationTerms terms = PoeditTranslationTerms.FromBytes(poData);

// From a POCatalog
PoeditTranslationTerms terms = new(poCatalog);
```

The created `terms` instance can be used for the `wan24-Core` `Translation`. 
Please refer to the `wan24-Core` documentation for more details about that.

### Creating a PO file from source code

This library only contains PO reading helpers for working with the 
`wan24-Core` translation helpers. For creating a PO file you might want to use 
the dotnet tool 
[wan24-PoeditParser](https://github.com/nd1012/wan24-PoeditParser), which is 
able to parse C# source code (and any other source language code) and create a 
PO file, which can be used with Poedit. Also the `wan24-PoeditParser` can be 
used as custom extractor for the Poedit GUI.
