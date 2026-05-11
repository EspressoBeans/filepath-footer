using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace FilepathFooter.Editor
{
    /// <summary>
    /// MEF provider that creates a <see cref="FilepathFooterMargin"/> for every
    /// document editor window.  The margin is placed below the horizontal
    /// scroll bar in the bottom margin container.
    /// </summary>
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(FilepathFooterMargin.MarginName)]
    [Order(After = PredefinedMarginNames.HorizontalScrollBar)]
    [MarginContainer(PredefinedMarginNames.Bottom)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class FilepathFooterMarginFactory : IWpfTextViewMarginProvider
    {
        [Import]
        internal ITextDocumentFactoryService TextDocumentFactoryService { get; set; } = null!;

        public IWpfTextViewMargin CreateMargin(
            IWpfTextViewHost wpfTextViewHost,
            IWpfTextViewMargin marginContainer)
        {
            return new FilepathFooterMargin(
                wpfTextViewHost.TextView,
                TextDocumentFactoryService);
        }
    }
}
