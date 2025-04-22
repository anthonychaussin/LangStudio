namespace LangStudio.Models.Tests
{
    [TestClass()]
    public class TranslationEntryTests
    {
        [TestMethod()]
        public void Entry_IsComplete_ShouldBeFalse_WhenTranslationIsMissing()
        {
            var langs = new List<string> { "en", "fr", "es" };
            var entry = new TranslationEntry(langs, "app.title");

            entry.UpdateTranslation("en", "Home");
            entry.UpdateTranslation("fr", "Accueil");

            Assert.IsFalse(entry.IsComplete);
        }

        [TestMethod()]
        public void Entry_IsComplete_ShouldBeTrue_WhenAllTranslationsAreFilled()
        {
            var langs = new List<string> { "en", "fr" };
            var entry = new TranslationEntry(langs, "app.title");

            entry.UpdateTranslation("en", "Home");
            entry.UpdateTranslation("fr", "Accueil");

            Assert.IsTrue(entry.IsComplete);
        }
    }
}