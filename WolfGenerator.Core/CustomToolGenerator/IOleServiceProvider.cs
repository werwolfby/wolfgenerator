using System;
using System.Runtime.InteropServices;

namespace CustomToolGenerator
{
	[ComImport]
	[Guid( "6D5140C1-7436-11CE-8034-00AA006009FA" )]
	[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
	public interface IOleServiceProvider
	{
		[PreserveSig]
		int QueryService( [In] ref Guid guidService,
		                  [In] ref Guid riid,
		                  out IntPtr ppvObject );
	}
}