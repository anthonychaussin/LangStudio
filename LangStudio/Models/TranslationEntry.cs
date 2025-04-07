using System.ComponentModel;

namespace LangStudio.Models
{
    public class TranslationEntry : INotifyPropertyChanged
    {
        public TranslationEntry(List<String> allLangs, String key)
        {
            this.key = key;
            Translations = allLangs.ConvertAll(lang => new TranslationLine(lang, String.Empty));
            Translations.ForEach(t =>
            {
                t.PropertyChanged += OnTranslationChanged;
            });
        }

        private String? key;

        public string? Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsComplete)));
            }
        }

        public List<TranslationLine> Translations { get; set; }

        public bool IsComplete
        {
            get
            {
                return Translations.All(t => t.Text != string.Empty);
            }
        }

        public void UpdateTranslation(string language, string value)
        {
            var item = Translations.FirstOrDefault(t => t.Lang == language);
            if (item == null)
            {
                item = new TranslationLine(language, value);
                item.PropertyChanged += OnTranslationChanged;
                Translations.Add(item);
            }
            else
            {
                item.Text = value;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsComplete)));
        }

        private void OnTranslationChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TranslationLine.Text) || e.PropertyName == nameof(TranslationLine.Lang))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsComplete)));
            }
        }

        public override String ToString()
        {
            return Key + "[" + String.Join(',', Translations.ConvertAll(t => t.Lang).ToList()) + "]";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}