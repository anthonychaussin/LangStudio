using LangStudio.Models;

namespace LangStudio.ViewModel.Tests
{
    [TestClass()]
    public class MainViewModelTests
    {
        [TestMethod()]
        public void ViewModel_Should_Filter_Incomplete_Entries()
        {
            var vm = new MainViewModel();
            var langs = new List<string> { "en", "fr" };
            var entry = new TranslationEntry(langs, "test.key");
            entry.UpdateTranslation("en", "Hello");

            vm.Entries.Add(entry);
            vm.ShowIncompleteOnly = true;

            var tree = vm.TreeNodes;

            Assert.IsTrue(tree.Count == 1);
        }

        public void ViewModel_Should_LoadTranslations_FromJsonFiles()
        {
            string testFolder = Path.Combine(Path.GetTempPath(), "LangStudioTest");
            var dir = Directory.CreateDirectory(testFolder);

            File.WriteAllText(Path.Combine(testFolder, "en.json"), @"{ ""app"": { ""title"": ""Home"" } }");
            File.WriteAllText(Path.Combine(testFolder, "fr.json"), @"{ ""app"": { ""title"": ""Accueil"" } }");
            File.WriteAllText(Path.Combine(testFolder, "es.json"), @"{ ""app"": { }}");

            var vm = new MainViewModel();

            vm.LoadTranslationFiles(testFolder);

            Assert.IsTrue(vm.Entries.Count == 1);
            var entry = vm.Entries.First();
            Assert.Equals("app.title", entry.Key);
            Assert.IsFalse(entry.IsComplete);

            dir.Delete();
        }
    }
}