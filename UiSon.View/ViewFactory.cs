// UiSon, by Cameron Gale 2022

using System.Reflection;
using System.Text.RegularExpressions;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class ViewFactory
    {
        private readonly IHaveProject _hasProject;

        public ViewFactory(IHaveProject hasProject)
        {
            _hasProject = hasProject ?? throw new ArgumentNullException(nameof(hasProject));
        }

        /// <summary>
        /// Creates an Element View
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="autoGenerateMemberAttributes"></param>
        /// <param name="manager"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public ElementView MakeElementView(string name,
                                           Type type,
                                           bool autoGenerateMemberAttributes,
                                           ElementManager manager)
        {
            Dictionary<string, IUiValueView>? tagToView = null;

            IUiValueView? mainView;

            if (type.IsEnum || type.IsArray || type.IsPrimitive || type == typeof(string))
            {
                mainView = MakeView(type, autoGenerateMemberAttributes, null, null, null);
            }
            // do this seperatly so we can grab the tags and ignore buffers. There's a bit of wasted work, but I'd rather not have so much dupe code
            else if (type.GetConstructor(Array.Empty<Type>()) != null)
            {
                // check for collection
                var collectionInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

                // collections
                if (collectionInterface != null)
                {
                    var entryType = collectionInterface.GetGenericArguments().FirstOrDefault();

                    if (entryType != null)
                    {
                        // attributes
                        UiSonCollectionAttribute? collectionAttribute = type.GetCustomAttribute<UiSonCollectionAttribute>();

                        mainView = new CollectionValueView(this,
                                                           type,
                                                           autoGenerateMemberAttributes,
                                                           entryType,
                                                           type.GetCustomAttribute<UiSonUiAttribute>() ?? GetDefaultUiAttribute(entryType),
                                                           collectionAttribute?.IsModifiable ?? true,
                                                           0,
                                                           null,
                                                           collectionAttribute?.DisplayMode ?? DisplayMode.Vertial,
                                                           null,
                                                           (collectionAttribute?.IncludeMembers ?? true)
                                                                ? MakeMemberViews(type, autoGenerateMemberAttributes, out tagToView)
                                                                : Array.Empty<IReadWriteView>());
                    }
                    else
                    {
                        mainView = new StaticView($"{type}'s entry type was null.", true, 0);
                    }
                }
                // structs and classes
                else
                {
                    mainView = new EncapsulatingView(type,
                                 0,
                                 null,
                                 DisplayMode.Vertial,
                                 null,
                                 MakeMemberViews(type, autoGenerateMemberAttributes, out tagToView));
                }
            }
            else
            {
                mainView = new StaticView($"MakeElementView unhandled type: {type}", true, 0);
            }

            return new ElementView(name, mainView, tagToView ?? new Dictionary<string, IUiValueView>(), manager);
        }

        /// <summary>
        /// Makes an <see cref="IUiValueView"/> representing the type.
        /// Invalid if the type cannot be created without parameters, or is a collection who's
        /// entry type cannot be created without parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        /// <returns>a <see cref="IUiValueView"/> representing the type, or null if the type is not supported.</returns>
        public IUiValueView MakeView(Type type,
                                     bool autoGenerateMemberAttributes,
                                     MemberInfo? info,
                                     UiSonUiAttribute? uiAttribute,
                                     UiSonCollectionAttribute? collectionAttribute)
        {
            string? name = null;

            if (info != null)
            {
                name = info.Name;
                // unfourunatly compiler dependant, but it's just cosmetic so it shouldn't really matter
                name = Regex.IsMatch(name, "<.+>k__BackingField") ? name.Substring(1, name.Length - 17) : name;
            }
            else
            {
                name = GetTypeName(type);
            }

            var valueMemberInfo = info == null ? null : new ValueMemberInfo(info);

            if (uiAttribute == null)
            {
                // enums without attributes make a selector instead of the normal default because I
                // don't want to add new entries to the project's array dict and risk name conflicts from the user
                if (type.IsEnum)
                {
                    var converter = new Map<string, string>();

                    var strippedType = Nullable.GetUnderlyingType(type) ?? type;
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

                    return new SelectorValueView(new PrimitivishUiValueView(typeof(int), 0, name, UiType.Selector, valueMemberInfo), converter, valueMemberInfo);
                }

                uiAttribute = GetDefaultUiAttribute(type);
            }

            var nonNullableType = Nullable.GetUnderlyingType(type) ?? type;

            // make view from type
            if (nonNullableType.IsEnum || nonNullableType.IsPrimitive || nonNullableType == typeof(decimal) || nonNullableType == typeof(string))
            {
                var view = new PrimitivishUiValueView(type, uiAttribute.DisplayPriority, name, uiAttribute.Type, valueMemberInfo);
                return DecorateViewFromAttribute(type, view, uiAttribute, valueMemberInfo);
            }
            else
            {
                var isModifiable = collectionAttribute?.IsModifiable ?? true;
                var displayMode = collectionAttribute?.DisplayMode ?? (uiAttribute as UiSonMemberElementUiAttribute)?.DisplayMode ?? DisplayMode.Vertial;
                var includeCollectionMembers = collectionAttribute?.IncludeMembers ?? false;

                if (nonNullableType.IsArray)
                {
                    return new NullBufferValueView(uiAttribute.DisplayPriority,
                                                   name,
                                                   valueMemberInfo,
                                                   () => new ArrayValueView(this,
                                                                            type,
                                                                            uiAttribute,
                                                                            isModifiable,
                                                                            uiAttribute.DisplayPriority,
                                                                            name,
                                                                            displayMode,
                                                                            valueMemberInfo));
                }
                else if (nonNullableType.IsValueType || nonNullableType.GetConstructor(Array.Empty<Type>()) != null)
                {
                    return MakeEncapsulatingView(type,
                                                 autoGenerateMemberAttributes,
                                                 uiAttribute.DisplayPriority,
                                                 name,
                                                 displayMode,
                                                 isModifiable,
                                                 includeCollectionMembers,
                                                 uiAttribute,
                                                 valueMemberInfo);
                }
            }

            return new StaticView($"Unhandled type in MakeView: {type}", true, uiAttribute.DisplayPriority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="autoGenerateMemberAttributes"></param>
        /// <param name="tagToView"></param>
        /// <returns></returns>
        private IUiValueView MakeEncapsulatingView(Type type,
                                                   bool autoGenerateMemberAttributes,
                                                   int displayPriority,
                                                   string name,
                                                   DisplayMode displayMode,
                                                   bool collectionIsModifiable,
                                                   bool includeCollectionMembers,
                                                   UiSonUiAttribute? collectionEntryUiAttribute,
                                                   ValueMemberInfo? valueMemberInfo)
        {
            // check for collection
            var collectionInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

            // collections
            if (collectionInterface != null)
            {
                var entryType = collectionInterface.GetGenericArguments().FirstOrDefault();

                return entryType == null
                    ? new StaticView($"{type}'s entry type was null.", true, displayPriority)
                    : new NullBufferValueView(displayPriority,
                                              name,
                                              valueMemberInfo,
                                              () => {
                                                   var view = new CollectionValueView(this,
                                                                             type,
                                                                             autoGenerateMemberAttributes,
                                                                             entryType,
                                                                             collectionEntryUiAttribute ?? GetDefaultUiAttribute(entryType),
                                                                             collectionIsModifiable,
                                                                             displayPriority,
                                                                             null,
                                                                             displayMode,
                                                                             valueMemberInfo,
                                                                             includeCollectionMembers
                                                                                ? MakeMemberViews(type, autoGenerateMemberAttributes, out _)
                                                                                : Array.Empty<IReadWriteView>());

                                                  view.TrySetValue(Activator.CreateInstance(type));

                                                  return view;
                                              }); 
            }
            else if (type.IsValueType)// structs
            {
                return new EncapsulatingView(type,
                                             displayPriority,
                                             name,
                                             displayMode,
                                             valueMemberInfo,
                                             MakeMemberViews(type, autoGenerateMemberAttributes, out _));
            }
            else if (type.IsClass)
            {
                return new NullBufferValueView(displayPriority,
                                               name,
                                               valueMemberInfo,
                                               () => {
                                                   var view = new EncapsulatingView(type,
                                                                                       displayPriority,
                                                                                       null,
                                                                                       displayMode,
                                                                                       valueMemberInfo,
                                                                                       MakeMemberViews(type, autoGenerateMemberAttributes, out _));

                                                   view.TrySetValue(Activator.CreateInstance(type));

                                                   return view;
                                               });
            }

            return new StaticView($"Unhandled type in make encapsulating view: {type}.",
                        true,
                        displayPriority);
        }

        private IReadWriteView[] MakeMemberViews(Type type,
                                               bool autoGenerateMemberAttributes,
                                               out Dictionary<string, IUiValueView> tagToView)
        {
            tagToView = new Dictionary<string, IUiValueView>();
            var members = new List<IReadWriteView>();

            // check for group and text block attributes
            var groupAttributesByName = new Dictionary<string, UiSonGroupAttribute>();
            var groups = new Dictionary<string, List<IReadWriteView>>();
            var textBlockAttributes = new List<UiSonTextBlockAttribute>();

            foreach (var att in type.GetCustomAttributes(true))
            {
                if (att is UiSonGroupAttribute groupAtt)
                {
                    if (groupAttributesByName.ContainsKey(groupAtt.Name))
                    {
                        var repeatGroupErrorView = new StaticView($"A group with the name \"{groupAtt.Name}\" has already been defined.", true, groupAtt.Priority);

                        if (string.IsNullOrEmpty(groupAtt.GroupName))
                        {
                            members.Add(repeatGroupErrorView);
                        }
                        else
                        {
                            if (groups.ContainsKey(groupAtt.GroupName))
                            {
                                groups[groupAtt.GroupName].Add(repeatGroupErrorView);
                            }
                            else
                            {
                                groups.Add(groupAtt.GroupName, new List<IReadWriteView> { repeatGroupErrorView });
                            }
                        }
                    }
                    else
                    {
                        groups.Add(groupAtt.Name, new List<IReadWriteView>());
                        groupAttributesByName.Add(groupAtt.Name, groupAtt);
                    }
                }
                else if (att is UiSonTextBlockAttribute textBlockAtt)
                {
                    textBlockAttributes.Add(textBlockAtt);
                }
            }

            // add TextBlocks
            foreach (var textBlockAtt in textBlockAttributes)
            {
                var newTextBlockView = new StaticView(textBlockAtt.Text, false, textBlockAtt.Priority);

                if (string.IsNullOrWhiteSpace(textBlockAtt.GroupName))
                {
                    members.Add(newTextBlockView);
                }
                else
                {
                    if (groups.ContainsKey(textBlockAtt.GroupName))
                    {
                        groups[textBlockAtt.GroupName].Add(newTextBlockView);
                    }
                    else
                    {
                        groups.Add(textBlockAtt.GroupName, new List<IReadWriteView> { newTextBlockView });
                    }
                }
            }

            // check for collection
            var collectionInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

            // public members (may generate attribute)
            foreach (var memberInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                           .OfType<MemberInfo>()
                                           .Concat(type.GetFields(BindingFlags.Public | BindingFlags.Instance)))
            {
                UiSonUiAttribute? memberUiAttribute = memberInfo.GetCustomAttribute<UiSonUiAttribute>();
                var tags = memberInfo.GetCustomAttributes<UiSonTagAttribute>();

                if (autoGenerateMemberAttributes || memberUiAttribute != null || tags.Any())
                {
                    UiSonCollectionAttribute? memberCollectionAttribute = memberInfo.GetCustomAttribute<UiSonCollectionAttribute>();

                    var newView = MakeView(memberInfo.GetUnderlyingType(),
                                           autoGenerateMemberAttributes,
                                           memberInfo,
                                           memberUiAttribute ?? GetDefaultUiAttribute(memberInfo.GetUnderlyingType()),
                                           memberCollectionAttribute);

                    // tags
                    foreach (var tag in memberInfo.GetCustomAttributes<UiSonTagAttribute>())
                    {
                        tagToView.AddOrReplace(tag.Name, newView);
                    }

                    if ((memberUiAttribute != null)
                         &&
                         (collectionInterface == null ? true : memberCollectionAttribute?.IncludeMembers ?? false))
                    {
                        if (memberUiAttribute?.GroupName == null)
                        {
                            members.Add(newView);
                        }
                        else
                        {
                            if (!groups.ContainsKey(memberUiAttribute.GroupName))
                            {
                                groups.Add(memberUiAttribute.GroupName, new List<IReadWriteView>());
                                groupAttributesByName.Add(memberUiAttribute.GroupName, new UiSonGroupAttribute(memberUiAttribute.GroupName));
                            }

                            groups[memberUiAttribute.GroupName].Add(newView);
                        }
                    }
                }
            }

            // private members (must have attribute)
            var privateMemberToView = new Dictionary<MemberInfo, IReadWriteView>();

            foreach (var memberInfo in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                                           .OfType<MemberInfo>()
                                           .Concat(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)))
            {
                UiSonUiAttribute? memberUiAttribute = memberInfo.GetCustomAttribute<UiSonUiAttribute>();
                var tags = memberInfo.GetCustomAttributes<UiSonTagAttribute>();

                if (memberUiAttribute != null || tags.Any())
                {
                    UiSonCollectionAttribute? memberCollectionAttribute = memberInfo.GetCustomAttribute<UiSonCollectionAttribute>();

                    var newView = MakeView(memberInfo.GetUnderlyingType(),
                                           autoGenerateMemberAttributes,
                                           memberInfo,
                                           memberUiAttribute ?? GetDefaultUiAttribute(memberInfo.GetUnderlyingType()),
                                           memberCollectionAttribute);

                    privateMemberToView.Add(memberInfo, newView);

                    // tags
                    foreach (var tag in memberInfo.GetCustomAttributes<UiSonTagAttribute>())
                    {
                        tagToView.AddOrReplace(tag.Name, newView);
                    }

                    if ((memberUiAttribute != null)
                        &&
                        (collectionInterface == null ? true : memberCollectionAttribute?.IncludeMembers ?? false))
                    {
                        if (memberUiAttribute?.GroupName == null)
                        {
                            members.Add(newView);
                        }
                        else
                        {
                            if (!groups.ContainsKey(memberUiAttribute.GroupName))
                            {
                                groups.Add(memberUiAttribute.GroupName, new List<IReadWriteView>());
                                groupAttributesByName.Add(memberUiAttribute.GroupName, new UiSonGroupAttribute(memberUiAttribute.GroupName));
                            }

                            groups[memberUiAttribute.GroupName].Add(newView);
                        }
                    }
                }
            }

            // if there were no attributes defined, add all fields
            if ((!members.Any() && !groups.Any()))
            {
                foreach (var memberInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (privateMemberToView.ContainsKey(memberInfo))
                    {
                        members.Add(privateMemberToView[memberInfo]);
                    }
                    else
                    {
                        members.Add(MakeView(memberInfo.GetUnderlyingType(),
                                             autoGenerateMemberAttributes,
                                             memberInfo,
                                             GetDefaultUiAttribute(memberInfo.GetUnderlyingType()),
                                             null));
                    }
                }
            }

            // add groups
            foreach (var groupAtt in groupAttributesByName)
            {
                    var newGroupVM = new GroupView(groupAtt.Value.Priority,
                               groupAtt.Value.Name,
                               groupAtt.Value.DisplayMode,
                               groups[groupAtt.Key].OrderByDescending(x => x.DisplayPriority).ToArray());

                    if (string.IsNullOrEmpty(groupAtt.Value.GroupName))
                    {
                        members.Add(newGroupVM);
                    }
                    else
                    {
                        // all missing group parent groups were added earlier, so it's here
                        groups.First(x => x.Key == groupAtt.Value.GroupName).Value.Add(newGroupVM);
                    }
            }

            return members.OrderByDescending(x => x.DisplayPriority).ToArray();
        }

        /// <summary>
        /// Decorates a view based on its ui attribute
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private IUiValueView DecorateViewFromAttribute(Type type, IUiValueView decorated, UiSonUiAttribute Attribute, ValueMemberInfo? info)
        {
            if (Attribute is UiSonSelectorUiAttribute selectorUiAttribute)
            {
                Map<string, string>? converter = null;

                if (selectorUiAttribute.OptionsArrayName == null)
                {
                    converter = new Map<string, string>();
                }
                else
                {
                    // converter map
                    var optionsArray = _hasProject.Project.StringArrays.FirstOrDefault(x => x.Key == selectorUiAttribute.OptionsArrayName).Value;

                    if (optionsArray == null)
                    {
                        return new StaticView($"{nameof(UiSonSelectorUiAttribute)} {info?.Name}: {selectorUiAttribute.OptionsArrayName ?? "null"} is not an exsisting {nameof(UiSonArrayAttribute)}.", true, selectorUiAttribute.DisplayPriority);
                    }

                    if (selectorUiAttribute.IdentifiersArrayName == null)
                    {
                        converter = MakeSelectorMap(optionsArray, optionsArray);
                    }
                    else
                    {
                        var identifiersArray = _hasProject.Project.StringArrays.FirstOrDefault(x => x.Key == selectorUiAttribute.IdentifiersArrayName).Value;

                        if (identifiersArray == null)
                        {
                            return new StaticView($"{nameof(UiSonSelectorUiAttribute)} {info?.Name}: {selectorUiAttribute.IdentifiersArrayName ?? "null"} is not an exsisting {nameof(UiSonArrayAttribute)}.", true, selectorUiAttribute.DisplayPriority);
                        }

                        converter = MakeSelectorMap(optionsArray, identifiersArray);
                    }
                }

                var selectorView = new SelectorValueView(decorated, converter, info);

                // element selectors
                if (selectorUiAttribute is UiSonElementSelectorUiAttribute elementSelectorUiAttribute)
                {
                    return new ReferenceSelectorUiValueView(selectorView,
                                                          new ElementReferenceView(_hasProject,
                                                                                   elementSelectorUiAttribute.ElementName,
                                                                                   elementSelectorUiAttribute.IdentifierTagName),
                                                          info);
                }

                return selectorView;
            }
            else if (Attribute is UiSonSliderUiAttribute sliderUiAttribute)
            {
                return new RangeUiValueView(decorated,
                                            sliderUiAttribute.Min,
                                            sliderUiAttribute.Max,
                                            sliderUiAttribute.Precision,
                                            sliderUiAttribute.IsVertical,
                                            info);
            }
            else if (Attribute is UiSonTextEditUiAttribute textEditUiAttribute)
            {
                if (!string.IsNullOrWhiteSpace(textEditUiAttribute.RegexValidation))
                {
                    return new RegexValueView(decorated, textEditUiAttribute.RegexValidation, info);
                }
            }
            else if (Attribute is UiSonCheckboxUiAttribute checkboxUiAttribute)
            {
                if (type != typeof(bool) && type != typeof(bool?) && type != typeof(string))
                {
                    return new StaticView($"{info?.Name}: {nameof(UiSonCheckboxUiAttribute)} does not support type {type}.", true, checkboxUiAttribute.DisplayPriority);
                }
            }

            return decorated;
        }

        /// <summary>
        /// returns the default UiSonUiAttribute for the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private UiSonUiAttribute GetDefaultUiAttribute(Type? type)
        {
            // base attributes on non-nullables.
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(double) || type == typeof(string))
            {
                return new UiSonTextEditUiAttribute();
            }
            if (type.IsPrimitive)
            {
                if (type == typeof(object))
                {
                    return new UiSonLabelUiAttribute();
                }
                else if (type == typeof(bool))
                {
                    return new UiSonCheckboxUiAttribute();
                }
                else
                {
                    return new UiSonTextEditUiAttribute();
                }
            }
            else 
            {
                var collectionInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

                if (collectionInterface != null)
                {
                    var entryType = collectionInterface.GetGenericArguments().FirstOrDefault();

                    return GetDefaultUiAttribute(entryType);
                }

                if (type.GetConstructor(Array.Empty<Type>()) != null)
                {
                    return new UiSonMemberElementUiAttribute();
                }
            }

            return new UiSonLabelUiAttribute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        private Map<string, string> MakeSelectorMap(IEnumerable<string> options, IEnumerable<string> identifiers)
        {
            var newMap = new Map<string, string>();

            var optionIt = options.GetEnumerator();
            var idIt = identifiers.GetEnumerator();

            while (optionIt.MoveNext() && idIt.MoveNext())
            {
                if (optionIt.Current != null
                    && idIt.Current != null)
                {
                    newMap.Add(optionIt.Current, idIt.Current);
                }
            }

            return newMap;
        }

        /// <summary>
        /// Gets a type's name, taking into account generics.
        /// </summary>
        /// <param name="type">Type to get the name of.</param>
        /// <returns>The type's name as a string.</returns>
        private string GetTypeName(Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetTypeName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }
    }
}
