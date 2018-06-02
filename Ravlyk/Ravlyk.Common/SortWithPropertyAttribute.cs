using System;

namespace Ravlyk.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SortWithPropertyAttribute : Attribute
	{
		public SortWithPropertyAttribute(string propertyName)
		{
			SortablePropertyName = propertyName;
		}

		public string SortablePropertyName { get; }
	}
}
