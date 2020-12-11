using System;

namespace Joonaxii.ConsoleBulletHell
{
    public class TextLine
    {
        public bool IsOK { get; }
        public bool MarkedForClear { get; set; }

        public ConsoleColor Color;

        public string text;
        public int textLength;

        public bool isDirty;
        public bool written;

        public TextLine(bool isOK)
        {
            text = "";
            textLength = 0;
            IsOK = isOK;

            MarkedForClear = false;
            isDirty = false;
            written = false;
        }

        public void SetText(string txt)
        {
            MarkedForClear = false;
            isDirty = !written | text != txt;

            text = txt;
            textLength = txt.Length;

            written = false;
        }

        public void Clear()
        {
            isDirty = false;
            MarkedForClear = false;
            text = string.Empty;
            textLength = 0;
            written = true;
        }
    }
}