using System.Runtime.InteropServices;

namespace SmartFormNETDLL;

[ClassInterface(ClassInterfaceType.None)]
[TypeLibType(TypeLibTypeFlags.FHidden)]
public sealed class __SFLayoutPrint_SinkHelper : __SFLayoutPrint
{
	public int m_dwCookie;

	internal __SFLayoutPrint_SinkHelper()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		m_dwCookie = 0;
	}
}
