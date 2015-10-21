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
using System.Reflection;
using strange.extensions.injector.api;
using strange.extensions.implicitBind.api;

namespace strange.extensions.implicitBind.impl
{
	public abstract class AbstractConcreteBinder<T> : IConcreteBinder where T : Attribute
	{
		[Inject]
		public IInjectionBinder injectionBinder { get; set; }

		protected List<Type> typesInNamespaces;

		protected List<Type> GetAttributedTypes()
		{
			List<Type> ret = new List<Type>();

			ret = typesInNamespaces.Where<Type>(t => t.GetCustomAttributes(typeof(T), true).Length > 0).ToList();
			return ret;
		}

		protected List<PropertyInfo> GetAttributedProperties()
		{
			List<PropertyInfo> ret = new List<PropertyInfo>();

			foreach (Type classType in typesInNamespaces)
			{
				ret.AddRange(classType.GetProperties().Where<PropertyInfo>(p => p.GetCustomAttributes(typeof(T), false).Length > 0).ToList());
			}
			return ret;
		}

		protected void Bind(ImplicitBindingVO toBind)
		{
			//We do not check for the existence of a binding. Because implicit bindings are weak bindings, they are overridden automatically by other implicit bindings
			//Therefore, ImplementedBy will be overriden by an Implements to that interface.

			IInjectionBinding binding = injectionBinder.Bind(toBind.BindTypes.First());
			binding.Weak();//SDM2014-0120: added as part of cross-context implicit binding fix (moved from below)

			for (int i = 1; i < toBind.BindTypes.Count; i++)
			{
				Type bindType = toBind.BindTypes.ElementAt(i);
				binding.Bind(bindType);
			}

			binding = toBind.ToType != null ?
				binding.To(toBind.ToType).ToName(toBind.Name).ToSingleton() :
				binding.ToName(toBind.Name).ToSingleton();

			if (toBind.IsCrossContext) //Bind this to the cross context injector
				binding.CrossContext();

			//binding.Weak();//SDM2014-0120: removed as part of cross-context implicit binding fix (moved up higher)
		}

		protected sealed class ImplicitBindingVO
		{
			public List<Type> BindTypes = new List<Type>();
			public Type ToType;
			public bool IsCrossContext;
			public object Name;

			public ImplicitBindingVO(Type bindType, Type toType, bool isCrossContext, object name)
			{
				BindTypes.Add(bindType);
				ToType = toType;
				IsCrossContext = isCrossContext;
				Name = name;
			}

			public ImplicitBindingVO(List<Type> bindTypes, Type toType, bool isCrossContext, object name)
			{
				BindTypes = bindTypes;
				ToType = toType;
				IsCrossContext = isCrossContext;
				Name = name;
			}
		}

		public abstract void ExecuteBindings(List<Type> typesInNamespaces);
	}
}