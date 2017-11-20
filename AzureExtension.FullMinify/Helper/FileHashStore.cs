﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AzureExtension.FullMinify.Helper
{
    /// <summary>
    /// Taken from: https://github.com/ligershark/AzureJobs
    /// </summary>
    public class FileHashStore
    {
        private string _filePath;
        private Dictionary<string, string> _store = new Dictionary<string, string>();
        private static object _syncRoot = new object();

        public FileHashStore(string logfolder)
        {
            _filePath = Path.Combine(logfolder, "index.txt");

            var dir = Path.GetDirectoryName(_filePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            Load();
        }

        public int Count => _store.Count;

        private void Load()
        {
            try
            {
                // If the file hasn't been created yet, just ignore it.
                if (!File.Exists(_filePath))
                    return;

                foreach (string line in File.ReadAllLines(_filePath))
                {
                    string[] args = line.Split('|');

                    if (args.Length == 2 && !_store.ContainsKey(args[0]))
                        _store.Add(args[0], args[1]);
                }
            }
            catch
            {
                // Do nothing. The file format has changed and will be overwritten next time Save() is called.
            }
        }

        private string GetLogFilePath(string path)
        {
            Directory.CreateDirectory(path);
            string name = Assembly.GetEntryAssembly().ManifestModule.Name;
            return Path.Combine(path, name + "");
        }

        public void Save(string file)
        {
            bool exist = _store.ContainsKey(file);

            try
            {
                lock (_syncRoot)
                {
                    _store[file] = GetHash(file);

                    if (!exist)
                    {
                        // If the file is new to the azure job, just append it to the existing file
                        File.AppendAllLines(_filePath, new[] { file + "|" + _store[file] });
                    }
                    else
                    {
                        // If the file is known we must avoid duplicates, so this just writes the entire store
                        StringBuilder sb = new StringBuilder();

                        foreach (string key in _store.Keys)
                        {
                            sb.AppendLine(key + "|" + _store[key]);
                        }

                        File.WriteAllText(_filePath, sb.ToString());
                    }
                }
            }
            catch { }
        }

        public bool HasChangedOrIsNew(string file)
        {
            if (!_store.ContainsKey(file))
                return true;

            string currentHash = GetHash(file);

            if (string.IsNullOrEmpty(currentHash))
                return true;

            return currentHash != _store[file];
        }

        private string GetHash(string file)
        {
            try
            {
                if (!File.Exists(file))
                    return null;

                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(file))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash);
                }
            }
            catch
            {
                return null;
            }
        }
    }

    [XmlType(TypeName = "item")]
    public class Item
    {
        [XmlAttribute(AttributeName = "file")]
        public string File;

        [XmlAttribute(AttributeName = "hash")]
        public string Hash;
    }
}
