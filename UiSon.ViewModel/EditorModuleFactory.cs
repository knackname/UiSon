// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.Notify.Interface;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Creates editor modules
    /// </summary>
    public class EditorModuleFactory
    {
        private static readonly string[] _nullOption = new string[] { "null" };

        public IUiSonProject Project { get; set; }

        private readonly INotifier _notifier;
        private readonly ModuleTemplateSelector _templateSelector;

        public EditorModuleFactory(INotifier notifier,
                                   ModuleTemplateSelector templateSelector)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
        }

        /// <summary>
        /// Makes an <see cref="IEditorModule"/> based on a type
        /// </summary>
        /// <param name="type">Type the element represents</param>
        /// <param name="initialValue">Initial json string value of the element</param>
        /// <returns></returns>
        public IElementEditorTab MakeElementEditorTab(ElementView view, TabControl tabController)
        {
            var mainElement = new GroupModule(MakeEditorModules(view.ElementType), null, 0, DisplayMode.Vertial);
            mainElement.Read(view.Value);
            
            return new ElementEditorTab(view, mainElement, tabController);
        }

        public CollectionEntryModule MakeCollectionEntryVM(CollectionModule parent, Type entryType, UiSonCollectionAttribute enumerableAttribute, UiSonUiAttribute uiAttribute)
        {
            // don't apply enumerable attributes to non-collections
            if (entryType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)) == null)
            {
                enumerableAttribute = null;
            }

            var editor = uiAttribute == null ? MakeEditorModule(entryType) : MakeEditorModule(uiAttribute, entryType, null, enumerableAttribute);

            return new CollectionEntryModule(parent,
                                             editor);
        }

        public IEditorModule MakeEditorModule(Type type, string name = null, int displayPriority = 0, MemberInfo info = null, DisplayMode? displayMode = null)
        {
            // null
            if (type == null) { return MakeErrorTextBlock("Attempt to make editor module from type null", displayPriority); }

            // strip nullables for value types
            var strippedType = Nullable.GetUnderlyingType(type) ?? type;

            // enums
            if (strippedType.IsEnum)
            {
                var converter = new Map<string, string>();

                if (type != strippedType)
                {
                    converter.Add("null", "null");
                }

                var names = Enum.GetNames(strippedType);
                int nameIndex = 0;
                foreach (var value in Enum.GetValues(strippedType))
                {
                    // all enum values are int castable, so use that for common handling
                    converter.Add(names[nameIndex], ((int)value).ToString());
                    nameIndex++;
                }

                return new SelectorModule(new SelectorView(new StringView(type, info == null ? null : new ValueMemberInfo(info)), converter),
                                             _templateSelector,
                                             name,
                                             displayPriority);
            }

            // value types (and string)
            if (TypeLookup.TypeToId.ContainsKey(strippedType))
            {
                // default editor for each value type
                switch (TypeLookup.TypeToId[strippedType])
                {
                    case ValueishType._string:
                    case ValueishType._sbyte:
                    case ValueishType._byte:
                    case ValueishType._short:
                    case ValueishType._ushort:
                    case ValueishType._int:
                    case ValueishType._uint:
                    case ValueishType._long:
                    case ValueishType._ulong:
                    case ValueishType._nint:
                    case ValueishType._nuint:
                    case ValueishType._float:
                    case ValueishType._double:
                    case ValueishType._decimal:
                    case ValueishType._char:
                        return new TextEditModule(new StringView(type, info == null ? null : new ValueMemberInfo(info)),
                                                     _templateSelector,
                                                     name,
                                                     displayPriority);
                    case ValueishType._bool:
                        return new CheckboxModule(new ValueView<bool>(type, info == null ? null : new ValueMemberInfo(info)),
                                                                         _templateSelector,
                                                                         name,
                                                                         displayPriority);
                }
            }

            // collections
            if (type.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ICollection<>)) != null)
            {
                return MakeCollection(type, name, displayPriority, info);
            }

            // other classes and structs
            return MakeMemberElement(type, name, displayPriority, info, displayMode ?? DisplayMode.Vertial);
        }

        /// <summary>
        /// Makes an <see cref="IEditorModule"/> from an <see cref="IUiSonUiAttribute"/>
        /// </summary>
        /// <param name="attribute">The attribute</param>
        /// <param name="info">The member info</param>
        /// <returns>An IEditorModule selected based on the attribute</returns>
        private IEditorModule MakeEditorModule(UiSonUiAttribute attribute, Type type, MemberInfo info = null, UiSonCollectionAttribute enumerableAttribute = null)
        {
            var name = info?.Name ?? type.Name;

            // first check for collections
            var hasCollectionInterface = type.GetInterfaces()
                                             .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                                             != null;

            var hasParamaterlessConstructor = type.GetConstructor(Array.Empty<Type>()) != null;

            // collection attributes
            if (enumerableAttribute != null)
            {
                if (!hasCollectionInterface)
                {
                    return MakeErrorTextBlock($"{name}: has a {nameof(UiSonCollectionAttribute)} but type {type} doesn't impliment a generic ICollection<T>", attribute.DisplayPriority);
                }
                else if (!hasParamaterlessConstructor)
                {
                    return MakeErrorTextBlock($"{name}: has a {nameof(UiSonCollectionAttribute)} but doesn't impliment a parameterless constructor", attribute.DisplayPriority);
                }
                else if (attribute is UiSonMultiChoiceUiAttribute)
                {
                    return MakeErrorTextBlock($"{name}: has a {nameof(UiSonCollectionAttribute)} and a {nameof(UiSonMultiChoiceUiAttribute)}", attribute.DisplayPriority);
                }
                else
                {
                    return MakeCollection(type,
                        name,
                        attribute.DisplayPriority,
                        info,
                        enumerableAttribute.IsModifiable,
                        enumerableAttribute,
                        attribute,
                        enumerableAttribute.DisplayMode);
                }
            }
            // collection interfaces with paramaterless constructors
            else if (hasParamaterlessConstructor && hasCollectionInterface)
            {
                // attributes for collections
                if (attribute is UiSonMultiChoiceUiAttribute multiChoiceAtt)
                {
                    return MakeErrorTextBlock($"ToDo: multi selectors", attribute.DisplayPriority);

                    //var entryType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)).GetGenericArguments().FirstOrDefault();

                    //if (entryType == null)
                    //{
                    //    return MakeErrorTextBlock($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: Unable to find entry type for {type}", multi.Priority);
                    //}
                    //else if (!entryType.IsValueType && !entryType.IsAssignableFrom(typeof(string)) && entryType.GetConstructor(Array.Empty<Type>()) == null)
                    //{
                    //    return MakeErrorTextBlock($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: {type}'s entry type lacks a parameterless constructor");
                    //}

                    //if (multi.OptionsArrayName != null && _stringArrays.ContainsKey(multi.OptionsArrayName))
                    //{
                    //    var optionUi = new List<CheckboxViewModel>();

                    //    // make checkboxes for each option
                    //    foreach (var option in _stringArrays[multi.OptionsArrayName])
                    //    {
                    //        optionUi.Add(new CheckboxViewModel(new ValueView<bool>(typeof(bool), null),
                    //                                           _templateSelector,
                    //                                           option,
                    //                                           0));
                    //    }

                    //    var madeInfo = info == null ? null : new ValueMemberInfo(info);

                    //    var view = new RefView();

                    //    return new NullableViewModel(view,
                    //                                 _templateSelector,
                    //                                 name,
                    //                                 multi.Priority,
                    //                                 type,
                    //                                 () => new MultiChoiceViewModel(optionUi,
                    //                                                                type,
                    //                                                                entryType,
                    //                                                                madeInfo,
                    //                                                                name,
                    //                                                  multi.Priority,
                    //                                                  multi.DisplayMode));
                    //}
                    //else
                    //{
                    //    return MakeErrorTextBlock($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: {multi.OptionsArrayName ?? "null"} is not an exsisting {nameof(UiSonArrayAttribute)}.", multi.Priority);
                    //}
                }

                // default to using a normal collection ui
                enumerableAttribute = new UiSonCollectionAttribute();

                return MakeCollection(type,
                                      name,
                                      attribute.DisplayPriority,
                                      info,
                                      enumerableAttribute.IsModifiable,
                                      enumerableAttribute,
                                      attribute,
                                      enumerableAttribute.DisplayMode);
            }

            // then go by ui attribute
            if (attribute is UiSonTextEditUiAttribute textEditAtt)
            {
                return new TextEditModule(new StringView(type, info == null ? null : new ValueMemberInfo(info), textEditAtt.RegexValidation),
                                             _templateSelector,
                                             name,
                                             textEditAtt.DisplayPriority);
            }
            else if (attribute is UiSonLabelUiAttribute textBlockAtt)
            {
                return new TextBlockModule(new StringView(type, info == null ? null : new ValueMemberInfo(info)),
                                              _templateSelector,
                                              textBlockAtt.DisplayPriority,
                                              ModuleState.Normal);
            }
            else if (attribute is UiSonSliderUiAttribute sliderAtt)
            {
                var element = new RangeView(type,
                                            info == null ? null : new ValueMemberInfo(info),
                                            sliderAtt.Min,
                                            sliderAtt.Max,
                                            sliderAtt.Precision);

                return new GroupModule(new List<IEditorModule>()
                                          {new TextEditModule(element,
                                                                 _templateSelector,
                                                                 null,
                                                                 0),
                                           new SliderModule(element,
                                                               _templateSelector,
                                                               null,
                                                               0,
                                                               sliderAtt.IsVertical)},
                                          name,
                                          sliderAtt.DisplayPriority,
                                          DisplayMode.Vertial);
            }
            else if (attribute is UiSonElementSelectorUiAttribute elementSelectorAtt)
            {
                var manager = Project.ElementManagers.FirstOrDefault(x => x.ElementName == elementSelectorAtt.ElementName);

                if (manager == null)
                {
                    return MakeErrorTextBlock($"{nameof(UiSonElementSelectorUiAttribute)} {name}: {elementSelectorAtt.ElementName} is not an existing {nameof(UiSonElementAttribute)}.", elementSelectorAtt.DisplayPriority);
                }

                if (elementSelectorAtt.OptionsArrayName == null)
                {
                    return MakeElementSelector(manager,
                                               type,
                                               elementSelectorAtt.IdentifierTagName,
                                               MakeSelectorMap(_nullOption, _nullOption, type),
                                               elementSelectorAtt.DisplayPriority,
                                               name,
                                               info);
                }
                else
                {
                    var optionsArray = Project.StringArrays.FirstOrDefault(x => x.Key == elementSelectorAtt.OptionsArrayName).Value;

                    if (optionsArray == null)
                    {
                        return MakeErrorTextBlock($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: {elementSelectorAtt.OptionsArrayName ?? "null"} is not an exsisting {nameof(UiSonArrayAttribute)}.", elementSelectorAtt.DisplayPriority);
                    }

                    Map<string, string> converter = null;

                    if (elementSelectorAtt.IdentifiersArrayName == null)
                    {
                        converter = MakeSelectorMap(optionsArray,
                                                    optionsArray,
                                                    type);
                    }
                    else
                    {
                        var identifiersArray = Project.StringArrays.FirstOrDefault(x => x.Key == elementSelectorAtt.IdentifiersArrayName).Value;

                        if (identifiersArray == null)
                        {
                            return MakeErrorTextBlock($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: {elementSelectorAtt.IdentifiersArrayName ?? "null"} is not an exsisting {nameof(UiSonArrayAttribute)}.", elementSelectorAtt.DisplayPriority);
                        }

                        converter = MakeSelectorMap(optionsArray,
                                                    identifiersArray,
                                                    type);
                    }

                    return MakeElementSelector(manager,
                                               type,
                                               elementSelectorAtt.IdentifierTagName,
                                               converter,
                                               elementSelectorAtt.DisplayPriority,
                                               name,
                                               info);
                }
            }
            else if (attribute is UiSonSelectorUiAttribute selectorAtt)
            {
                var optionsArray = Project.StringArrays.FirstOrDefault(x => x.Key == selectorAtt.OptionsArrayName).Value;

                if (optionsArray == null)
                {
                    return MakeErrorTextBlock($"{nameof(UiSonSelectorUiAttribute)} {name}: {selectorAtt.OptionsArrayName ?? "null"} is not an exsisting {nameof(UiSonArrayAttribute)}.", selectorAtt.DisplayPriority);
                }

                Map<string, string> converter = null;

                if (selectorAtt.IdentifiersArrayName == null)
                {
                    converter = MakeSelectorMap(optionsArray,
                                                optionsArray,
                                                type);
                }
                else
                {
                    var identifiersArray = Project.StringArrays.FirstOrDefault(x => x.Key == selectorAtt.IdentifiersArrayName).Value;

                    if (identifiersArray == null)
                    {
                        return MakeErrorTextBlock($"{nameof(UiSonSelectorUiAttribute)} {name}: {selectorAtt.IdentifiersArrayName ?? "null"} is not an exsisting {nameof(UiSonArrayAttribute)}.", selectorAtt.DisplayPriority);
                    }

                    converter = MakeSelectorMap(optionsArray,
                                                identifiersArray,
                                                type);
                }

                return new SelectorModule(new SelectorView(new StringView(type, info == null ? null : new ValueMemberInfo(info)),
                                                           converter),
                                          _templateSelector,
                                          name,
                                          selectorAtt.DisplayPriority);
            }
            else if (attribute is UiSonCheckboxUiAttribute checkboxAtt)
            {
                if (type.IsAssignableFrom(typeof(string)) || type.IsAssignableFrom(typeof(bool)))
                {
                    return new CheckboxModule(new ValueView<bool>(type, info == null ? null : new ValueMemberInfo(info)),
                                                 _templateSelector,
                                                 name,
                                                 checkboxAtt.DisplayPriority);
                }
                else
                {
                    return MakeErrorTextBlock($"{nameof(UiSonCheckboxUiAttribute)} {name}: {type?.ToString() ?? "null"} is not compatable", checkboxAtt.DisplayPriority);
                }
            }
            else if (attribute is UiSonMemberElementAttribute memberAtt)
            {
                return MakeMemberElement(type, name, memberAtt.DisplayPriority, info, memberAtt.DisplayMode);
            }
            else if (attribute is UiSonMultiChoiceUiAttribute)
            {
                return MakeErrorTextBlock($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: Member lacks ICollection implimentation", attribute.DisplayPriority);
            }

            return MakeErrorTextBlock($"Unhandled Ui Attribute {attribute}", attribute.DisplayPriority);
        }

        private TextBlockModule MakeErrorTextBlock(string message, int priority = 0)
        {
            return new TextBlockModule(new ReadOnlyView(message), _templateSelector, priority, ModuleState.Error);
        }

        private IEditorModule MakeMemberElement(Type type, string name, int displayPriority, MemberInfo info, DisplayMode displayMode)
        {
            name = name ?? type.Name;

            var valueMemberInfo = info == null ? null : new ValueMemberInfo(info);


            if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
            {
                if (type.GetConstructor(Array.Empty<Type>()) == null)
                {
                    return MakeErrorTextBlock($"Member Element {name}: {type} lacks a parameterless constructor.", displayPriority);
                }
                else
                {
                    return new NullableModule(name,
                                              displayPriority,
                                              type,
                                              valueMemberInfo,
                                              _templateSelector,
                                              () => new EncapsulatingModule(type,
                                                                            MakeEditorModules(type),
                                                                            name,
                                                                            0,
                                                                            displayMode));
                }
            }
            else
            {
                return new EncapsulatingModule(type,
                                               MakeEditorModules(type),
                                               name,
                                               displayPriority,
                                               displayMode);
            }
        }

        private NullableModule MakeCollection(Type type,
            string name = null,
            int displayPriority = 0,
            MemberInfo info = null,
            bool modifiable = true,
            UiSonCollectionAttribute enumerableAttribute = null,
            UiSonUiAttribute uiAttribute = null,
            DisplayMode displayMode = DisplayMode.Vertial)
        {
            Func<IEditorModule> makeEditorFunc = null;
            name = name ?? type.Name;
            var valueMemberInfo = info == null ? null : new ValueMemberInfo(info);

            if (type.GetConstructor(Array.Empty<Type>()) == null)
            {
                makeEditorFunc = () => MakeErrorTextBlock($"Collection Element for {name}: {type} lacks a parameterless constructor.", displayPriority);
            }
            else
            {
                var entryType = type.GetInterfaces()
                                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                                    .GetGenericArguments()
                                    .FirstOrDefault();

                if (entryType == null)
                {
                    makeEditorFunc = () => MakeErrorTextBlock($"Collection Element for {name}: Unable to find entry type for {type}", displayPriority);
                }
                else if (!entryType.IsValueType && !entryType.IsAssignableFrom(typeof(string)) && entryType.GetConstructor(Array.Empty<Type>()) == null)
                {
                    makeEditorFunc = () => MakeErrorTextBlock($"Collection Element for {name}: {type}'s entry type lacks a parameterless constructor.", displayPriority);
                }
                else
                {
                    makeEditorFunc = () => new CollectionModule(valueMemberInfo,
                                                                _templateSelector,
                                                                name,
                                                                displayPriority,
                                                                modifiable,
                                                                displayMode,
                                                                type,
                                                                entryType,
                                                                uiAttribute,
                                                                enumerableAttribute,
                                                                this,
                                                                _notifier,
                                                                new NotifyingCollection<IEditorModule>());
                }
            }

            return new NullableModule(name, displayPriority, type, valueMemberInfo, _templateSelector, makeEditorFunc);
        }

        private IEditorModule MakeElementSelector(IElementManager manager,
                                                  Type type,
                                                  string identifierTagName,
                                                  Map<string,string> additionalOptions,
                                                  int displayPriority,
                                                  string name,
                                                  MemberInfo info)
        {
            // find identifing member if there is one
            MemberInfo identifingMember = null;

            if (identifierTagName != null)
            {
                foreach (var member in manager.ElementType.GetAllDataAccessInstances())
                {
                    var identifierTag = member.GetCustomAttributes(typeof(UiSonTagAttribute), false)
                                              .Select(x => x as UiSonTagAttribute)
                                              .FirstOrDefault(x => x.Name == identifierTagName);

                    if (identifierTag != null && type.IsAssignableFrom(member.GetUnderlyingType()))
                    {
                        identifingMember = member;
                        break;
                    }
                }

                if (identifingMember == null)
                {
                    return MakeErrorTextBlock($"Element selector {name}: No {nameof(UiSonTagAttribute)} was found on a property/field of {nameof(manager.ElementType)}", displayPriority);
                }
            }

            // make the selector
            var valueMemberInfo = info == null ? null : new ValueMemberInfo(info);

            return new SelectorModule(new ElementSelectorView(new SelectorView(new StringView(type, valueMemberInfo),
                                                                                  additionalOptions),
                                                                 identifingMember == null ? null : new ValueMemberInfo(identifingMember),
                                                                 valueMemberInfo,
                                                                 manager),
                                         _templateSelector,
                                         name,
                                         displayPriority);
        }

        /// <summary>
        /// Makes editor modules from a type by looking at its attributes or lack thereof
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<IEditorModule> MakeEditorModules(Type type)
        {
            var editorModules = new List<IEditorModule>();
            var groups = new Dictionary<string, List<IEditorModule>>();

            // Read identifier attributes
            var groupAttributesByName = new Dictionary<string, UiSonGroupAttribute>();
            var textBlockAttributes = new List<UiSonTextBlockAttribute>();

            foreach (var att in type.GetCustomAttributes(true))
            {
                if (att is UiSonGroupAttribute groupAtt)
                {
                    if (groupAttributesByName.ContainsKey(groupAtt.Name))
                    {
                        var repeatGroupBadVm = MakeErrorTextBlock($"A group with the name \"{groupAtt.Name}\" has already been defined.", groupAtt.Priority);

                        if (string.IsNullOrEmpty(groupAtt.GroupName))
                        {
                            editorModules.Add(repeatGroupBadVm);
                        }
                        else
                        {
                            if (groups.ContainsKey(groupAtt.GroupName))
                            {
                                groups[groupAtt.GroupName].Add(repeatGroupBadVm);
                            }
                            else
                            {
                                // add missing group parent groups
                                groups.Add(groupAtt.GroupName, new List<IEditorModule> { repeatGroupBadVm });
                            }
                        }
                    }
                    else
                    {
                        groups.Add(groupAtt.Name, new List<IEditorModule>());
                        groupAttributesByName.Add(groupAtt.Name, groupAtt);
                    }
                }
                else if ( att is UiSonTextBlockAttribute textBlockAtt)
                {
                    textBlockAttributes.Add(textBlockAtt);
                }
            }

            // add TextBlocks
            foreach (var textBlockAtt in textBlockAttributes)
            {
                var element = new ReadOnlyView(textBlockAtt.Text);
                var newTextBlockVM = new TextBlockModule(element,
                                                            _templateSelector,
                                                            textBlockAtt.Priority,
                                                            ModuleState.Normal);

                if (string.IsNullOrWhiteSpace(textBlockAtt.GroupName))
                {
                    editorModules.Add(newTextBlockVM);
                }
                else
                {
                    if (groups.ContainsKey(textBlockAtt.GroupName))
                    {
                        groups[textBlockAtt.GroupName].Add(newTextBlockVM);
                    }
                    else
                    {
                        groups.Add(textBlockAtt.GroupName, new List<IEditorModule> { newTextBlockVM });
                    }
                }
            }

            // Read Ui attributes from properties and fields
            foreach (var member in type.GetAllDataAccessInstances())
            {
                UiSonCollectionAttribute collectionAttribute = null;
                UiSonUiAttribute uiAttribute = null;

                // grab attributes
                foreach (var att in member.GetCustomAttributes())
                {
                    if (att is UiSonCollectionAttribute enumAtt)
                    {
                        collectionAttribute = enumAtt;
                    }
                    else if (att is UiSonUiAttribute uiAtt)
                    {
                        uiAttribute = uiAtt;
                    }
                }

                if (uiAttribute != null)
                {
                    var newEditor = MakeEditorModule(uiAttribute, member.GetUnderlyingType(), member, collectionAttribute);

                    // if no group, add directly
                    if (uiAttribute.GroupName == null)
                    {
                        editorModules.Add(newEditor);
                    }
                    else
                    {
                        if (!groups.ContainsKey(uiAttribute.GroupName))
                        {
                            groups.Add(uiAttribute.GroupName, new List<IEditorModule>());
                        }

                        groups[uiAttribute.GroupName].Add(newEditor);
                    }
                }
                else if (collectionAttribute != null)
                {
                    editorModules.Add(MakeErrorTextBlock($"{member} has a {nameof(UiSonCollectionAttribute)}, but lacks a {nameof(UiSonUiAttribute)}"));
                }
            }

            // add groups
            foreach (var groupAtt in groupAttributesByName)
            {
                var newGroupVM = new BorderedModule(new GroupModule(groups[groupAtt.Key],
                                                            groupAtt.Value.Name,
                                                            groupAtt.Value.Priority,
                                                            groupAtt.Value.DisplayMode));

                if (string.IsNullOrEmpty(groupAtt.Value.GroupName))
                {
                    editorModules.Add(newGroupVM);
                }
                else
                {
                    // all missing group parent groups were added earlier, so it's here
                    groups.First(x => x.Key == groupAtt.Value.GroupName).Value.Add(newGroupVM);
                }
            }

            // if no attributes, then guess at all fields (includes backing fields for properties)
            if (editorModules.Count == 0 && groups.Count == 0)
            {
                foreach (var member in type.GetAllFieldInstances())
                {
                    // trim the backingfield off the name if nessisarry
                    var name = Regex.IsMatch(member.Name, "<.+>k__BackingField") ? member.Name.Substring(1, member.Name.Length - 17) : member.Name;

                    editorModules.Add(MakeEditorModule(member.GetUnderlyingType(), name, 0, member));
                }
            }

            return editorModules.OrderByDescending(x => x.DisplayPriority).ToArray();
        }

        private Map<string, string> MakeSelectorMap(IEnumerable<string> options, IEnumerable<string> identifiers, Type underlyingType)
        {
            var newMap = new Map<string, string>();

            if (options == null || identifiers == null)
            {
                return newMap;
            }

            var optionIt = options.GetEnumerator();
            var idIt = identifiers.GetEnumerator();
            var isNullable = !underlyingType.IsValueType || Nullable.GetUnderlyingType(underlyingType) != null;

            while (optionIt.MoveNext() && idIt.MoveNext())
            {
                if ((idIt.Current == null || idIt.Current == "null")
                    && isNullable)
                {
                    newMap.Add("null", "null");
                }
                else
                {
                    var parsed = idIt.Current.ParseAs(underlyingType);
                    if (optionIt.Current != null && parsed != null)
                    {
                        newMap.Add(optionIt.Current, idIt.Current);
                    }
                }
            }

            return newMap;
        }
    }
}
