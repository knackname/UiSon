// UiSon, by Cameron Gale 2021

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Command;
using UiSon.Element;
using UiSon.View;
using UiSon.ViewModel;
using UiSon.ViewModel.Interface;

namespace UiSon
{
    /// <summary>
    /// Contains all of one type of UiSonElement
    /// </summary>
    public class ElementManagerVM : NPCBase, INamedOrderedViewModel
    {
        public string Name => _view.ElementName;

        public int DisplayPriority { get; private set; }

        public bool IsNameVisible => string.IsNullOrEmpty(Name);


        private readonly NotifyingCollection<ElementVM> _elementsVMs = new NotifyingCollection<ElementVM>();

        public Type ManagedType => _view.ElementType;

        private readonly UiSonElementAttribute _elementAtt;
        private readonly TabControl _tabController;
        private readonly EditorModuleFactory _factory;
        private readonly ElementManager _view;

        public ElementManagerVM(ElementManager view,
                                UiSonElementAttribute elementAtt,
                                TabControl tabController,
                                EditorModuleFactory factory)
        {
            _view = view;
            _tabController = tabController ?? throw new ArgumentNullException(nameof(tabController));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _elementAtt = elementAtt ?? throw new ArgumentNullException(nameof(elementAtt));

            // when a manager is deleted, remove it's tab as well
            _elementsVMs.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var tab in _tabController.Items)
                        {
                            if (tab is ElementEditorTab elementEditor
                                && (e.OldItems?.Contains(elementEditor.MainModule) ?? false))
                            {
                                _tabController.Items.Remove(elementEditor);
                                return;
                            }
                        }
                        break;
                }
            };
        }

        /// <summary>
        /// Removes a specific element
        /// </summary>
        /// <param name="elementVM"></param>
        public void RemoveElement(ElementVM elementVM) => _elementsVMs.Remove(elementVM);

        /// <summary>
        /// Saves all elements to a folder at the given path 
        /// </summary>
        /// <param name="path">Path to save to</param>
        public void Save(string path) => _view.Save(path);
    }
}
