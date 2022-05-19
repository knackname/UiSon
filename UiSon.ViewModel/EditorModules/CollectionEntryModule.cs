// UiSon, by Cameron Gale 2021

using System;
using System.Windows.Input;
using UiSon.Command;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates another <see cref="IEditorModule"/> to act as a collection entry
    /// </summary>
    public class CollectionEntryModule : BaseDecoratingEditorModule, ICollectionEntryModule
    {
        public bool CanModifyCollection => _parent.CanModifyCollection;

        private readonly CollectionModule _parent;

        public CollectionEntryModule(CollectionModule parent, IEditorModule decorated)
            :base(decorated)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        #region Commands

        public ICommand RemoveElement => new UiSonActionCommand((s) => _parent.RemoveEntry(this));

        #endregion
    }
}
