using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFClientApp
{
    public class BoxWriter : TextWriter
    {
        TextBox textbox = null;

        public BoxWriter(TextBox output)
        {
            textbox = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            textbox.AppendText(value.ToString());
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
