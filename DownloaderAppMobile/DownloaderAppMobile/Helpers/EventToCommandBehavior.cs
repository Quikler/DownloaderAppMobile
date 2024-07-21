using System;
using System.Windows.Input;
using System.Reflection;
using Xamarin.Forms;

namespace DownloaderAppMobile.Helpers
{
    public class EventToCommandBehavior : Behavior<VisualElement>
    {
        #region Bindable Properties
        public static readonly BindableProperty EventNameProperty =
            BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior), null, propertyChanged: OnEventNameChanged);

        public string EventName
        {
            get => (string)GetValue(EventNameProperty);
            set => SetValue(EventNameProperty, value);
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior), null);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior), null);

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        #endregion

        public VisualElement AssociatedObject { get; private set; }

        private Delegate _eventHandler;
        
        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            RegisterEvent(EventName);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            DeregisterEvent(EventName);
            AssociatedObject = null;
            bindable.BindingContextChanged -= OnBindingContextChanged;
            base.OnDetachingFrom(bindable);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }

        private void OnBindingContextChanged(object sender, EventArgs e) => OnBindingContextChanged();
        
        private void RegisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var eventInfo = AssociatedObject.GetType().GetEvent(name) ??
                throw new ArgumentException($"EventToCommandBehavior: Can't register the '{EventName}' event.");
            var methodInfo = typeof(EventToCommandBehavior).GetMethod(nameof(OnEvent),
                BindingFlags.NonPublic | BindingFlags.Instance);
            _eventHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
            eventInfo.AddEventHandler(AssociatedObject, _eventHandler);
        }

        private void DeregisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            if (_eventHandler == null)
                return;

            var eventInfo = AssociatedObject.GetType().GetEvent(name) ?? 
                throw new ArgumentException($"EventToCommandBehavior: Can't de-register the '{EventName}' event.");
            eventInfo.RemoveEventHandler(AssociatedObject, _eventHandler);
            _eventHandler = null;
        }

        private void OnEvent(object sender, object eventArgs)
        {
            ICommand command = Command;
            if (command == null)
                return;

            var resolvedParameter = CommandParameter ?? eventArgs;
            if (command.CanExecute(resolvedParameter))
                command.Execute(resolvedParameter);
        }

        private static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (EventToCommandBehavior)bindable;
            if (behavior.AssociatedObject == null)
                return;

            behavior.DeregisterEvent((string)oldValue);
            behavior.RegisterEvent((string)newValue);
        }
    }
}
