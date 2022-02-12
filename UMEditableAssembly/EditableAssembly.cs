using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Plugins.UMEditableAssembly
{
    [Serializable]
    class FakeClass
    {
        [SerializeField]
        internal string name;
        [SerializeField]
        internal string rootNamespace;
        [SerializeField]
        internal List<string> references;
        [SerializeField]
        internal List<string> includePlatforms;
        [SerializeField]
        internal List<string> excludePlatforms;
        [SerializeField]
        internal bool allowUnsafeCode;
        [SerializeField]
        internal bool overrideReferences;
        [SerializeField]
        internal List<string> precompiledReferences;
        [SerializeField]
        internal bool autoReferenced;
        [SerializeField]
        internal List<string> defineConstraints;
        [SerializeField]
        internal List<string> versionDefines;
        [SerializeField]
        internal bool noEngineReferences;

        public bool NoEngineReferences => noEngineReferences;

        public string[] VersionDefines => versionDefines.ToArray();

        public string[] DefineConstraints => defineConstraints.ToArray();

        public bool AutoReferenced => autoReferenced;

        public string[] PrecompiledReferences => precompiledReferences.ToArray();

        public bool OverrideReferences => overrideReferences;

        public bool AllowUnsafeCode => allowUnsafeCode;

        public string[] ExcludePlatforms => excludePlatforms.ToArray();

        public string[] IncludePlatforms => includePlatforms.ToArray();

        public string[] References => references.ToArray();

        public string RootNamespace => rootNamespace;

        public string Name => name;
    }
    public class EditableAssembly
    {
        private FakeClass _fakeClass;
        public bool NoEngineReferences => _fakeClass.NoEngineReferences;

        public string[] VersionDefines => _fakeClass.VersionDefines;

        public string[] DefineConstraints => _fakeClass.DefineConstraints;

        public bool AutoReferenced => _fakeClass.AutoReferenced;

        public string[] PrecompiledReferences => _fakeClass.PrecompiledReferences;

        public bool OverrideReferences => _fakeClass.OverrideReferences;

        public bool AllowUnsafeCode => _fakeClass.AllowUnsafeCode;

        public string[] ExcludePlatforms => _fakeClass.ExcludePlatforms;

        public string[] IncludePlatforms => _fakeClass.IncludePlatforms;

        public string[] References => _fakeClass.References;

        public string RootNamespace => _fakeClass.RootNamespace;

        public string Name => _fakeClass.Name;

        private readonly AssemblyDefinitionAsset _definitionAsset;
        private readonly string _assemblyPath;
        

        public EditableAssembly(AssemblyDefinitionAsset definitionAsset)
        {
            _definitionAsset = definitionAsset;
            _fakeClass = JsonUtility.FromJson<FakeClass>(_definitionAsset.text);
            _assemblyPath = AssetDatabase.GetAssetPath(definitionAsset);
        }

        public EditableAssembly(string definitionAssetName)
        {
            _assemblyPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(definitionAssetName);
            _definitionAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(_assemblyPath);
            _fakeClass = JsonUtility.FromJson<FakeClass>(_definitionAsset.text);
        }

        public bool AddAssembly(AssemblyDefinitionAsset asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);
            var st = "GUID:" + guid.ToString();
            if (!_fakeClass.references.Contains(st))
            {
                _fakeClass.references.Add(st);
                return true;
            }

            return false;
        }

        public void Save(bool reimport = true)
        {
            using (StreamWriter sw = new StreamWriter(_assemblyPath))
            {
                sw.Write(JsonUtility.ToJson(_fakeClass));
            }
            if(reimport)
                AssetDatabase.ImportAsset(_assemblyPath);
        }
    }
}
