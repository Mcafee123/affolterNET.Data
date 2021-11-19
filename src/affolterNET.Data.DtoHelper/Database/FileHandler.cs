using System;
using System.IO;
using affolterNET.Data.DtoHelper.CodeGen;
using affolterNET.Data.DtoHelper.Interfaces;

namespace affolterNET.Data.DtoHelper.Database
{
    public class FileHandler : IFileHandler
    {
        private readonly string fileName;

        public FileHandler(GeneratorCfg props)
        {
            if (string.IsNullOrWhiteSpace(props.TargetFile))
            {
                throw new ArgumentException($"{props.TargetFile} not found");
            }

            fileName = props.TargetFile;
            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.Combine(Environment.CurrentDirectory, fileName);
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"{fileName} not found");
            }
        }

        public void WriteCode(string code)
        {
            File.WriteAllText(fileName, code);
        }
    }
}