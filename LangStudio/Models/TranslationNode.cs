using System.ComponentModel;

namespace LangStudio.Models
{
    public class TranslationNode : INotifyPropertyChanged
    {
        private TranslationEntry? entry;

        #region Properties

        public List<TranslationNode> Children { get; set; } = [];

        public TranslationEntry? Entry
        {
            get
            {
                return entry;
            }
            set
            {
                entry = value;
                if (entry != null)
                {
                    entry.PropertyChanged += this.Entry_PropertyChanged;
                }
            }
        }

        private void Entry_PropertyChanged(Object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsComplete)));
        }

        public bool IsComplete
        {
            get
            {
                return Children.Count == 0 ? Entry!.IsComplete : Children.All(c => c.IsComplete);
            }
        }

        public string Name { get; set; } = "";

        #endregion Properties

        #region Public Methods

        public override string ToString() => Name;

        #endregion Public Methods

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}