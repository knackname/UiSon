<p align="center">
	<img src="https://raw.githubusercontent.com/knackname/UiSon/main/UiSon/Images/logo.png" alt="UiSon" width="180" /><br>
	<b>Version 0.1</b><br>
	A combination attribute library and UI</br>
  for editing json asset files from C# projects
</p>

### About

UiSon is a tool designed for C# projects using json assets. Including the UiSon.Attributes lib into your project lets you to use attributes on your json-serializable classes and influence how they appear in the editor. 'null' is a reserved keyword that will set the value to null rather than a string."

### How it works

UiSon uses reflection to look for it's attributes on classes in .dll files it's given. If one is found, it creates viewmodels with member info from you class so they can write to instances of it. Using System.Text.Json it then can serialize and deserialize intances of those classes.

### Installation

Reference UiSon.Attributes.dll (also available through Nuget) in your project and used the attributes to define elements for your classes. After you compile your project UiSon can read these attributes to construct its editor.

### Contribute

• Use UiSon and provide feedback  
• Spread the word in your developer communities  
• Donate to support development of this and my current projects (comming soon)

### Documentation

## Attributes

UiSon.Attributes has two different kinds of attributes: Ui attributes, all with the suffix 'Ui', and identifier attributes.

Ui attributes are placed directly on the field/property they are ment to represent. Only one Ui attribute can be used per field/property. All Ui attributes have a int 'priority' and a string 'groupName' paramiter. 'priority' defines the order in which Ui modules are displayed ascending. 'groupName' will place the moduel into the group of that name.

Identifier attributes identify or define information used by UiSon and its Ui moduels.

# UiSonCheckboxUi

Designates the field/property to be represented by a checkbox Ui in UiSon. This Ui is only effective for string, bool and bool? values or user defined types assignable by bool.

# UiSonTextEditUi

Designates the field/property to be represented by a textedit Ui in UiSon. The input string will be parsed to the represented field/property's type. Also includes an optional string 'regexValidation' paramiter which will validate all input and only allow those satisfying the regex.

# UiSonSelectorUi

Designates the field/property to be represented by a selector Ui in UiSon. Takes a string[] Options and a string[] Identifiers. Options and identifiers of the same index will be pared, selecting an option will cause the pared identifier's value to be saved. If no identifiers are provided, The value of the option itself will be used as its identifier. Identifiers will be parsed to the represented field/property's type.

# UiSonEnumSelectorUi

Designates the member to be represented by a selector Ui in UiSon. Takes a Type 'enumType' which must be the type of an enum. Selector options will be from the enum's defined values. If the enumType is a nullable type, 'null' will be included as an option.

# UiSonElementSelectorUi

Designates the member to be represented by a selector Ui in UiSon. Selector options will be from the elements created in UiSon of the designated elementName along with any additional options defined in this attribute. In the case of a selected element sharing a name with an additional option, the element will take priority. A UiSonTagAttribute can be defined on a member the designated UiSonElement to select an identifier. The saved value will then be the value of the identifier. If no identifing tag is provided, the saved value will be the name of the selected element.

# UiSonMultiChoiceUi

Designates the member to be represented by a multi choice Ui in UiSon. Use this attribute on an ICollection{T}, not compatable with UiSonCollectionAttribute. Options and identifiers of the same index will be pared, selecting an option will cause the pared identifier's value to be saved. If no identifiers are provided, The value of the option itself will be saved parsed at the type T.

