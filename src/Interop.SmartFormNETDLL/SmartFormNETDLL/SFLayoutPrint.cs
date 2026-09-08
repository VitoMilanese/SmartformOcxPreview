using System.Runtime.InteropServices;

namespace SmartFormNETDLL;

[ComImport]
[Guid("1FE93EF5-013F-45CF-8A1F-DE695A2F3555")]
[CoClass(typeof(SFLayoutPrintClass))]
public interface SFLayoutPrint : _SFLayoutPrint, __SFLayoutPrint_Event
{
}
