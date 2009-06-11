using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using eWoW.Common;

namespace eWoW.Network
{
    public class HookCollection : List<IPacketHook>, IDisposable
    {
        public HookCollection(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("Could not find the directory " + directory);
            }
            int loadedHooks = 0;
            foreach (string dir in Directory.GetFiles(directory, "*.dll"))
            {
                loadedHooks += LoadDll(dir);
            }
            Logging.Write("Loaded " + loadedHooks + " packet hooks.");
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Loads a DLL containing instances of <see cref="IPacketHook"/>.
        /// </summary>
        /// <param name="dllPath">The full path of the DLL to load.</param>
        /// <returns>The number of hooks loaded.</returns>
        public int LoadDll(string dllPath)
        {
            int ret = 0;
            if (string.IsNullOrEmpty(dllPath))
            {
                throw new ArgumentNullException("dllPath");
            }
            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException("Could not find the specified DLL.", dllPath);
            }

            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(dllPath);
            }
            catch (Exception e)
            {
                Logging.WriteException(e);
                return ret;
            }

            IEnumerable<Type> classes = from t in asm.GetTypes()
                                        where
                                            t.GetInterfaces().Contains(typeof(IPacketHook)) &&
                                            t.GetCustomAttributes(typeof(SkipPacketHookAttribute), false).Length == 0
                                        select t;
            foreach (Type type in classes)
            {
                var loaded = (IPacketHook) Activator.CreateInstance(type);
                if (!Contains(loaded))
                {
                    Add(loaded);
                    Logging.WriteDebug(ConsoleColor.Yellow, "Loaded packet hook {0}.", loaded.GetType().FullName);
                    ret++;
                }
            }
            return ret;
        }

        ~HookCollection()
        {
            Dispose();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            string ret = "";
            foreach (IPacketHook hook in this)
            {
                ret += string.Format("{0} hooking opcodes:{1}", hook.GetType().FullName, Environment.NewLine);
                foreach (uint u in hook.AcceptedOpCodes)
                {
                    ret += string.Format("\t - 0x{0}{1}", u.ToString("X"), Environment.NewLine);
                }
            }
            return ret;
        }
    }
}