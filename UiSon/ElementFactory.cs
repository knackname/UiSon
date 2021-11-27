// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using UiSon.Attribute;
using UiSon.Attribute.Enums;
using UiSon.Element;
using UiSon.Element.Element.Interface;
using UiSon.Extensions;

namespace UiSon
{
    public class ElementFactory
    {
        IEnumerable<ElementManager> _elementManagers;

        public ElementFactory(IEnumerable<ElementManager> elementManagers)
        {
            _elementManagers = elementManagers ?? throw new ArgumentNullException(nameof(elementManagers));
        }

        /// <summary>
        /// Makes the main element for the class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public GroupElement MakeMainElement(Type type, string initialValue = null)
        {
            var elements = new List<IElement>();

            var mainElement = new GroupElement(null, 0, MakeGroups(type));

            if (initialValue != null)
            {
                mainElement.Read(JsonSerializer.Deserialize(initialValue, type));
            }

            return mainElement;
        }

        /// <summary>
        /// Makes an element representing a reference type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public RefElement MakeRefElement(Type type, MemberInfo info) => new RefElement(null, 0, MakeGroups(type), info, type.GetConstructor(new Type[] { }));

        /// <summary>
        /// Makes the regions for a reference element
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<IElement> MakeGroups(Type type)
        {
            // Read attributes
            UiSonElementAttribute elementAttribute;
            var regionAttributes = new Dictionary<string, UiSonGroupAttribute>();

            foreach (var att in type.GetCustomAttributes(typeof(UiSonAttribute), false))
            {
                if (att is UiSonElementAttribute ele)
                {
                    elementAttribute = ele;
                }
                else if (att is UiSonGroupAttribute reg)
                {
                    regionAttributes.Add(reg.Name, reg);
                }
            }

            // Read properties
            var members = new List<IElement>();
            var regions = new Dictionary<string, List<IElement>>();

            foreach (var member in type.GetMembers())
            {
                foreach (var att in member.GetCustomAttributes(typeof(UiSonAttribute), false))
                {
                    // There should be only one per member
                    if (att is UiSonAttribute memAtt)
                    {
                        if (memAtt.RegionName == null)
                        {
                            members.Add(MakeElement(member));
                        }
                        else
                        {
                            if (!regions.ContainsKey(memAtt.RegionName))
                            {
                                regions.Add(memAtt.RegionName, new List<IElement>());
                            }

                            regions[memAtt.RegionName].Add(MakeElement(member));
                        }
                        break;
                    }
                }
            }

            // Construct regions
            foreach (var region in regions)
            {
                var priority = 0;
                var displayMode = DisplayMode.Vertial;
                var alignment = Alignment.Left;
                var groupType = GroupType.Border;

                if (regionAttributes.ContainsKey(region.Key))
                {
                    var att = regionAttributes[region.Key];
                    priority = att.Priority;
                    displayMode = att.DisplayMode;
                    alignment = att.Alignment;
                    groupType = att.GroupType;
                }

                members.Add(new GroupElement(region.Key, priority, region.Value, displayMode, alignment, groupType));
            }

            return members;
        }

        /// <summary>
        /// Creates an element from a member info
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private IElement MakeElement(MemberInfo memberInfo)
        {
            // Read attributes
            UiSonCollectionAttribute collection = null;
            UiSonAttribute attribute = null;

            foreach (var att in memberInfo.GetCustomAttributes(typeof(UiSonAttribute), false))
            {
                if (att is UiSonCollectionAttribute col)
                {
                    collection = col;
                }
                else
                {
                    attribute = att as UiSonAttribute;
                }
            }

            if (attribute == null)
            {
                throw new Exception($"Member {memberInfo} lacks any UiSon Attribute");
            }

            // handle collection elements
            if (collection != null)
            {
                // val types
                if (collection.EntryType.IsAssignableFrom(typeof(bool)))
                {
                    return new ValCollectionElement<bool>(collection.Name, collection.Priority,
                                                          collection.DisplayMode, collection.Modifiable, collection.CollectionType, collection.Alignment,
                                                          attribute, memberInfo, this);
                }
                else if (collection.EntryType.IsAssignableFrom(typeof(char)))
                {
                    return new ValCollectionElement<char>(collection.Name, collection.Priority,
                                                          collection.DisplayMode, collection.Modifiable, collection.CollectionType, collection.Alignment,
                                                          attribute, memberInfo, this);
                }
                else if (collection.EntryType.IsAssignableFrom(typeof(int)))
                {
                    return new ValCollectionElement<int>(collection.Name, collection.Priority,
                                                          collection.DisplayMode, collection.Modifiable, collection.CollectionType, collection.Alignment,
                                                          attribute, memberInfo, this);
                }
                else if (collection.EntryType.IsAssignableFrom(typeof(float)))
                {
                    return new ValCollectionElement<float>(collection.Name, collection.Priority,
                                                           collection.DisplayMode, collection.Modifiable, collection.CollectionType, collection.Alignment,
                                                           attribute, memberInfo, this);
                }
                // and also string because it doesn't have a constructor
                else if (collection.EntryType.IsAssignableFrom(typeof(string)))
                {
                    return new ValCollectionElement<string>(collection.Name, collection.Priority,
                                                            collection.DisplayMode, collection.Modifiable, collection.CollectionType, collection.Alignment,
                                                            attribute, memberInfo, this);
                }
                // ref types
                else
                {
                    return new RefCollectionElement(collection.Name, collection.Priority,
                                                    collection.DisplayMode, collection.Modifiable, collection.CollectionType, collection.Alignment,
                                                    collection.EntryType, memberInfo, this);
                }
            }
            // handle object elements
            else if (attribute is UiSonMemberElementAttribute ele)
            {
                return MakeRefElement(memberInfo.GetUnderlyingType(), memberInfo);
            }

            // handle value elemnts
            return MakeValElement(attribute, memberInfo);
        }

        /// <summary>
        /// creates a elemnts representing a value type
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public IElement MakeValElement(UiSonAttribute attribute, MemberInfo info = null)
        {
            if (attribute is UiSonTextEditAttribute textEdit)
            {
                return new TextEditElement(textEdit.Name, textEdit.Priority, info, textEdit.DefaultValue, textEdit.RegexValidation);
            }
            else if (attribute is UiSonElementSelectorAttribute elesel)
            {
                return new ElementSelectorElement(elesel.Name,
                                                  elesel.Priority,
                                                  info,
                                                  elesel.Options,
                                                  _elementManagers.FirstOrDefault(x => x.ElementName == elesel.ElementName).Elements);
            }
            else if (attribute is UiSonSelectorAttribute sel)
            {
                return new SelectorElement(sel.Name, sel.Priority, info, sel.Options, sel.DefaultValue);
            }
            else if (attribute is UiSonCheckboxAttribute chk)
            {
                return new CheckboxElement(chk.Name, chk.Priority, chk.DefaultValue, info);
            }

            return null;
        }
    }
}
