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
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;

namespace strange.extensions.implicitBind.impl
{
	[ConcreteBinder(20, Scope = InjectionBindingScope.CROSS_CONTEXT)]
	public class ConcreteMediatedByBinder : AbstractConcreteBinder<MediatedBy>
	{
		[Inject]
		public IMediationBinder mediationBinder { get; set; }

		public override void ExecuteBindings(List<Type> typesInNamespaces)
		{
			this.typesInNamespaces = typesInNamespaces;

			Type mediatorType = null;
			Type viewType = null;
			foreach (Type type in GetAttributedTypes())
			{
				viewType = type;
				mediatorType = ((MediatedBy)type.GetCustomAttributes(typeof(MediatedBy), true).First()).MediatorType;

				if (mediatorType == null)
					throw new MediationException("Cannot implicitly bind view of type: " + type.Name + " due to null MediatorType",
						MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);

				// DUP
				if (mediationBinder != null && viewType != null && mediatorType != null) //Bind this mediator!
					mediationBinder.Bind(viewType).To(mediatorType);

			}
		}
	}
}
