// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Linq;
using UiSon.Attribute;
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
        private readonly ModuleTemplateSelector _templateSelector;
        private readonly ClipBoardManager _clipBoardManager;
        private readonly INotifier _notifier;

        public EditorModuleFactory(ModuleTemplateSelector templateSelector, ClipBoardManager ClipBoardManager, INotifier notifier)
        {
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
            _clipBoardManager = ClipBoardManager ?? throw new ArgumentNullException(nameof(ClipBoardManager));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
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
                return new NullableModule(bufferValueView, this, _clipBoardManager, _templateSelector, _notifier);
            }

            switch (view.UiType)
            {
                case UiType.Checkbox:
                    return new CheckboxModule(view, _templateSelector, _clipBoardManager, _notifier);
                case UiType.Selector:
                case UiType.ElementSelector:
                    if (view is ISelectorValueView selectorValueView)
                    {
                        return new SelectorModule(selectorValueView, _templateSelector, _clipBoardManager, _notifier);
                    }
                    else
                    {
                        throw new Exception("Encapsulating ui type on non-selector view.");
                    }
                case UiType.Label:
                    return new TextBlockModule(view, _templateSelector, _clipBoardManager, _notifier);
                case UiType.Slider:
                    if (view is RangeUiValueView rangeView)
                    {
                        return new ValueGroupModule(view.Name,
                                                    view.DisplayPriority,
                                                    DisplayMode.Vertial,
                                                    _clipBoardManager,
                                                    _notifier,
                                                    view,
                                                    new IEditorModule[] { new TextEditModule(view, _templateSelector, _clipBoardManager, _notifier),
                                                                          new SliderModule(rangeView, _templateSelector, _clipBoardManager, _notifier) });
                    }
                    else
                    {
                        throw new Exception("slider ui type on non-range view.");
                    }
                case UiType.TextEdit:
                    return new TextEditModule(view, _templateSelector, _clipBoardManager, _notifier);
                case UiType.Encapsulating:
                    if (view is IEncapsulatingView encapsulatingView)
                    {
                        var members = new List<IEditorModule>();

                        foreach (var memberView in encapsulatingView.Members)
                        {
                            members.Add(MakeEditorModule(memberView));
                        }

                        if (encapsulatingView is ICollectionValueView collectionView)
                        {
                            members.Add(new CollectionModule(collectionView, this, _templateSelector, _clipBoardManager, _notifier));
                        }

                        var encapsulatingModule =  new EncapsulatingModule(encapsulatingView,
                                                                           _clipBoardManager,
                                                                           _notifier,
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
            return new TextBlockModule(new StaticView(message, priority, Element.ModuleState.Error, "View model error"), _templateSelector, _clipBoardManager, _notifier);
        }
    }
}
