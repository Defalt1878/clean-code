using MarkdownRenderer.Abstractions;

namespace MarkdownRenderer.Implementations.Elements;

public class ParagraphElement : StandardElement,
    IStorageOf<ItalicElement>, IStorageOf<PlainText>, IStorageOf<StrongElement>
{
    public ParagraphElement(string rawContent) : base(rawContent)
    {
    }
}