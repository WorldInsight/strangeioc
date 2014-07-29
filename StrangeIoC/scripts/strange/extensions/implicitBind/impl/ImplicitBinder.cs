/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 * Modified  2014 DB Systel GmbH
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using strange.extensions.implicitBind.api;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

namespace strange.extensions.implicitBind.impl
{

	public class ImplicitBinder : IImplicitBinder
	{

		[Inject]
		public IInjectionBinder injectionBinder { get; set; }

		[Inject]
		public ITypeRegistry typeRegistry { get; set; }
		
		//Hold a copy of the assembly so we aren't retrieving this multiple times. 
		private Assembly assembly;
	
		/// <summary>
		/// Search through indicated namespaces and scan for all annotated classes.
		/// Automatically create bindings
		/// </summary>
		/// <param name="usingNamespaces">Array of namespaces. Compared using StartsWith. </param>

		public virtual void ScanForAnnotatedClasses(string[] usingNamespaces)
		{
			IEnumerable<Type> types = typeRegistry.GetRegisteredTypes();
			if (types.Count() != 0)
			{
				List<Type> typesInNamespaces = new List<Type>();
				int namespacesLength = usingNamespaces.Length;
				for (int ns = 0; ns < namespacesLength; ns++)
				{
					typesInNamespaces.AddRange(types.Where(t => !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(usingNamespaces[ns])));
				}

				IEnumerable<Type> concreteBinderClasses = types.Where(
					t => t.BaseType != null && t.BaseType.Name.Equals(typeof(AbstractConcreteBinder<>).Name) //&& t != typeof(AbstractConcreteBinder<>)
				);

				foreach(Type binder in concreteBinderClasses)
				{
					Type[] genericTypes = binder.BaseType.GetGenericArguments();

					//object instance = Activator.CreateInstance(binder);
					if (injectionBinder.GetBinding(binder) == null)
					{
						injectionBinder.Bind(binder).ToSingleton();
					}
					object instance = injectionBinder.GetInstance(binder);

					((IConcreteBinder)instance).ExecuteBindings(typesInNamespaces);
				}
			}
			else
			{
				throw new InjectionException("Assembly was not initialized yet for Implicit Bindings!", InjectionExceptionType.UNINITIALIZED_ASSEMBLY);
			}
		}
	}
}
