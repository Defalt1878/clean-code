using MarkdownRenderer.Abstractions.Elements;
using MarkdownRenderer.Abstractions.ElementsParsers;
using MarkdownRenderer.Implementations.Elements;

namespace MarkdownRenderer.Implementations.MarkdownParsers;

public class MarkdownParagraphParser : ILineElementParser
{
    public Type ParsingElementType { get; } = typeof(ParagraphElement);

    IElement ILineElementParser.ParseElement(string content) =>
        ParseElement();

    public ParagraphElement ParseElement() => new();
}