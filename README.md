<p align="center">
	<img src="https://raw.githubusercontent.com/knackname/UiSon/main/UiSon/Images/logo.png" alt="UiSon" width="180" /><br>
	<b>Version 0.1</b><br>
	A combination attribute library and UI</br>
  for editing json asset files from C# projects
</p>

### About

UiSon is a tool designed for C# projects using json assets, use it to generate a validated ui for editing them based on the class they represent. Including the UiSon.Attributes lib into your project lets you to use UiSon attributes on your json-serializable classes and influence how they appear in the editor. The word 'null' is a reserved keyword that will set the value to null rather than a string."

### How it works

UiSon uses reflection to look for its attributes on classes in .dll files it is given. If one is found, it creates viewmodels with member info from your class so they can write to instances of it. Using System.Text.Json it then can serialize and deserialize intances of those classes.

### Installation

Reference UiSon.Attributes.dll (also available through NuGet) in your project and use the attributes to define ui modules for your classes. After you compile your project UiSon can read these attributes to construct its editor.

### Contribute

• Use UiSon and provide feedback  
• Spread the word in your developer communities  
• Donate to support development of this and my current projects (comming soon)

### Documentation

## Attributes

UiSon.Attributes has two kinds of attributes: Ui attributes, all with the suffix 'Ui', and identifier attributes.

Ui attributes are placed directly on the property/field and designate a Ui module to represent its value. Only one Ui attribute can be used per property/field. All Ui attributes have a int 'priority' and a string 'groupName' paramiter. 'priority' defines the order in which Ui modules are displayed in ascending order. 'groupName' will place the moduel into the group of that name.

Identifier attributes identify or define information used by UiSon and its Ui moduels.

## Customization

Once a .uis file is created, it can be edited to customize the user experience. This includes setting up relative assembly paths, disabling the modification of the assembly list, and setting a custom logo and description.