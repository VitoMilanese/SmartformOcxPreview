using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SmartFormNETDLL;

[ComImport]
[Guid("1FE93EF5-013F-45CF-8A1F-DE695A2F3555")]
[TypeLibType(4304)]
public interface _SFLayoutPrint
{
	void _VtblGap7_482();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809344)]
	short PreviewLayout([In] short PageNumber, [In] short Zoom);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809345)]
	short PrintLayout([In] short PrintingType, [In] short PageSelect, [In] short Zoom);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809346)]
	short SetLayout([In][MarshalAs(UnmanagedType.BStr)] string LayoutName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809347)]
	short SetField([In][MarshalAs(UnmanagedType.BStr)] string FieldName, [In][MarshalAs(UnmanagedType.BStr)] string Value);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809348)]
	short Init([In][MarshalAs(UnmanagedType.BStr)] string IniFileName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809349)]
	[return: MarshalAs(UnmanagedType.BStr)]
	string GetErrorMsg();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809350)]
	short SetPrinter([In][MarshalAs(UnmanagedType.BStr)] string PrinterName);

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809351)]
	void ResizeToZeroPb();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809353)]
	short ZoomIn();

	[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	[DispId(1610809354)]
	short ZoomOut();
}
