using Sdl.Enterprise2.Platform.Contracts.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;

namespace Sdl.Enterprise2.MultiTerm.Client.IdentityModel
{
	public abstract class FaultExceptionHandlerBase
	{
		protected virtual Exception ProcessServiceException(FaultException<ServiceError> ex)
		{
			Exception ex2 = null;
			Type type;
			try
			{
				type = Type.GetType(ex.Detail.AssemblyQualifiedName);
			}
			catch (Exception)
			{
				string[] array = ex.Detail.AssemblyQualifiedName.Split(',');
				type = Type.GetType(array[0] + "," + array[1]);
			}
			if (type == null)
			{
				string assemblyName = ex.Detail.AssemblyQualifiedName.Substring(ex.Detail.AssemblyQualifiedName.IndexOf(',') + 1).TrimStart();
				AssemblyName assemblyName2 = new AssemblyName(assemblyName);
				Assembly assembly = Assembly.Load(assemblyName2.FullName);
				string[] array2 = ex.Detail.AssemblyQualifiedName.Split(',');
				try
				{
					type = assembly.GetType(array2[0]);
				}
				catch (Exception)
				{
					return new Exception(ex.Message);
				}
			}
			try
			{
				if (!(type == typeof(COMException)))
				{
					if (!(type != null))
					{
						return ex2;
					}
					ConstructorInfo constructorInfo = FindPreferredConstructor(type.GetConstructors());
					if (!(constructorInfo != null))
					{
						return ex2;
					}
					ex2 = (Exception)constructorInfo.Invoke(new object[1]
					{
						ex.Message
					});
					return ex2;
				}
				ex2 = new COMException(ex.Message, ex.Detail.ErrorCode);
				return ex2;
			}
			catch (Exception)
			{
				return ex2;
			}
			finally
			{
				if (ex2 == null)
				{
					ex2 = new Exception(ex.Message);
				}
			}
		}

		private static ConstructorInfo FindPreferredConstructor(IEnumerable<ConstructorInfo> constructors)
		{
			return (from _003C_003Eh__TransparentIdentifier0 in constructors?.Select((ConstructorInfo ci) => new
				{
					ci = ci,
					parameters = ci.GetParameters()
				})
				where _003C_003Eh__TransparentIdentifier0.parameters != null && _003C_003Eh__TransparentIdentifier0.parameters.Length == 1 && _003C_003Eh__TransparentIdentifier0.parameters[0].ParameterType == typeof(string)
				select _003C_003Eh__TransparentIdentifier0.ci).FirstOrDefault();
		}

		protected virtual Exception ProcessServiceException(FaultException ex)
		{
			return ex;
		}
	}
}
