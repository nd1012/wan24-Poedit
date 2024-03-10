using Karambolo.PO;
using Microsoft.Extensions.Localization;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using wan24.Core;

namespace wan24.Poedit
{
    /// <summary>
    /// Poedit translation terms
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    /// <param name="po">PO catalog</param>
    public class PoeditTranslationTerms(in POCatalog po) : ITranslationTerms
    {
        /// <inheritdoc/>
        public string this[in string key, params string[] values] => GetTerm(key,values);

        /// <inheritdoc/>
        public string this[in string key, in int count, params string[] values] => GetTerm(key, values);

        /// <inheritdoc/>
        string IReadOnlyDictionary<string, string>.this[string key] => this[key];

        /// <inheritdoc/>
        LocalizedString IStringLocalizer.this[string name, params object[] arguments] => StringLocalizer(name,arguments);

        /// <inheritdoc/>
        LocalizedString IStringLocalizer.this[string name] => StringLocalizer(name, []);

        /// <summary>
        /// PO catalog
        /// </summary>
        public POCatalog Catalog { get; } = po;

        /// <inheritdoc/>
        public bool PluralSupport => Catalog.PluralFormCount > 0;

        /// <inheritdoc/>
        public IEnumerable<string> Keys => Catalog.Keys.Select(k => k.Id);

        /// <inheritdoc/>
        public IEnumerable<string> Values => Catalog.Values.Select(v => v[0]);

        /// <inheritdoc/>
        public int Count => Catalog.Count;

        /// <inheritdoc/>
        public bool ContainsKey(string key) => Catalog.Contains(new POKey(key));

        /// <inheritdoc/>
        public string GetTerm(in string key, params string[] values)
        {
            string res = TryGetValue(key, out string? value) ? value : key;
            if (values.Length > 0)
            {
                int len = values.Length;
                Dictionary<string, string> valuesDict = new(len);
                for (int i = 0; i < len; valuesDict[i.ToString()] = values[i], i++) ;
                res = res.Parse(valuesDict, Translation.ParserOptions);
            }
            return res;
        }

        /// <inheritdoc/>
        public string GetTerm(in string key, in int count, params string[] values)
        {
            if (!PluralSupport) throw new NotSupportedException();
            string res = Catalog.GetTranslation(new(key), count);
            if (values.Length > 0)
            {
                int len = values.Length;
                Dictionary<string, string> valuesDict = new(len);
                for (int i = 0; i < len; valuesDict[i.ToString()] = values[i], i++) ;
                res = res.Parse(valuesDict, Translation.ParserOptions);
            }
            return res;
        }

        /// <inheritdoc/>
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            if (!Catalog.TryGetValue(new(key), out IPOEntry? entry))
            {
                value = null;
                return false;
            }
            value = entry[0];
            return true;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Keys.Select(k => new KeyValuePair<string, string>(k, GetTerm(k))).GetEnumerator();

        /// <inheritdoc/>
        IEnumerable<LocalizedString> IStringLocalizer.GetAllStrings(bool includeParentCultures) => Keys.Select(k => StringLocalizer(k, []));

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// String localizer used for the <see cref="IStringLocalizer"/> implementation
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns>Localized string</returns>
        protected virtual LocalizedString StringLocalizer(in string name, in object[] arguments)
            => new(name, GetTerm(name, [.. arguments.Select(a => a.ToString() ?? string.Empty)]));

        /// <summary>
        /// Create from a PO stream
        /// </summary>
        /// <param name="stream">PO stream (won't be disposed)</param>
        /// <returns>Translation terms</returns>
        public static PoeditTranslationTerms FromStream(in Stream stream) => new(new POParser().Parse(stream).Catalog);

        /// <summary>
        /// Create from a PO stream
        /// </summary>
        /// <param name="stream">PO stream (won't be disposed)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Translation terms</returns>
        public static async Task<PoeditTranslationTerms> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            using MemoryPoolStream ms = new();
            await stream.CopyToAsync(ms, cancellationToken).DynamicContext();
            ms.Position = 0;
            return new(new POParser().Parse(ms).Catalog);
        }

        /// <summary>
        /// Create from a PO file
        /// </summary>
        /// <param name="fileName">PO filename</param>
        /// <returns>Translation terms</returns>
        public static PoeditTranslationTerms FromFile(in string fileName)
        {
            using FileStream fs = FsHelper.CreateFileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return FromStream(fs);
        }

        /// <summary>
        /// Create from a PO file
        /// </summary>
        /// <param name="fileName">PO filename</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Translation terms</returns>
        public static async Task<PoeditTranslationTerms> FromFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            FileStream fs = FsHelper.CreateFileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            await using(fs.DynamicContext()) return await FromStreamAsync(fs, cancellationToken).DynamicContext();
        }

        /// <summary>
        /// Create from a PO string
        /// </summary>
        /// <param name="str">PO string</param>
        /// <returns>Translation terms</returns>
        public static PoeditTranslationTerms FromString(in string str) => new(new POParser().Parse(str).Catalog);

        /// <summary>
        /// Create from bytes (UTF-8 encoded PO string)
        /// </summary>
        /// <param name="data">PO data</param>
        /// <returns>Translation terms</returns>
        public static PoeditTranslationTerms FromBytes(in byte[] data)
        {
            using MemoryStream ms = new(data);
            return FromStream(ms);
        }
    }
}
