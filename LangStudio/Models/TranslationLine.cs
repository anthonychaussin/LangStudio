using System.ComponentModel;

namespace LangStudio.Models
{
    public class TranslationLine(String language, String value) : INotifyPropertyChanged
    {
        #region Fields

        private String lang = language;
        private String text = value;

        #endregion Fields

        #region Properties

        public string Lang
        {
            get
            {
                return lang;
            }
            set
            {
                lang = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Lang)));
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        #endregion Properties

        #region Public Methods

        public override String ToString()
        {
            return Lang + " : " + Text;
        }

        #endregion Public Methods

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Events
    }
}