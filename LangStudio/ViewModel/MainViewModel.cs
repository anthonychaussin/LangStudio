using CommunityToolkit.Mvvm.Input;
using LangStudio.Helpers;
using LangStudio.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace LangStudio.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private string? jsonFolderPath;
        private string searchQuery = string.Empty;
        private TranslationEntry? selectedEntry;
        private bool showIncompleteOnly;
        private TranslationNode? selectedNode;

        #endregion Fields

        #region Private Methods

        private static List<TranslationNode> BuildTree(List<TranslationEntry> entries)
        {
            var rootNodes = new List<TranslationNode>();
            var currentList = rootNodes;
            TranslationNode? currentNode;
            TranslationNode existing;

            entries.ForEach(entry =>
            {
                currentNode = null;
                currentList = rootNodes;

                entry.Key!.Split('.')
                    .ToList()
                    .ForEach(part =>
                {
                    existing = currentList.FirstOrDefault(n => n.Name == part);
                    if (existing == null)
                    {
                        existing = new TranslationNode { Name = part };
                        currentList.Add(existing);
                    }

                    currentNode = existing;
                    currentList = existing.Children;
                });

                currentNode!.Entry = entry;
            });

            return [.. rootNodes.OrderBy(n => n.Name)];
        }

        private void ApplyFilters()
        {
            TreeNodes.Clear();

            BuildTree([.. Entries
                .Where(entry =>
                {
                    if (ShowIncompleteOnly && entry.IsComplete)
                        return false;

                    if (!string.IsNullOrWhiteSpace(SearchQuery))
                        return entry.Key!.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        entry.Translations.Any(t => t.Text.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

                    return true;
                })])
                .ForEach(TreeNodes.Add);
        }

        private void OpenFolder()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog()
            {
                Title = "Choose the translation file",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FolderName;
                JsonFolderPath = path;
                Properties.Settings.Default.LastUsedFolder = path;
                Properties.Settings.Default.Save();
                LoadTranslationFiles(path);
            }
        }

        private void SaveTranslations()
        {
            if (string.IsNullOrEmpty(JsonFolderPath)) return;

            var byLang = new ConcurrentDictionary<string, JObject>();

            Entries.ToList().ForEach(entry =>
            {
                entry.Translations.ForEach(kvp =>
                {
                    if (!byLang.ContainsKey(kvp.Lang))
                        byLang[kvp.Lang] = [];

                    JsonHelper.SetJsonValue(byLang[kvp.Lang], entry.Key!.Split('.'), kvp.Text);
                });
            });

            foreach (var (lang, content) in byLang)
            {
                File.WriteAllText(Path.Combine(JsonFolderPath, $"{lang}.json"), JsonConvert.SerializeObject(content, Formatting.Indented));
            }

            MessageBox.Show("Saved!", "LangStudio", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadLastUsedFolderIfAvailable()
        {
            string lastPath = Properties.Settings.Default.LastUsedFolder;

            if (!string.IsNullOrEmpty(lastPath) && Directory.Exists(lastPath))
            {
                JsonFolderPath = lastPath;
                LoadTranslationFiles(lastPath);
            }
        }

        private void SelectNode(TranslationNode? node)
        {
            SelectedNode = node;
        }

        #endregion Private Methods

        #region Protected Methods

        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion Protected Methods

        #region Properties

        public List<string> AllLanguages { get; private set; } = [];
        public ObservableCollection<TranslationEntry> Entries { get; } = new([]);

        public string? JsonFolderPath
        {
            get => jsonFolderPath;
            set
            {
                jsonFolderPath = value;
                OnPropertyChanged(nameof(JsonFolderPath));
            }
        }

        public ICommand OpenFolderCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SelectNodeCommand { get; }

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                ApplyFilters();
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        public TranslationEntry? SelectedEntry
        {
            get => selectedEntry;
            set
            {
                selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
            }
        }

        public bool ShowIncompleteOnly
        {
            get => showIncompleteOnly;
            set
            {
                showIncompleteOnly = value;
                ApplyFilters();
                OnPropertyChanged(nameof(ShowIncompleteOnly));
            }
        }

        public TranslationNode? SelectedNode
        {
            get => selectedNode;
            set
            {
                selectedNode = value;
                OnPropertyChanged(nameof(SelectedNode));
            }
        }

        public ObservableCollection<TranslationNode> TreeNodes { get; } = [];

        #endregion Properties

        #region Public Constructors

        public MainViewModel()
        {
            OpenFolderCommand = new RelayCommand(OpenFolder);
            SaveCommand = new RelayCommand(SaveTranslations);
            SelectNodeCommand = new RelayCommand<TranslationNode>(SelectNode);

            LoadLastUsedFolderIfAvailable();
        }

        #endregion Public Constructors

        #region Public Methods

        public void LoadTranslationFiles(string folderPath)
        {
            Entries.Clear();
            TreeNodes.Clear();

            var files = Directory.GetFiles(folderPath, "*.json").ToList();
            if (files.Count == 0)
                return;

            AllLanguages = [.. files.Select(f => Path.GetFileNameWithoutExtension(f))];

            string lang;

            files.ForEach(file =>
            {
                lang = Path.GetFileNameWithoutExtension(file);

                foreach (var (key, value) in JsonHelper.FlattenJson(JObject.Parse(File.ReadAllText(file))))
                {
                    var entry = Entries.FirstOrDefault(e => e.Key == key);
                    if (entry == null)
                    {
                        entry = new TranslationEntry(AllLanguages, key);
                        Entries.Add(entry);
                    }
                    entry.UpdateTranslation(lang, value);
                }
            });
            ApplyFilters();
        }

        #endregion Public Methods

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Events
    }
}