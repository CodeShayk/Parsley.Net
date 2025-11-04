using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace parsley
{
    public interface IParser
    {
        /// <summary>
        /// Parses a file at the specified filepath into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        T[] Parse<T>(string filepath) where T : IFileLine, new();

        /// <summary>
        /// Parses an array of delimiter seperated strings into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lines"></param>
        /// <returns></returns>
        T[] Parse<T>(string[] lines) where T : IFileLine, new();

        /// <summary>
        /// Parses an array of bytes of delimiter separated records into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        T[] Parse<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Parses a stream of delimiter separated records into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        T[] Parse<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously parses a file at the specified filepath into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        Task<T[]> ParseAsync<T>(string filepath) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously parses an array of delimiter separated strings into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lines"></param>
        /// <returns></returns>
        Task<T[]> ParseAsync<T>(string[] lines) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously parses an array of bytes of delimiter separated records into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        Task<T[]> ParseAsync<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously parses a stream of delimiter separated strings into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<T[]> ParseAsync<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse a file at the specified filepath into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(string filepath) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse a file at the specified filepath into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(string filepath, ParseOptions options) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse an array of delimiter separated strings into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lines"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(string[] lines) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse an array of delimiter separated strings into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lines"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(string[] lines, ParseOptions options) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse an array of bytes of delimiter separated records into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse an array of bytes of delimiter separated records into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse a stream of delimiter separated records into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Attempts to parse a stream of delimiter separated records into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Result<T[]> TryParse<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse a file at the specified filepath into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(string filepath) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse a file at the specified filepath into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(string filepath, ParseOptions options) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse an array of delimiter separated strings into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lines"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(string[] lines) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse an array of delimiter separated strings into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lines"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(string[] lines, ParseOptions options) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse an array of bytes of delimiter separated records into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(byte[] bytes, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse an array of bytes of delimiter separated records into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(byte[] bytes, Encoding encoding, ParseOptions options) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse a stream of delimiter separated records into an array of objects of type T with explicit result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(Stream stream, Encoding encoding = null) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously attempts to parse a stream of delimiter separated records into an array of objects of type T with explicit result and options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<Result<T[]>> TryParseAsync<T>(Stream stream, Encoding encoding, ParseOptions options) where T : IFileLine, new();
    }
}