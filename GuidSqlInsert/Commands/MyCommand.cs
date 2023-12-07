using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using static System.Linq.Enumerable;
using System;
using Task = System.Threading.Tasks.Task;

namespace GuidSqlInsert
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            var position = doc?.TextView?.Caret.Position.BufferPosition;
            if (position.HasValue)
            {
                var input = doc.TextView.GetTextViewLineContainingBufferPosition(position.Value).Extent.GetText();

                int guidcount;
                var isnumber = int.TryParse(input, out guidcount);

                var guidlist = GetGuidList(count: isnumber ? guidcount : 1);

                if (isnumber && guidcount > 0)
                {
                    doc.TextBuffer?.Replace(doc.TextView.GetTextViewLineContainingBufferPosition(position.Value).Extent.Span, guidlist);
                }
                else
                {
                    doc.TextBuffer.Insert(position.Value, guidlist);
                }
            }
        }

        private string GetGuidList(int count = 1) => string.Join(",\n", Range(1, count).Select(c => $"('{Guid.NewGuid()}')"));
    }
}
