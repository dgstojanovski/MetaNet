using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MetaNet.Core;
using MetaNet.Core.Attributes;
using static MetaNet.Generators.Markdown.MarkdownGenerator;

namespace MetaNet.Generators.Markdown
{
    public class ValueMarkdownGenerator : IGenerator<ValueAttribute>
    {
        public IEnumerable<FileInfo> Generate(Type type, DirectoryInfo destination)
        {
            var metadata = type.GetCustomAttribute<ValueAttribute>()
                ?? throw new InvalidOperationException($"Type {type.FullName} missing {nameof(ValueAttribute)}");

            var properties = type.GetMembers()
                .Where(m => m.GetCustomAttribute<PropertyAttribute>() != null)
                .Select(p => new { Member = p, Metadata = p.GetCustomAttribute<PropertyAttribute>()})
                .ToArray();

            var directory = destination.CreateSubdirectory(OutputFolder).CreateSubdirectory(ValuesFolder);
            var path = Path.Combine(directory.FullName, $"{type.Name}.md");
            
            using var writer = File.CreateText(path);
            writer.WriteChapter($"Value {type.FullName}");

            writer.WriteSection(PropertiesSection);
            
            foreach (var property in properties)
            {
                writer.WriteSubSection(property.Member.Name);
                writer.WriteLine(property.Metadata.Description);
            }
            
            return new[] { new FileInfo(path) };
        }
    }
}
