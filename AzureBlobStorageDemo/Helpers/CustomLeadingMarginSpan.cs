using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace AzureBlobStorageDemo.Helpers
{
    public class CustomLeadingMarginSpan : ILeadingMarginSpanLeadingMarginSpan2
    {
        private int _margin;
        private int _lines;
        public CustomLeadingMarginSpan(int margin, int lines)
        {
            _margin = margin;
            _lines = lines;
        }
        public void Dispose()
        {
            
        }

        public IntPtr Handle { get; }

        public void DrawLeadingMargin(Canvas c, Paint p, int x, int dir, int top, int baseline, int bottom, ICharSequence text,
            int start, int end, bool first, Layout layout)
        {

        }

        public int GetLeadingMargin(bool first)
        {
            return first ? _margin : 0;
        }

        public int LeadingMarginLineCount => _lines;
    }
}