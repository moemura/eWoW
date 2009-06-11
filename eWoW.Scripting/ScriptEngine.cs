using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace eWoW.Scripting
{
    public class ScriptEngine : MarshalByRefObject
    {
        private ICodeCompiler compiler;
        private CompilerParameters compilerParams;

        public static ScriptEngine CreateInSeparateDomain(string domainName)
        {
            AppDomain domain = AppDomain.CreateDomain(domainName);
            return domain.CreateInstanceAndUnwrap(typeof(ScriptEngine).Assembly.GetName().Name, typeof(ScriptEngine).FullName)
                as ScriptEngine;
        }


    }
}
