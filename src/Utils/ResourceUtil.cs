using System.IO;
using System.Reflection;
using System.Text;

namespace Kiss.Utils
{
    /// <summary>
    /// resource util
    /// </summary>
    public static class ResourceUtil
    {
        /// <summary>
        /// load text from specified assembly
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LoadTextFromAssembly(Assembly asm, string name)
        {
            AssertUtils.ArgumentNotNull(asm, "asm");
            AssertUtils.ArgumentHasText(name, "name");

            using (Stream stream = asm.GetManifestResourceStream(name))
            {
                if (stream == null)
                    return null;

                using (StreamReader rdr = new StreamReader(stream, Encoding.UTF8))
                {
                    return rdr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// load text from the calling assembly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LoadTextFromAssembly(string name)
        {
            return LoadTextFromAssembly(Assembly.GetCallingAssembly(), name);
        }
    }
}
