using System;
using System.ComponentModel;
using System.Windows.Forms;
using SmartFormNETDLL;

namespace AxSmartFormNETDLL;

[Clsid("{4ecea871-fb05-4329-85ea-a9cc47659308}")]
[DesignTimeVisible(true)]
public class AxSFLayoutPrint : AxHost
{
	private _SFLayoutPrint ocx;

	private AxSFLayoutPrintEventMulticaster eventMulticaster;

	private ConnectionPointCookie cookie;

	public AxSFLayoutPrint()
		: base("4ecea871-fb05-4329-85ea-a9cc47659308")
	{
	}

	public virtual short PreviewLayout(short pageNumber, short zoom)
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("PreviewLayout", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.PreviewLayout(pageNumber, zoom);
	}

	public virtual short PrintLayout(short printingType, short pageSelect, short zoom)
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("PrintLayout", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.PrintLayout(printingType, pageSelect, zoom);
	}

	public virtual short SetLayout(string layoutName)
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("SetLayout", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.SetLayout(layoutName);
	}

	public virtual short SetField(string fieldName, string value)
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("SetField", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.SetField(fieldName, value);
	}

	public virtual short Init(string iniFileName)
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("Init", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.Init(iniFileName);
	}

	public virtual string GetErrorMsg()
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("GetErrorMsg", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.GetErrorMsg();
	}

	public virtual short SetPrinter(string printerName)
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("SetPrinter", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.SetPrinter(printerName);
	}

	public virtual void ResizeToZeroPb()
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("ResizeToZeroPb", ActiveXInvokeKind.MethodInvoke);
		}
		ocx.ResizeToZeroPb();
	}

	public virtual short ZoomIn()
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("ZoomIn", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.ZoomIn();
	}

	public virtual short ZoomOut()
	{
		if (ocx == null)
		{
			throw new InvalidActiveXStateException("ZoomOut", ActiveXInvokeKind.MethodInvoke);
		}
		return ocx.ZoomOut();
	}

	protected override void CreateSink()
	{
		try
		{
			eventMulticaster = new AxSFLayoutPrintEventMulticaster(this);
			cookie = new ConnectionPointCookie(ocx, eventMulticaster, typeof(__SFLayoutPrint));
		}
		catch (Exception)
		{
		}
	}

	protected override void DetachSink()
	{
		try
		{
			cookie.Disconnect();
		}
		catch (Exception)
		{
		}
	}

	protected override void AttachInterfaces()
	{
		try
		{
			ocx = (_SFLayoutPrint)GetOcx();
		}
		catch (Exception)
		{
		}
	}
}
