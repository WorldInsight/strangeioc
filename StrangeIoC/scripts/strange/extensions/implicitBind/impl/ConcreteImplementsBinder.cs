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
using strange.framework.api;

namespace strange.extensions.implicitBind.impl
{
	[ConcreteBinder(41)]
	public class ConcreteImplementsBinder : AbstractConcreteBinder<Implements>
	{
		public override void ExecuteBindings(List<Type> typesInNamespaces)
		{
			this.typesInNamespaces = typesInNamespaces;

			List<ImplicitBindingVO> implementsBindings = new List<ImplicitBindingVO>();

			foreach (Type type in GetAttributedTypes())
			{
				Type[] interfaces = type.GetInterfaces();

				foreach (Implements impl in type.GetCustomAttributes(typeof(Implements), true))
				{
					object name = impl.Name ?? BindingConst.NULLOID;
					List<Type> bindTypes = new List<Type>();

					//Confirm this type implements the type specified
					if (impl.DefaultInterface != null)
					{
						//Verify this Type implements the passed interface
						if (interfaces.Contains(impl.DefaultInterface) || type == impl.DefaultInterface || type.BaseType == impl.DefaultInterface)
						{
							bindTypes.Add(impl.DefaultInterface);
						}
						else
						{
							throw new InjectionException("Annotated type " + type.Name + " does not implement Default Interface " + impl.DefaultInterface.Name,
							InjectionExceptionType.IMPLICIT_BINDING_TYPE_DOES_NOT_IMPLEMENT_DESIGNATED_INTERFACE);
						}
					}
					else //Concrete
					{
						bindTypes.Add(type);
					}

					ImplicitBindingVO thisBindingVo = new ImplicitBindingVO(bindTypes, type, InjectionBindingScope.CROSS_CONTEXT.Equals(impl.Scope), name);
					implementsBindings.Add(thisBindingVo);
				}
			}

			implementsBindings.ForEach(Bind);
		}
	}
}