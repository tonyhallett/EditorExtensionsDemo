using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace EditorExtensionsDemo
{
    internal class AttachToProperty : Freezable
    {
        public object Value
        {
            get { 
                var value = (object)GetValue(ValueProperty);
                return value;
            }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(AttachToProperty), new PropertyMetadata(null));

        public DependencyProperty AttachedProperty { get; set; }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
    internal class AttachToPropertyCollection : FreezableCollection<AttachToProperty> {
        protected override Freezable CreateInstanceCore()
        {
            return base.CreateInstanceCore();
        }
    }

    [ContentProperty(nameof(AttachToProperties))]
    internal class AttachToPropertyAction : TriggerAction<UIElement>
    {
        public AttachToPropertyCollection AttachToProperties { get; set; } = new AttachToPropertyCollection();

        public DependencyProperty ControlProperty { get; set; }

        protected override void Invoke(object parameter)
        {
            var dependencyObject = AssociatedObject.GetValue(ControlProperty) as DependencyObject;
            foreach(var attachToProperty in AttachToProperties)
            {
                BindingOperations.SetBinding(dependencyObject, attachToProperty.AttachedProperty, new Binding { Source = attachToProperty, Path = new PropertyPath(nameof(AttachToProperty.Value)) });
            }
        }

    }

}
