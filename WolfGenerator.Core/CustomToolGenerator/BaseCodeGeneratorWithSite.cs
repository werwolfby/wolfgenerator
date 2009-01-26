using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Designer.Interfaces;
using VSLangProj;
using CodeNamespace=System.CodeDom.CodeNamespace;

namespace CustomToolGenerator
{
	/// <summary>
	///     This class exists to be cocreated a in a preprocessor build step.
	/// </summary>
	public abstract class BaseCodeGeneratorWithSite : BaseCodeGenerator, IObjectWithSite
	{
		private const int E_FAIL = unchecked((int)0x80004005);
		private const int E_NOINTERFACE = unchecked((int)0x80004002);

		private object site;
		private CodeDomProvider codeDomProvider;
		private static readonly Guid CodeDomInterfaceGuid = new Guid( "{73E59688-C7C4-4a85-AF64-A538754784C5}" );
		private static readonly Guid CodeDomServiceGuid = CodeDomInterfaceGuid;
		private ServiceProvider serviceProvider;

		/// <summary>
		/// demand-creates a CodeDomProvider
		/// </summary>
		protected virtual CodeDomProvider CodeProvider
		{
			get
			{
				if (this.codeDomProvider == null)
				{
					var vsmdCodeDomProvider = (IVSMDCodeDomProvider)this.GetService( CodeDomServiceGuid );
					if (vsmdCodeDomProvider != null) this.codeDomProvider = (CodeDomProvider)vsmdCodeDomProvider.CodeDomProvider;
					Debug.Assert( this.codeDomProvider != null,
					              "Get CodeDomProvider Interface failed.  GetService(QueryService(CodeDomProvider) returned Null." );
				}
				return this.codeDomProvider;
			}
			set
			{
				if (value == null) throw new ArgumentNullException();

				this.codeDomProvider = value;
			}
		}

		/// <summary>
		/// demand-creates a ServiceProvider given an IOleServiceProvider
		/// </summary>
		private ServiceProvider SiteServiceProvider
		{
			get
			{
				if (this.serviceProvider == null)
				{
					var oleServiceProvider = this.site as IOleServiceProvider;
					Debug.Assert( oleServiceProvider != null, "Unable to get IOleServiceProvider from site object." );

					this.serviceProvider = new ServiceProvider( oleServiceProvider );
				}
				return this.serviceProvider;
			}
		}

		/// <summary>
		/// method to get a service by its GUID
		/// </summary>
		/// <param name="serviceGuid">GUID of service to retrieve</param>
		/// <returns>an object that implements the requested service</returns>
		protected object GetService( Guid serviceGuid )
		{
			return this.SiteServiceProvider.GetService( serviceGuid );
		}

		/// <summary>
		/// method to get a service by its Type
		/// </summary>
		/// <param name="serviceType">Type of service to retrieve</param>
		/// <returns>an object that implements the requested service</returns>
		protected object GetService( Type serviceType )
		{
			return this.SiteServiceProvider.GetService( serviceType );
		}

		/// <summary>
		/// gets the default extension of the output file by asking the CodeDomProvider
		/// what its default extension is.
		/// </summary>
		/// <returns></returns>
		public override string GetDefaultExtension()
		{
			var codeDom = this.CodeProvider;
			Debug.Assert( codeDom != null, "CodeDomProvider is NULL." );
			var extension = codeDom.FileExtension;
			if (!string.IsNullOrEmpty( extension )) if (extension[0] != '.') extension = "." + extension;

			return extension;
		}

		/// <summary>
		/// Method to get an ICodeGenerator with which this class can create code.
		/// </summary>
		/// <returns></returns>
		protected virtual ICodeGenerator GetCodeWriter()
		{
			var codeDom = this.CodeProvider;
			if (codeDom != null) return codeDom.CreateGenerator();

			return null;
		}

		/// <summary>
		/// SetSite method of IOleObjectWithSite
		/// </summary>
		/// <param name="pUnkSite">site for this object to use</param>
		public virtual void SetSite( object pUnkSite )
		{
			this.site = pUnkSite;
			this.codeDomProvider = null;
			this.serviceProvider = null;
		}

