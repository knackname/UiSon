// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Linq;
using UiSon.Attribute;
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
        private readonly ModuleTemplateSelector _templateSelector;

        public EditorModuleFactory(ModuleTemplateSelector templateSelector)
        {
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
        }

        /// <summary>
        /// Makes an editor madule from an IReadWriteView
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public IEditorModule MakeEditorModule(IReadWriteView view)
        {
            if (view is IUiValueView uiValueView)
            {
                return MakeUiValueEditorModule(uiValueView);
            }
            else if (view is IGroupView groupView)
            {
                var members = new List<IEditorModule>();

                foreach (var member in groupView.Members)
                {
                    members.Add(MakeEditorModule(member));
                }

                var group = new GroupModule(groupView.Name,
                                            groupView.DisplayPriority,
                                            groupView.DisplayMode,
                                            members.ToArray());

                return group.Name == null
                    ? group
                    : new BorderedModule(group);
            }

            return MakeErrorTextBlock($"Unhandled view type {view.GetType()}, {view.Name}", view.DisplayPriority);
        }

        /// <summary>
        /// Makes a Value editor module forom a IUiValueView
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IValueEditorModule MakeUiValueEditorModule(IUiValueView view)
        {
            if (view is NullBufferValueView bufferValueView)
            {
                return new NullableModule(bufferValueView, this, _templateSelector);
            }

            switch (view.UiType)
            {
                case UiType.Checkbox:
                    return new CheckboxModule(view, _templateSelector);
                case UiType.Selector:
                case UiType.ElementSelector:
                    if (view is ISelectorValueView selectorValueView)
                    {
                        return new SelectorModule(selectorValueView, _templateSelector);
                    }
                    else
                    {
                        throw new Exception("Encapsulating ui type on non-selector view.");
                    }
                case UiType.Label:
                    return new TextBlockModule(view, _templateSelector);
                case UiType.Slider:
                    if (view is RangeUiValueView rangeView)
                    {
                        return new ValueGroupModule(view.Name,
                                                    view.DisplayPriority,
                                                    DisplayMode.Vertial,
                                                    view,
                                                    new IEditorModule[] { new TextEditModule(view, _templateSelector),
                                                                          new SliderModule(rangeView, _templateSelector) });
                    }
                    else
                    {
                        throw new Exception("slider ui type on non-range view.");
                    }
                case UiType.TextEdit:
                    return new TextEditModule(view, _templateSelector);
                case UiType.Encapsulating:
                    if (view is EncapsulatingView encapsulatingView)
                    {
                        var members = new List<IEditorModule>();

                        foreach (var memberView in encapsulatingView.Members)
                        {
                            members.Add(MakeEditorModule(memberView));
                        }

                        if (encapsulatingView is ICollectionValueView collectionView)
                        {
                            //if (collectionView is MultiChoiceView multiChoiceView)
                            //{
                                // make options

                                //members.Add(new MultiChoiceModule(multiChoiceView, this, _templateSelector));
                            //}
                            //else
                            //{
                                members.Add(new CollectionModule(collectionView, this, _templateSelector));
                            //}
                        }

                        var encapsulatingModule =  new EncapsulatingModule(encapsulatingView,
                                                                           members.OrderByDescending(x => x.DisplayPriority).ToArray());

                        return encapsulatingView.Name == null
                            ? encapsulatingModule
                            : new BorderedValueModule(encapsulatingModule);
                    }
                    else
                    {
                        throw new Exception("Encapsulating ui type on non-encapsulating view.");
                    }
            }

            return MakeErrorTextBlock($"Unhandled ui type: {view.UiType}", view.DisplayPriority);
        }

        private TextBlockModule MakeErrorTextBlock(string message, int priority)
        {
            return new TextBlockModule(new StaticView(message, true, priority), _templateSelector);
        }
    }
}
