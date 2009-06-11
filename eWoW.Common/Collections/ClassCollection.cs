using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace eWoW.Common.Collections
{
    /// <summary>
    /// This is a collection to hold class instances. It dynamically loads any classes found within DLLs
    /// in the base application directory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassCollection<T> : List<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassCollection&lt;T&gt;"/> class.
        /// </summary>
        public ClassCollection()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassCollection&lt;T&gt;"/> class.
        /// </summary>
        public ClassCollection(string dllFolderPath)
        {
            RecursiveLoad(dllFolderPath);
        }

        private void RecursiveLoad(string dir)
        {
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string s in dirs)
            {
                RecursiveLoad(s);
            }
            foreach (string s in Directory.GetFiles(dir, "*.dll"))
            {
                LoadClasses(s);
            }
        }

        /// <summary>
        /// Unloads the class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool UnloadClass(T item)
        {
            return Remove(item);
        }

        /// <summary>
        /// Loads the classes.
        /// </summary>
        /// <param name="dllPath">The DLL path.</param>
        /// <returns>The number of classes loaded.</returns>
        public int LoadClasses(string dllPath)
        {
            int ret = 0;
            // Reflection based loading here.
            if (File.Exists(dllPath))
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(dllPath);
                    Type[] types = asm.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsClass)
                        {
                            if (type.IsSubclassOf(typeof(T)))
                            {
                                var loaded = (T)Activator.CreateInstance(type);
                                if (!Contains(loaded))
                                {
                                    Logging.WriteDebug("{0} loaded!", loaded.ToString());
                                    Add(loaded);
                                    ret++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteDebug(ex.GetType().Name + " - ClassCollection<" + typeof(T).Name + ">: " + ex.Message);
                }
            }
            return ret;
        }

        public int LoadClassesInFolder(string folder)
        {
            int ret = 0;
            if (Directory.Exists(folder))
            {
                foreach (string s in Directory.GetFiles(folder,"*.dll"))
                {
                    ret += LoadClasses(s);
                }
            }
            return ret;
        }

        /// <summary>
        /// Loads the class.
        /// </summary>
        /// <param name="dllPath">The DLL path.</param>
        /// <param name="classFQN">The class's Fully Qualified Name (Type.FullName).</param>
        /// <returns></returns>
        public T LoadClass(string dllPath, string classFQN)
        {
            bool ok = false;
            T ret = default(T);
            //Logging.WriteDebug(dllPath);
            // Reflection based loading here.
            if (File.Exists(dllPath))
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(dllPath);
                    Type[] types = asm.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsClass)
                        {
                            if (type.IsSubclassOf(typeof(T)) && type.FullName == classFQN)
                            {
                                ret = (T)Activator.CreateInstance(type);
                                if (!Contains(ret))
                                {
                                    Logging.WriteDebug("{0} loaded!", ret.ToString());
                                    Add(ret);
                                    ok = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteDebug(ex.Message);
                }
            }
            return ok ? ret : default(T);
        }
    }
}