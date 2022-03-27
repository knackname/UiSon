<p align="center">
	<img src="https://raw.githubusercontent.com/knackname/UiSon/Images/logo.png" alt="UiSon" width="180" /><br>
	<b>Version 0.1</b><br>
	A combination attribute library and UI</br>
  for editing json asset files from C# projects
</p>

### About

UiSon is a tool designed for C# projects using json assets. Including the UiSon.Attributes lib into your project lets you to use attributes on your json-serializable classes and influence how they appear in the editor. 'null' is a reserved keyword that will set the value to null rather than a string."

### How it works

UiSon uses reflection to look for it's attributes on classes in .dll files it's given. If one is found, it creates viewmodels with member info from you class so they can write to instances of it. Using System.Text.Json it then can serialize and deserialize intances of those classes.

### Installation

Reference UiSon.Attributes.dll in your project and used the attributes to define elements for your classes. After you compile your project UiSon can read these attributes to construct its editor.

### Documentation

To DO

### Contribute

• Use UiSon and provide feedback 
• Spread the word in your developer communities  
• Donate to support development (comming soon)
