// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using UiSon.Attribute;
using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class EncapsulatingModule : GroupModule
    {
        public override object Value
        {
            get
            {
                var instance = Activator.CreateInstance(_type);

                Write(instance);

                return instance;
            }
            set
            {
                if (value?.GetType().IsAssignableTo(_type) ?? false)
                {
                    Read(value);
                }
            }
        }

        private readonly Type _type;

        public EncapsulatingModule(Type type,
                                   IEnumerable<IEditorModule> members,
                                   string name,
                                   int displayPriority,
                                   DisplayMode displayMode)
            : base(members, name, displayPriority, displayMode)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
