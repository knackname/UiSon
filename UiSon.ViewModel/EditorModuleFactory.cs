// UiSon, by Cameron Gale 2021

using System;
using System.Collections;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementManagers">The collection of element managers</param>
        public EditorModuleFactory(IEnumerable<ElementManager> elementManagers, Notifier notifier)
        {
            _elementManagers = elementManagers ?? throw new ArgumentNullException(nameof(elementManagers));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        /// <summary>
        /// Makes an <see cref="IEditorModule"/> based on a type
        /// </summary>
        /// <param name="type">Type the element represents</param>
        /// <param name="initialValue">Initial json string value of the element</param>
        /// <returns></returns>
        public IEditorModule MakeMainElement(Type type, string initialValue = null)
        {
            _notifier.StartCashe();
            var mainElement = new GroupVM(MakeGroups(type));
            _notifier.EndCashe();

            if (initialValue == null)
            {
                mainElement.Read(Activator.CreateInstance(type));
            }
            else
            {
                mainElement.Read(JsonSerializer.Deserialize(initialValue, type));
            }

            return mainElement;
        }

        public CollectionEntryVM MakeCollectionEntryVM(Collection<CollectionEntryVM> parent, Type entryType, Visibility modifyVisability, UiSonGenericEnumerableAttribute enumerableAttribute, IUiSonUiAttribute uiAttribute)
        {
            // don't apply enumerable attributes to types witout parameterless constructors or that aren't enumerables
            if (entryType.GetConstructor(new Type[] { }) == null
                || entryType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)) == null)
            {
                enumerableAttribute = null;
            }

            var editor = uiAttribute == null ? MakeEditorModule(entryType) : MakeEditorModule(uiAttribute, entryType, null, enumerableAttribute);

            if (editor == null)
            {
                _notifier.Notify($"Failed to create an entry of type {entryType}", "Entry Fail");
                return null;
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
        public IEditorModule MakeEditorModule(Type type, string name = null, int priority = 0, MemberInfo info = null)
        {
            // null
            if (type == null) { return null; }

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
                        return new TextEditVM(new StringElement(type, info), name, priority);
                    case ValueishType._bool:
                        return new CheckboxVM(new ValueElement<bool>(type, info), name, priority);
                }
            }

            // enumerables
            if (type.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>)) != null)
            {
                return MakeCollection(type, name, priority, info);
            }

            // other classes and structs
            return MakeMemberElement(type, name, priority, info);
        }

        /// <summary>
        /// Makes an <see cref="IEditorModule"/> from an <see cref="IUiSonUiAttribute"/>
        /// </summary>
        /// <param name="attribute">The attribute</param>
        /// <param name="info">The member info</param>
        /// <returns>An IEditorModule selected based on the attribute</returns>
        private IEditorModule MakeEditorModule(IUiSonUiAttribute attribute, Type type, MemberInfo info = null, UiSonGenericEnumerableAttribute enumerableAttribute = null)
        {
            // first check for enumerables
            var enumerableInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            var hasParamaterlessConstructor = type.GetConstructor(new Type[] { }) != null;

            // enumerable attributes
            if (enumerableAttribute != null)
            {
                if (enumerableInterface == null)
                {
                    _notifier.Notify($"{info} has a {nameof(UiSonGenericEnumerableAttribute)} but its type doesn't impliment a generic IEnumerable<T>",
                                     $"Invalid {nameof(UiSonGenericEnumerableAttribute)}");
                }
                else if (!hasParamaterlessConstructor)
                {
                    _notifier.Notify($"{info ?? type} has a {nameof(UiSonGenericEnumerableAttribute)} but doesn't impliment a parameterless constructor",
                    $"Invalid {nameof(UiSonGenericEnumerableAttribute)}");
                }
                else
                {
                    return MakeCollection(type,
                        info?.Name,
                        attribute.Priority,
                        info,
                        enumerableAttribute.IsModifiable,
                        enumerableAttribute,
                        attribute,
                        enumerableAttribute.DisplayMode,
                        enumerableAttribute.CollectionType);
                }
            }
            // enumerable interfaces with paramaterless constructors
            else if (hasParamaterlessConstructor && enumerableInterface != null)
            {
                enumerableAttribute = new UiSonGenericEnumerableAttribute();

                return MakeCollection(type,
                                      info?.Name,
                                      attribute.Priority,
                                      info,
                                      enumerableAttribute.IsModifiable,
                                      enumerableAttribute,
                                      attribute,
                                      enumerableAttribute.DisplayMode,
                                      enumerableAttribute.CollectionType);
            }

            // then go by ui attribute
            if (attribute is UiSonTextEditUiAttribute textEdit)
            {
                return new TextEditVM(new StringElement(type, info, textEdit.RegexValidation), info?.Name, textEdit.Priority);
            }
            else if (attribute is UiSonElementSelectorUiAttribute eleSel)
            {
                // find manager
                var manager = _elementManagers.FirstOrDefault(x => x.ElementName == eleSel.ElementName);

                // ignore if invalid, needs warning
                if (manager == null) { return null; }

                return MakeElementSelector(manager, type, eleSel.IdentifierTagName, eleSel.Options, eleSel.Identifiers, eleSel.Priority, info);
            }
            else if (attribute is UiSonSelectorUiAttribute sel)
            {
                return new SelectorVM<string>(new StringElement(type, info),
                                             info?.Name, sel.Priority,
                                             MakeStringSelectorMap(sel.Options, sel.Identifiers ?? sel.Options, type));
            }
            else if (attribute is UiSonCheckboxUiAttribute chk)
            {
                if (type.IsAssignableFrom(typeof(string)) || type.IsAssignableFrom(typeof(bool)))
                {
                    return new CheckboxVM(new ValueElement<bool>(type, info), info?.Name, chk.Priority);
                }
                else
                {
                    _notifier.Notify($"{type?.ToString() ?? "null"} is not compatable with {nameof(UiSonCheckboxUiAttribute)}", "Checkbox");
                }
            }
            else if (attribute is UiSonEnumSelectorUiAttribute enums)
            {
                if (enums.EnumType != null)
                {
                    var strippedType = Nullable.GetUnderlyingType(enums.EnumType) ?? enums.EnumType;

                    if (strippedType.IsEnum)
                    {
                        return MakeEnumSelector(enums.EnumType, type, info?.Name, enums.Priority, info);
                    }
                }

                _notifier.Notify($"{info} has {nameof(UiSonEnumSelectorUiAttribute.EnumType)} {enums.EnumType?.ToString() ?? "null"} which is not a valid enum type", "Enum Selector");
            }
            else if (attribute is UiSonMemberElementAttribute member)
            {
                return MakeMemberElement(type, info?.Name, member.Priority, info);
            }

            return null;
        }

        private IEditorModule MakeMemberElement(Type type, string name, int priority, MemberInfo info)
        {
            if (type.IsValueType)
            {
                return MakeMainElement(type);
            }

            if (type.GetConstructor(new Type[] { }) == null)
            {
                _notifier.Notify($"{type} lacks a parameterless constructor.", "Member Element");
                return null;
            }

            return new NullableVM(() =>
            {
                _notifier.StartCashe();
                var newRefVM = new RefVM(MakeGroups(type), info, name ?? type.Name, priority);
                _notifier.EndCashe();
                return newRefVM;
            },
            type, name ?? type.Name, priority, info);
        }

        private NullableVM MakeCollection(Type type,
            string name = null,
            int priority = 0,
            MemberInfo info = null,
            bool modifiable = true,
            UiSonGenericEnumerableAttribute enumerableAttribute = null,
            IUiSonUiAttribute uiAttribute = null,
            DisplayMode displayMode = DisplayMode.Vertial,
            CollectionStyle collectionStyle = CollectionStyle.Stack)
        {
            if (type.GetConstructor(new Type[] { }) == null)
            {
                _notifier.Notify($"{type} lacks a parameterless constructor.", "Enumerable Element");
                return null;
            }

            var entryType = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments().FirstOrDefault();

            if (entryType == null)
            {
                _notifier.Notify($"Unable to find entry type for {type}", "Enumerable Element");
                return null;
            }
            else if (!entryType.IsValueType && !entryType.IsAssignableFrom(typeof(string)) && entryType.GetConstructor(new Type[] { }) == null)
            {
                _notifier.Notify($"{type}'s entry type lacks a parameterless constructor.", "Enumerable Element");
                return null;
            }

            return new NullableVM(() => new CollectionVM(new ObservableCollection<CollectionEntryVM>(),
                                              entryType,
                                              this,
                                              uiAttribute,
                                              name ?? type.Name,
                                              priority,
                                              info,
                                              modifiable,
                                              displayMode,
                                              collectionStyle,
                                              enumerableAttribute),
                                   type, name ?? type.Name, priority, info);
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
            // make map
            var map = new Map<string, int>();

            var names = Enum.GetNames(enumType);
            int nameIndex = 0;
            foreach (var value in Enum.GetValues(enumType))
            {
                // all enum values are int castable, so use that for common handling
                map.Add(names[nameIndex++], (int)value);
            }

            return new SelectorVM<int>(new ValueElement<int>(valueType, info),
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
                var newElement = new StringElement(valueType, info);

                return new ElementSelectorVM(newElement,
                                           new SelectorVM<bool>(newElement, info?.Name, priority,
                                                MakeTSelectorMap<bool>(options, identifiers ?? options)),
                                           identifingMember,
                                           info,
                                           manager);
            }
            // treat enums as ints for common handling
            else if (identifierType == typeof(int)
                     || (identifierType?.IsEnum ?? false))
            {
                var newElement = new StringElement(valueType, info);

                return new ElementSelectorVM(newElement,
                                           new SelectorVM<int>(newElement, info?.Name, priority,
                                                MakeTSelectorMap<int>(options, identifiers ?? options)),
                                           identifingMember,
                                           info,
                                           manager);
            }
            // default to using strings
            else
            {
                var newElement = new StringElement(valueType, info);
                return new ElementSelectorVM(newElement,
                                            new SelectorVM<string>(newElement, info?.Name, priority,
                                                MakeStringSelectorMap(options, identifiers ?? options, identifierType ?? valueType)),
                                            identifingMember,
                                            info,
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
            var members = type.GetMembers().Where(x => x is PropertyInfo || x is FieldInfo).ToArray();

            foreach (var member in members)
            {
                UiSonGenericEnumerableAttribute enumerableAttribute = null;
                IUiSonUiAttribute uiAttribute = null;

                // grab attributes
                foreach (var att in member.GetCustomAttributes())
                {
                    if (att is UiSonGenericEnumerableAttribute enumAtt)
                    {
                        enumerableAttribute = enumAtt;
                    }
                    else if (att is IUiSonUiAttribute uiAtt)
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
                    _notifier.Notify($"{member} lacks a {nameof(IUiSonUiAttribute)}", "Standalone UiSonEnumerableAttribute");
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
