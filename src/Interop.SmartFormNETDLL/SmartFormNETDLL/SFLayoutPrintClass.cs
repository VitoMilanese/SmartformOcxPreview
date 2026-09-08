using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SmartFormNETDLL;

[ComImport]
[ClassInterface((short)0)]
[ComSourceInterfaces("SmartFormNETDLL.__SFLayoutPrint\0\0")]
[Guid("4ECEA871-FB05-4329-85EA-A9CC47659308")]
[TypeLibType(32)]
public class SFLayoutPrintClass : _SFLayoutPrint, SFLayoutPrint, __SFLayoutPrint_Event
{
	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809344)]
	public virtual extern short PreviewLayout([In] short PageNumber, [In] short Zoom);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809345)]
	public virtual extern short PrintLayout([In] short PrintingType, [In] short PageSelect, [In] short Zoom);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809346)]
	public virtual extern short SetLayout([In][MarshalAs(UnmanagedType.BStr)] string LayoutName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809347)]
	public virtual extern short SetField([In][MarshalAs(UnmanagedType.BStr)] string FieldName, [In][MarshalAs(UnmanagedType.BStr)] string Value);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809348)]
	public virtual extern short Init([In][MarshalAs(UnmanagedType.BStr)] string IniFileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809349)]
	[return: MarshalAs(UnmanagedType.BStr)]
	public virtual extern string GetErrorMsg();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809350)]
	public virtual extern short SetPrinter([In][MarshalAs(UnmanagedType.BStr)] string PrinterName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809351)]
	public virtual extern void ResizeToZeroPb();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809353)]
	public virtual extern short ZoomIn();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809354)]
	public virtual extern short ZoomOut();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	public virtual extern void _VtblGap7_482();
}
