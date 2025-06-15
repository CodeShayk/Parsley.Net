using System.IO;
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
        /// Parses a stream of delimiter separated records into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        T[] Parse<T>(Stream stream) where T : IFileLine, new();

        /// <summary>
        /// Parses an array of bytes of delimiter separated records into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        T[] Parse<T>(byte[] bytes) where T : IFileLine, new();

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
        /// Asynchronously parses a stream of delimiter separated strings into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<T[]> ParseAsync<T>(Stream stream) where T : IFileLine, new();

        /// <summary>
        /// Asynchronously parses an array of bytes of delimiter separated records into an array of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        Task<T[]> ParseAsync<T>(byte[] bytes) where T : IFileLine, new();
    }
}