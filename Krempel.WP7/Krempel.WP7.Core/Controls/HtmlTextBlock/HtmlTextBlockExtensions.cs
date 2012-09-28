using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls;

namespace Krempel.WP7.Core.Controls
{
    public static class HtmlTextBlockExtensions
    {
        public static IEnumerable<Inline> GetChildInlines(this RichTextBox richTextBox)
        {
            foreach (var block in richTextBox.Blocks.OfType<Paragraph>())
            {
                foreach (var inline in block.GetChildInlines())
                {
                    yield return inline;
                }
            }
        }

        public static IEnumerable<Inline> GetChildInlines(this Paragraph paragraph)
        {
            foreach (var inline in paragraph.Inlines)
            {
                yield return inline;

                if (inline is Span)
                {
                    foreach (var subInline in ((Span)inline).GetChildInlines())
                    {
                        yield return subInline;
                    }
                }
            }
        }

        public static IEnumerable<Inline> GetChildInlines(this Span span)
        {
            foreach (var inline in span.Inlines)
            {
                yield return inline;

                if (inline is Span)
                {
                    foreach (var subInline in ((Span)inline).GetChildInlines())
                    {
                        yield return subInline;
                    }
                }
            }
        }
    }
}
