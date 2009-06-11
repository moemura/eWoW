using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace eWoW.Scripting.Compiler
{
    public enum ScriptLanguage
    {
        CSharp,
        VisualBasic,
        CPlusPlus,
        IronPython,
        IronRuby,
        JSharp,
        FSharp
    }

    internal class CompilerBase
    {
        private ICodeCompiler _compiler;

        protected readonly string[] CommonAssemblies = new[]
                                                  {
                                                      "eWoW.Common.dll", "eWoW.Database.dll", "eWoW.Network.dll",
                                                      "System.dll", "System.Data.dll"
                                                  };

        private CompilerParameters compilerParams;

        public CompilerBase(string fileName)
        {
            using (TextReader tr = new StreamReader(fileName))
            {
                Source = tr.ReadToEnd();
            }
            AssemblyName = Path.GetFileNameWithoutExtension(fileName);
        }

        public string Source { get; set; }
        public string AssemblyName { get; set; }

        protected void SetupCompilerParams()
        {
            var p = new CompilerParameters();
            p.ReferencedAssemblies.AddRange(CommonAssemblies);
            p.CompilerOptions = "";
            p.GenerateExecutable = false;
            p.GenerateInMemory = true;
            p.IncludeDebugInformation = true;

            PolicyLevel policyLevel = PolicyLevel.CreateAppDomainLevel();
            PermissionSet perms = new PermissionSet(PermissionState.None);
            const SecurityPermissionFlag permissionFlags = SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.RemotingConfiguration |
                                                           SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.SkipVerification |
                                                           SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.Execution;
            perms.AddPermission(new SecurityPermission(permissionFlags));

            perms.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.AllFlags));

            PolicyStatement policy = new PolicyStatement(perms, PolicyStatementAttribute.Exclusive);
            CodeGroup group = new UnionCodeGroup(new AllMembershipCondition(), policy);
            policyLevel.RootCodeGroup = group;
            AppDomain.CurrentDomain.SetAppDomainPolicy(policyLevel);

            // All that work, and now we finally set the value =)
            compilerParams = p;
        }
    }
}