		/// <summary>
		/// GetSite method of IOleObjectWithSite
		/// </summary>
		/// <param name="riid">interface to get</param>
		/// <param name="ppvSite">array in which to stuff return value</param>
		public virtual void GetSite( ref Guid riid, object[] ppvSite )
		{
			if (ppvSite == null) throw new ArgumentNullException( "ppvSite" );
			if (ppvSite.Length < 1) throw new ArgumentException( "ppvSite array must have at least 1 member", "ppvSite" );

			if (this.site == null) throw new COMException( "object is not sited", E_FAIL );

			var pUnknownPointer = Marshal.GetIUnknownForObject( this.site );
			IntPtr intPointer;
			Marshal.QueryInterface( pUnknownPointer, ref riid, out intPointer );

			if (intPointer == IntPtr.Zero) throw new COMException( "site does not support requested interface", E_NOINTERFACE );

			ppvSite[0] = Marshal.GetObjectForIUnknown( intPointer );
		}

		/// <summary>
		/// gets a string containing the DLL names to add.
		/// </summary>
		/// <param name="DLLToAdd"></param>
		/// <returns></returns>
		private static string GetDLLNames( string[] DLLToAdd )
		{
			if (DLLToAdd == null || DLLToAdd.Length == 0) return string.Empty;

			var dllNames = DLLToAdd[0];
			for (var i = 1; i < DLLToAdd.Length; i++) dllNames = dllNames + ", " + DLLToAdd[i];
			return dllNames;
		}

		/// <summary>
		/// adds a reference to the project for each required DLL
		/// </summary>
		/// <param name="referenceDLL"></param>
		protected void AddReferenceDLLToProject( string[] referenceDLL )
		{
			if (referenceDLL.Length == 0) return;

			var serviceObject = this.GetService( typeof(ProjectItem) );
			Debug.Assert( serviceObject != null, "Unable to get Project Item." );
			if (serviceObject == null)
			{
				var errorMessage = String.Format( "Unable to add DLL to project references: {0}.  Please Add them manually.",
				                                  GetDLLNames( referenceDLL ) );
				this.GeneratorErrorCallback( false, 1, errorMessage, 0, 0 );
				return;
			}

			var containingProject = ((ProjectItem)serviceObject).ContainingProject;
			Debug.Assert( containingProject != null, "GetService(typeof(Project)) return null." );
			if (containingProject == null)
			{
				var errorMessage = String.Format( "Unable to add DLL to project references: {0}.  Please Add them manually.",
				                                  GetDLLNames( referenceDLL ) );
				this.GeneratorErrorCallback( false, 1, errorMessage, 0, 0 );
				return;
			}

			var vsProj = containingProject.Object as VSProject;
			Debug.Assert( vsProj != null, "Unable to ADD DLL to current project.  Project.Object does not implement VSProject." );
			if (vsProj == null)
			{
				var errorMessage = String.Format( "Unable to add DLL to project references: {0}.  Please Add them manually.",
				                                  GetDLLNames( referenceDLL ) );
				this.GeneratorErrorCallback( false, 1, errorMessage, 0, 0 );
				return;
			}

			try
			{
				for (var i = 0; i < referenceDLL.Length; i++) vsProj.References.Add( referenceDLL[i] );
			}
			catch (Exception e)
			{
				Debug.Fail( "**** ERROR: vsProj.References.Add() throws exception: " + e.ToString() );

				var errorMessage = String.Format( "Unable to add DLL to project references: {0}.  Please Add them manually.",
				                                  GetDLLNames( referenceDLL ) );
				this.GeneratorErrorCallback( false, 1, errorMessage, 0, 0 );
				return;
			}
		}

		/// <summary>
		/// method to create an exception message given an exception
		/// </summary>
		/// <param name="e">exception caught</param>
		/// <returns>message to display to the user</returns>
		protected virtual string CreateExceptionMessage( Exception e )
		{
			var message = e.Message ?? string.Empty;

			var innerException = e.InnerException;
			while (innerException != null)
			{
				var innerMessage = innerException.Message;
				if (!string.IsNullOrEmpty( innerMessage )) message = message + " " + innerMessage;
				innerException = innerException.InnerException;
			}

			return message;
		}

		/// <summary>
		/// method to create a version comment
		/// </summary>
		/// <param name="codeNamespace"></param>
		protected virtual void GenerateVersionComment( CodeNamespace codeNamespace )
		{
			codeNamespace.Comments.Add( new CodeCommentStatement( string.Empty ) );
			codeNamespace.Comments.Add(
				new CodeCommentStatement( String.Format( "This source code was auto-generated by {0}, Version {1}.",
				                                         Assembly.GetExecutingAssembly().GetName().Name,
				                                         Environment.Version ) ) );
			codeNamespace.Comments.Add( new CodeCommentStatement( string.Empty ) );
		}
	}
}