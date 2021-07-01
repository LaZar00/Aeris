using System.Windows.Forms;

namespace Aeris
{

    public class logEvents
    {


        public static bool bActivateLogging;

        public static void ClearEvents(RichTextBox rtb)
        {
            rtb.Clear();
        }

        public static void AddEventText(string evnttxt, RichTextBox rtb)
        {
            rtb.AppendText(evnttxt + "\r");
        }

        public static void SaveEvents(string FileName, RichTextBox rtbEvents)
        {
            rtbEvents.SaveFile(FileName, RichTextBoxStreamType.PlainText);
        }
    }
}