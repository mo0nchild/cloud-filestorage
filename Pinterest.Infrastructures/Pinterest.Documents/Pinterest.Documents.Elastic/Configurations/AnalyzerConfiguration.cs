using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;

namespace Pinterest.Documents.Elastic.Configurations;

internal static class AnalyzerConfiguration
{
    public static IndexSettings BasicSettings { get; } = new IndexSettings()
    {
        Analysis = new IndexSettingsAnalysis()
        {
            Analyzers = new Analyzers()
            {
                ["russian_english_analyzer"] = new CustomAnalyzer()
                {
                    Tokenizer = "standard", 
                    Filter = new[] { "lowercase", "russian_stop", "russian_stemmer", "english_stop", "english_stemmer" }
                },
                ["autocomplete"] = new CustomAnalyzer()
                {
                    Tokenizer = "autocomplete_tokenizer", Filter = new[] { "lowercase" }
                }
            }, 
            Tokenizers = new Tokenizers()
            {
                ["autocomplete_tokenizer"] = new EdgeNGramTokenizer
                {
                    MinGram = 2,
                    MaxGram = 10,
                    TokenChars = new[] { TokenChar.Letter, TokenChar.Digit }
                }
            },
            TokenFilters = new TokenFilters
            {
                { "russian_stop", new StopTokenFilter { Stopwords = new[] {"_russian_" } } },
                { "english_stop", new StopTokenFilter { Stopwords = new [] { "_english_"}  } },
                { "russian_stemmer", new StemmerTokenFilter { Language = "russian" } },
                { "english_stemmer", new StemmerTokenFilter { Language = "english" } }
            }
        }
    };
}