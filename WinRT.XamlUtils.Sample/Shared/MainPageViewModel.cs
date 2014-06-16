using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinRT.XamlUtils.Sample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainPageViewModel()
        {
            Fruits = new List<Fruit>
            {
                new Fruit { Icon = "cherries", Name = "Cherries" },
                new Fruit { Icon = "orange", Name = "Orange" },
                new Fruit { Icon = "grapes", Name = "Grapes" },
                new Fruit { Icon = "strawberry", Name = "Strawberry" },
                new Fruit { Icon = "banana", Name = "Banana" }
            };
        }

        public IEnumerable<Fruit> Fruits { get; set; }
    }

    public class Fruit
    {
        public string Icon { get; set; }
        public string Name { get; set; }
    }
}
