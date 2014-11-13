/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 * Modified 2014 DB Systel GmbH
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
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

namespace strange.extensions.implicitBind.impl
{
	[ConcreteBinder(40)]
	public class ConcreteImplementedByBinder : AbstractConcreteBinder<ImplementedBy>
	{
		public override void ExecuteBindings(List<Type> typesInNamespaces)
		{
			this.typesInNamespaces = typesInNamespaces;

			List<ImplicitBindingVO> implementedByBindings = new List<ImplicitBindingVO>();

			foreach (Type type in GetAttributedTypes())
			{
				Type[] interfaces = type.GetInterfaces();

				object name = null;
				bool isCrossContext = false;
				List<Type> bindTypes = new List<Type>();

				foreach (ImplementedBy implBy in type.GetCustomAttributes(typeof(ImplementedBy), true))
				{
					if (implBy.DefaultType.GetInterfaces().Contains(type)) //Verify this DefaultType exists and implements the tagged interface
					{
						implementedByBindings.Add(new ImplicitBindingVO(type, implBy.DefaultType, implBy.Scope == InjectionBindingScope.CROSS_CONTEXT, null));
					}
					else
					{
						throw new InjectionException("Default Type: " + implBy.DefaultType.Name + " does not implement annotated interface " + type.Name,
							InjectionExceptionType.IMPLICIT_BINDING_IMPLEMENTOR_DOES_NOT_IMPLEMENT_INTERFACE);
					}
				}
			}

			implementedByBindings.ForEach(Bind);
		}
	}
}
