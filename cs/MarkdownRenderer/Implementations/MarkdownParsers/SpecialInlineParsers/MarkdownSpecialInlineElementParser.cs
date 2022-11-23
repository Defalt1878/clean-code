using MarkdownRenderer.Abstractions.Elements;
using MarkdownRenderer.Abstractions.ElementsParsers;
using MarkdownRenderer.Implementations.Elements;
using MarkdownRenderer.Infrastructure;

namespace MarkdownRenderer.Implementations.MarkdownParsers.SpecialInlineParsers;

public abstract class MarkdownSpecialInlineElementParser<TElem> : ISpecialInlineElementParser
    where TElem : IElement
{
    public Type ParsingElementType => typeof(TElem);

    public abstract string Prefix { get; }

    public abstract string Postfix { get; }

    protected abstract Func<TElem> ElementCreator { get; }

    public virtual bool IsElementStart(string content, int index)
    {
        if (index + Prefix.Length + Postfix.Length >= content.Length)
            return false;
        if (Prefix.Where((c, i) => content[index + i] != c).Any())
            return false;

        return content[index + Prefix.Length] is not ' ';
    }

    public virtual bool IsElementEnd(string content, int index)
    {
        if (index - (Prefix.Length + Postfix.Length) < 0)
            return false;
        if (Postfix.Where((c, i) => content[index - Postfix.Length + i + 1] != c).Any())
            return false;

        return content[index - Postfix.Length] is not ' ';
    }

    IElement IInlineElementParser.ParseElement(string content, Token token)
    {
        if (token is ContentToken contentToken)
            return ParseElement(content, contentToken);
        return ParseElement(content, new ContentToken(token.Start, token.End, Prefix.Length, Postfix.Length));
    }

    public TElem ParseElement(string content, ContentToken contentToken)
    {
        if (!TryParseElement(content, contentToken, out var element))
            throw new ArgumentException("Unable to parse!");
        return element!;
    }

    bool ISpecialInlineElementParser.TryParseElement(string content, ContentToken contentToken, out IElement? element)
    {
        var result = TryParseElement(content, contentToken, out var tElement);
        element = tElement;
        return result;
    }

    public virtual bool TryParseElement(string content, ContentToken contentToken, out TElem? element)
    {
        element = default;
        if (contentToken.Length < Prefix.Length + Postfix.Length + 1)
            return false;
        if (!IsElementStart(content, contentToken.Start) || !IsElementEnd(content, contentToken.End))
            return false;

        var rawContent = content.Substring(contentToken);

        if (
            IsSelectionInsideWord(content, contentToken) &&
            rawContent.Any(symbol => char.IsDigit(symbol) || symbol == ' ')
        )
            return false;

        element = ElementCreator();
        return true;
    }

    protected static bool IsSelectionInsideWord(string content, Token contentToken)
    {
        if (contentToken.Start > 0 && content[contentToken.Start - 1] != ' ')
            return true;

        if (contentToken.End + 1 < content.Length && content[contentToken.End + 1] != ' ')
            return true;

        return false;
    }
}