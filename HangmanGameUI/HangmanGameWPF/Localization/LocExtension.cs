using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Markup;

namespace HangmanGameWPF.Localization
{
    public sealed class LocExtension : MarkupExtension
    {
        public LocExtension()
        {
        }

        public LocExtension(string key)
        {
            Key = key;
        }

        public string Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding binding = new Binding("[" + Key + "]")
            {
                Source = LocalizedTextProvider.Instance,
                Mode = BindingMode.OneWay
            };

            return binding.ProvideValue(serviceProvider);
        }
    }

    public sealed class LocalizedTextProvider : INotifyPropertyChanged
    {
        public static readonly LocalizedTextProvider Instance = new LocalizedTextProvider();

        private LocalizedTextProvider()
        {
            ClientLanguageContext.LanguageChanged += (sender, args) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string this[string key]
        {
            get { return ClientLocalizer.Get(key); }
        }
    }
}
