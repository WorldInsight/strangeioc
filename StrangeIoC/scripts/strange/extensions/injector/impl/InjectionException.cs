/*
 * Copyright 2013 ThirdMotion, Inc.
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

/**
 * @class strange.extensions.injector.impl.InjectionException
 * 
 * An exception thrown by the Injection system.
 */

using System;
using strange.extensions.injector.api;

namespace strange.extensions.injector.impl
{
	public class InjectionException : Exception
	{
		public InjectionExceptionType type{ get; set;}

		public InjectionException() : base()
		{
		}

		/// Constructs an InjectionException with a message and InjectionExceptionType
		public InjectionException(string message, InjectionExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}

		/// Constructs an InjectionException with a message and InjectionExceptionType and a cause
		public InjectionException(string message, InjectionExceptionType exceptionType, Exception cause)
			: base(message, cause)
		{
			type = exceptionType;
		}
	}
}

