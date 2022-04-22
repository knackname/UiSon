// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.Notify;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Creates editor modules
    /// </summary>
    public class EditorModuleFactory
    {
        /// <summary>
        /// The collection of element managers
        /// </summary>
        private readonly IEnumerable<ElementManager> _elementManagers;

        private Notifier _notifier;

        private ElementTemplateSelector _templateSelector;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementManagers">The collection of element managers</param>
        public EditorModuleFactory(IEnumerable<ElementManager> elementManagers, Notifier notifier, ElementTemplateSelector templateSelector)
        {
            _elementManagers = elementManagers ?? throw new ArgumentNullException(nameof(elementManagers));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
        }

        /// <summary>
        /// Makes an <see cref="IEditorModule"/> based on a type
        /// </summary>
        /// <param name="type">Type the element represents</param>
        /// <param name="initialValue">Initial json string value of the element</param>
        /// <returns></returns>
        public IEditorModule MakeMainElement(Type type)
        {
            _notifier.StartCashe();
            var mainElement = new GroupVM(MakeGroups(type));
            _notifier.EndCashe();

            mainElement.Read(Activator.CreateInstance(type));

            return mainElement;
        }

        public CollectionEntryVM MakeCollectionEntryVM(Collection<CollectionEntryVM> parent, Type entryType, Visibility modifyVisability, UiSonCollectionAttribute enumerableAttribute, UiSonUiAttribute uiAttribute)
        {
            // don't apply enumerable attributes to non-collections
            if (entryType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)) == null)
            {
                enumerableAttribute = null;
            }

            var editor = uiAttribute == null ? MakeEditorModule(entryType) : MakeEditorModule(uiAttribute, entryType, null, enumerableAttribute);

            if (editor == null)
            {
                return new CollectionEntryVM(parent, new BadDataVM($"Failed to create an entry of type {entryType}"), modifyVisability);
            }
            else
            {
                return new CollectionEntryVM(parent, editor, modifyVisability);
            }
        }

        /// <summary>
        /// Makes an <see cref="IEditorModule"/> from a type
        /// </summary>
        /// <param name="type">The type the editor is based on</param>
        /// <param name="name">The name of the editor</param>
        /// <param name="priority">The priority of the editor</param>
        /// <param name="info">Info for the editor</param>
        /// <returns>An IEditorModule selected based on the type</returns>
        public IEditorModule MakeEditorModule(Type type, string name = null, int priority = 0, MemberInfo info = null, DisplayMode? displayMode = null)
        {
            // null
            if (type == null) { return new BadDataVM("Attempt to make editor module from type null"); }

            // strip nullables for value types
            var strippedType = Nullable.GetUnderlyingType(type) ?? type;

            // enums
            if (strippedType.IsEnum)
            {
                return MakeEnumSelector(strippedType, type, name, priority, info);
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
                        return new TextEditVM(new StringElement(type, info == null ? null : new ValueMemberInfo(info)), name, priority);
                    case ValueishType._bool:
                        return new CheckboxVM(new ValueElement<bool>(type, info == null ? null : new ValueMemberInfo(info)), name, priority);
                }
            }

            // collections
            if (type.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ICollection<>)) != null)
            {
                return MakeCollection(type, name, priority, info);
            }

            // other classes and structs
            return MakeMemberElement(type, name, priority, info, displayMode ?? DisplayMode.Vertial);
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
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)) != null;
            var hasParamaterlessConstructor = type.GetConstructor(Array.Empty<Type>()) != null;

            // collection attributes
            if (enumerableAttribute != null)
            {
                if (!hasCollectionInterface)
                {
                    return new BadDataVM($"{name}: has a {nameof(UiSonCollectionAttribute)} but type {type} doesn't impliment a generic ICollection<T>");
                }
                else if (!hasParamaterlessConstructor)
                {
                    return new BadDataVM($"{name}: has a {nameof(UiSonCollectionAttribute)} but doesn't impliment a parameterless constructor");
                }
                else if (attribute is UiSonMultiChoiceUiAttribute)
                {
                    return new BadDataVM($"{name}: has a {nameof(UiSonCollectionAttribute)} and a {nameof(UiSonMultiChoiceUiAttribute)}");
                }
                else
                {
                    return MakeCollection(type,
                        name,
                        attribute.Priority,
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
                if (attribute is UiSonMultiChoiceUiAttribute multi)
                {
                    var entryType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)).GetGenericArguments().FirstOrDefault();

                    if (entryType == null)
                    {
                        return new BadDataVM($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: Unable to find entry type for {type}");
                    }
                    else if (!entryType.IsValueType && !entryType.IsAssignableFrom(typeof(string)) && entryType.GetConstructor(Array.Empty<Type>()) == null)
                    {
                        return new BadDataVM($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: {type}'s entry type lacks a parameterless constructor.");
                    }

                    var optionUi = new List<CheckboxVM>();

                    var options = multi.Options;
                    if (options == null)
                    {
                        if (multi.EnumType == null
                            || !multi.EnumType.IsEnum)
                        {
                            return new BadDataVM($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: {multi.EnumType?.ToString() ?? "null"} is not an Enum type");
                        }
                        else
                        {
                            options = Enum.GetNames(multi.EnumType).ToArray();
                        }
                    }

                    // make checkboxes for each option
                    foreach (var option in options)
                    {
                        optionUi.Add(new CheckboxVM(new ValueElement<bool>(typeof(bool), null), option, 0));
                    }

                    return new BorderedVM( new MultiChoiceVM(optionUi, type, entryType, info == null ? null : new ValueMemberInfo(info),
                                             name, multi.Priority, multi.DisplayMode));
                }

                // default to using a normal collection ui
                enumerableAttribute = new UiSonCollectionAttribute();

                return MakeCollection(type,
                                      name,
                                      attribute.Priority,
                                      info,
                                      enumerableAttribute.IsModifiable,
                                      enumerableAttribute,
                                      attribute,
                                      enumerableAttribute.DisplayMode);
            }

            // then go by ui attribute
            if (attribute is UiSonTextEditUiAttribute textEdit)
            {
                return new TextEditVM(new StringElement(type,
                                                        info == null ? null : new ValueMemberInfo(info),
                                                        textEdit.RegexValidation),
                                      name,
                                      textEdit.Priority);
            }
            else if (attribute is UiSonSliderUiAttribute slider)
            {
                var element = new RangeElement(type,
                                               info == null ? null : new ValueMemberInfo(info),
                                               slider.Min,
                                               slider.Max,
                                               slider.Precision);

                return new GroupVM(new List<IEditorModule>()
                                   {new TextEditVM(element, null, 0),
                                    new SliderVM(element, slider.IsVertical, null, 0, _templateSelector)},
                                   name,
                                   slider.Priority,
                                   DisplayMode.Vertial,
                                   false);
            }
            else if (attribute is UiSonElementSelectorUiAttribute eleSel)
            {
                var manager = _elementManagers.FirstOrDefault(x => x.ElementName == eleSel.ElementName);

                if (manager == null)
                {
                    return new BadDataVM($"{nameof(UiSonElementSelectorUiAttribute)}: Unable to find UiSonElement {eleSel.ElementName}");
                }

                return MakeElementSelector(manager, type, eleSel.IdentifierTagName, eleSel.Options, eleSel.Identifiers, eleSel.Priority, info);
            }
            else if (attribute is UiSonSelectorUiAttribute sel)
            {
                return new SelectorVM<string>(new StringElement(type, info == null ? null : new ValueMemberInfo(info)),
                                             name, sel.Priority,
                                             MakeStringSelectorMap(sel.Options, sel.Identifiers ?? sel.Options, type));
            }
            else if (attribute is UiSonCheckboxUiAttribute chk)
            {
                if (type.IsAssignableFrom(typeof(string)) || type.IsAssignableFrom(typeof(bool)))
                {
                    return new CheckboxVM(new ValueElement<bool>(type, info == null ? null : new ValueMemberInfo(info)), name, chk.Priority);
                }
                else
                {
                    return new BadDataVM($"{nameof(UiSonCheckboxUiAttribute)} {name}: {type?.ToString() ?? "null"} is not compatable");
                }
            }
            else if (attribute is UiSonEnumSelectorUiAttribute enums)
            {
                if (enums.EnumType != null)
                {
                    var strippedType = Nullable.GetUnderlyingType(enums.EnumType) ?? enums.EnumType;

                    if (strippedType.IsEnum)
                    {
                        return MakeEnumSelector(enums.EnumType, type, name, enums.Priority, info);
                    }
                }

                return new BadDataVM($"{nameof(UiSonEnumSelectorUiAttribute)} {name}: {type?.ToString() ?? "null"} is not an enum type");
            }
            else if (attribute is UiSonMemberElementAttribute member)
            {
                return MakeMemberElement(type, name, member.Priority, info, member.DisplayMode);
            }
            else if (attribute is UiSonMultiChoiceUiAttribute)
            {
                return new BadDataVM($"{nameof(UiSonMultiChoiceUiAttribute)} {name}: Member lacks ICollection implimentation");
            }

            return new BadDataVM($"Unhandled Ui Attribute {attribute}");
        }

        private IEditorModule MakeMemberElement(Type type, string name, int priority, MemberInfo info, DisplayMode displayMode)
        {
            name = name ?? type.Name;

            if (type.IsValueType)
            {
                return new RefVM(MakeGroups(type),
                                            type,
                                            info == null ? null : new ValueMemberInfo(info),
                                            name,
                                            priority,
                                            displayMode);
            }

            if (type.GetConstructor(Array.Empty<Type>()) == null)
            {
                return new BadDataVM($"Member Element {name}: {type} lacks a parameterless constructor.");
            }

            return new NullableVM(() =>
                {
                    _notifier.StartCashe();
                    var newRefVM = new RefVM(MakeGroups(type),
                                             type,
                                             info == null ? null : new ValueMemberInfo(info),
                                             name ?? type.Name,
                                             priority,
                                             displayMode);
                    _notifier.EndCashe();
                    return newRefVM;
                },
                type, name ?? type.Name, priority, info == null ? null : new ValueMemberInfo(info));
        }

        private NullableVM MakeCollection(Type type,
            string name = null,
            int priority = 0,
            MemberInfo info = null,
            bool modifiable = true,
            UiSonCollectionAttribute enumerableAttribute = null,
            UiSonUiAttribute uiAttribute = null,
            DisplayMode displayMode = DisplayMode.Vertial)
        {
            Func<IEditorModule> makeEditorFunc = null;

            name = name ?? type.Name;

            if (type.GetConstructor(Array.Empty<Type>()) == null)
            {
                makeEditorFunc = () => new BadDataVM($"Collection Element for {name}: {type} lacks a parameterless constructor.");
            }
            else
            {
                var entryType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)).GetGenericArguments().FirstOrDefault();

                if (entryType == null)
                {
                    makeEditorFunc = () => new BadDataVM($"Collection Element for {name}: Unable to find entry type for {type}");
                }
                else if (!entryType.IsValueType && !entryType.IsAssignableFrom(typeof(string)) && entryType.GetConstructor(Array.Empty<Type>()) == null)
                {
                    makeEditorFunc = () => new BadDataVM($"Collection Element for {name}: {type}'s entry type lacks a parameterless constructor.");
                }
                else
                {
                    makeEditorFunc = () => new CollectionVM(new ObservableCollection<CollectionEntryVM>(),
                                                            entryType,
                                                            type,
                                                            this,
                                                            uiAttribute,
                                                            name,
                                                            priority,
                                                            info == null ? null : new ValueMemberInfo(info),
                                                            modifiable,
                                                            displayMode,
                                                            enumerableAttribute,
                                                            _templateSelector);
                }
            }

            return new NullableVM(makeEditorFunc, type, name ?? type.Name, priority, info == null ? null : new ValueMemberInfo(info));
        }

        /// <summary>
        /// Makes a selector with options from the enumType
        /// enumType must be an enum.
        /// </summary>
        /// <param name="enumType">An enum type</param>
        /// <param name="valueType">The value type</param>
        /// <param name="info">The info</param>
        /// <returns>A selector with options from the enumType</returns>
        private SelectorVM<int> MakeEnumSelector(Type enumType, Type valueType, string name, int priority, MemberInfo info = null)
        {
            // strip off nullable layers
            enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;

            // make map
            var map = new Map<string, int>();

            var names = Enum.GetNames(enumType);
            int nameIndex = 0;
            foreach (var value in Enum.GetValues(enumType))
            {
                // all enum values are int castable, so use that for common handling
                map.Add(names[nameIndex++], (int)value);
            }

            return new SelectorVM<int>(new ValueElement<int>(valueType, info == null ? null : new ValueMemberInfo(info)),
                                name, priority,
                                map ?? new Map<string, int>());
        }

        /// <summary>
        /// Make an element selector
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="valueType"></param>
        /// <param name="identifierTagName"></param>
        /// <param name="options"></param>
        /// <param name="identifiers"></param>
        /// <param name="priority"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private ElementSelectorVM MakeElementSelector(ElementManager manager, Type valueType, string identifierTagName, IEnumerable<string> options, IEnumerable<string> identifiers, int priority, MemberInfo info = null)
        {
            // find identifing member if there is one
            MemberInfo identifingMember = null;

            if (identifierTagName != null)
            {
                foreach (var member in manager.ManagedType.GetMembers())
                {
                    var identifierTag = member.GetCustomAttributes(typeof(UiSonTagAttribute), false)
                                          .Select(x => x as UiSonTagAttribute)
                                          .FirstOrDefault(x => x.Name == identifierTagName);

                    if (identifierTag != null && member.GetUnderlyingType() == valueType)
                    {
                        identifingMember = member;
                        break;
                    }
                }
            }

            // find type of identifier (if there is one, otherwise it's null)
            var idUnderlyingType = identifingMember?.GetUnderlyingType();

            var identifierType = idUnderlyingType == null
                ? null
                : Nullable.GetUnderlyingType(idUnderlyingType) ?? idUnderlyingType;

            // make the selector
            if (identifierType == typeof(bool))
            {
                var newElement = new StringElement(valueType, info == null ? null : new ValueMemberInfo(info));

                return new ElementSelectorVM(newElement,
                                           new SelectorVM<bool>(newElement, info?.Name, priority,
                                                MakeTSelectorMap<bool>(options, identifiers ?? options)),
                                           identifingMember == null ? null : new ValueMemberInfo(identifingMember),
                                           info == null ? null : new ValueMemberInfo(info),
                                           manager);
            }
            // treat enums as ints for common handling
            else if (identifierType == typeof(int)
                     || (identifierType?.IsEnum ?? false))
            {
                var newElement = new StringElement(valueType, info == null ? null : new ValueMemberInfo(info));

                return new ElementSelectorVM(newElement,
                                           new SelectorVM<int>(newElement, info?.Name, priority,
                                                MakeTSelectorMap<int>(options, identifiers ?? options)),
                                           identifingMember == null ? null : new ValueMemberInfo(identifingMember),
                                           info == null ? null : new ValueMemberInfo(info),
                                           manager);
            }
            // default to using strings
            else
            {
                var newElement = new StringElement(valueType, info == null ? null : new ValueMemberInfo(info));
                return new ElementSelectorVM(newElement,
                                            new SelectorVM<string>(newElement, info?.Name, priority,
                                                MakeStringSelectorMap(options, identifiers ?? options, identifierType ?? valueType)),
                                            identifingMember == null ? null : new ValueMemberInfo(identifingMember),
                                            info == null ? null : new ValueMemberInfo(info),
                                            manager);
            }
        }

        /// <summary>
        /// Makes the regions for a reference element
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<IEditorModule> MakeGroups(Type type)
        {
            // Read attributes
            var groupAttributesByName = new Dictionary<string, UiSonGroupAttribute>();

            foreach (var att in type.GetCustomAttributes(typeof(UiSonGroupAttribute), true))
            {
                if (att is UiSonGroupAttribute group)
                {
                    groupAttributesByName.Add(group.Name, group);
                }
            }

            // Read properties with attributes
            var editors = new List<IEditorModule>();
            var groups = new Dictionary<string, List<IEditorModule>>();

            var members = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                              .Where(x => x.CanWrite && x.CanRead)
                              .OfType<MemberInfo>()
                              .Concat(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                          .OfType<MemberInfo>())
                              .ToArray();

            foreach (var member in members)
            {
                UiSonCollectionAttribute enumerableAttribute = null;
                UiSonUiAttribute uiAttribute = null;

                // grab attributes
                foreach (var att in member.GetCustomAttributes())
                {
                    if (att is UiSonCollectionAttribute enumAtt)
                    {
                        enumerableAttribute = enumAtt;
                    }
                    else if (att is UiSonUiAttribute uiAtt)
                    {
                        uiAttribute = uiAtt;
                    }
                }

                if (uiAttribute != null)
                {
                    var newEditor = MakeEditorModule(uiAttribute, member.GetUnderlyingType(), member, enumerableAttribute);

                    // if no group, add directly
                    if (uiAttribute.GroupName == null)
                    {
                        editors.Add(newEditor);
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
                else if (enumerableAttribute != null)
                {
                    _notifier.Notify($"{member} lacks a {nameof(UiSonUiAttribute)}", "Standalone UiSonEnumerableAttribute");
                }
            }

            // if no attributes, then guess at all members
            if (editors.Count == 0 && groups.Count == 0)
            {
                foreach (var member in members)
                {
                    editors.Add(MakeEditorModule(member.GetUnderlyingType(), member.Name, 0, member));
                }
            }
            // otherwise generate the groups
            else
            {
                // Construct regions
                foreach (var group in groups)
                {
                    var priority = 0;
                    var displayMode = DisplayMode.Vertial;

                    if (groupAttributesByName.ContainsKey(group.Key))
                    {
                        var att = groupAttributesByName[group.Key];
                        priority = att.Priority;
                        displayMode = att.DisplayMode;
                    }

                    editors.Add(new BorderedVM(new GroupVM(group.Value.OrderBy(x => x.Priority).ToList(), group.Key, priority, displayMode)));
                }
            }

            // editors will be null when their attributes have invalid inputs, ignore them
            return editors.Where(x => x != null).OrderBy(x => x.Priority).ToArray();
        }

        /// <summary>
        /// Makes a map for a string selector
        /// </summary>
        /// <param name="options"></param>
        /// <param name="identifiers"></param>
        /// <param name="underlyingType"></param>
        /// <returns></returns>
        private Map<string, string> MakeStringSelectorMap(IEnumerable<string> options, IEnumerable<string> identifiers, Type underlyingType)
        {
            var newMap = new Map<string, string>();

            if (options == null || identifiers == null)
            {
                return newMap;
            }

            var optionIt = options.GetEnumerator();
            var idIt = identifiers.GetEnumerator();

            while(optionIt.MoveNext() && idIt.MoveNext())
            {
                var parsed = idIt.Current.ParseAs(underlyingType);

                if (optionIt.Current != null && parsed != null)
                {
                    newMap.Add(optionIt.Current, idIt.Current);
                }
            }

            return newMap;
        }

        /// <summary>
        /// Makes a map for a selector of type t
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        private Map<string, T> MakeTSelectorMap<T>(IEnumerable<string> options, IEnumerable<string> identifiers)
        {
            var newMap = new Map<string, T>();

            if (options == null || identifiers == null)
            {
                return newMap;
            }

            var optionIt = options.GetEnumerator();
            var idIt = identifiers.GetEnumerator();

            while (optionIt.MoveNext() && idIt.MoveNext())
            {
                var parsed = idIt.Current.ParseAs(typeof(T));
                if (optionIt.Current != null && parsed != null)
                {
                    newMap.Add(optionIt.Current, (T)parsed);
                }
            }

            return newMap;
        }
    }
}
