using System.Runtime.InteropServices;
using SmartFormNETDLL;

namespace AxSmartFormNETDLL;

[ClassInterface(ClassInterfaceType.None)]
public class AxSFLayoutPrintEventMulticaster : __SFLayoutPrint
{
	private AxSFLayoutPrint parent;

	public AxSFLayoutPrintEventMulticaster(AxSFLayoutPrint parent)
	{
		this.parent = parent;
	}
}
