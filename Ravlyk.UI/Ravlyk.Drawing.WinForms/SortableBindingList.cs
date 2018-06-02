using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Ravlyk.Common;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Binding list with support for sorting
	/// </summary>
	/// <typeparam name="T">Type of element.</typeparam>
	public class SortableBindingList<T> : BindingList<T>
	{
		/// <summary>
		/// Instantiates SortableBindingList object.
		/// </summary>
		/// <param name="list">List of element to include in this SortableBindingList.</param>
		public SortableBindingList(IList<T> list) : base(list) { }

		/// <summary>Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null. </summary>
		/// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptor" /> used for sorting the list.</returns>
		protected override PropertyDescriptor SortPropertyCore => sortPropertyValue;
		PropertyDescriptor sortPropertyValue;

		/// <summary>Gets the direction the list is sorted.</summary>
		/// <returns>One of the <see cref="T:System.ComponentModel.ListSortDirection" /> values. The default is <see cref="F:System.ComponentModel.ListSortDirection.Ascending" />. </returns>
		protected override ListSortDirection SortDirectionCore => sortDirectionValue;
		ListSortDirection sortDirectionValue;

		/// <summary>Gets a value indicating whether the list supports sorting.</summary>
		/// <returns>true if the list supports sorting; otherwise, false. The default is false.</returns>
		protected override bool SupportsSortingCore => true;

		/// <summary>Gets a value indicating whether the list is sorted. </summary>
		/// <returns>true if the list is sorted; otherwise, false. The default is false.</returns>
		protected override bool IsSortedCore => isSortedValue;
		bool isSortedValue;

		/// <summary>Sorts the items if overridden in a derived class; otherwise, throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <param name="property">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that specifies the property to sort on.</param>
		/// <param name="direction">One of the <see cref="T:System.ComponentModel.ListSortDirection" />  values.</param>
		/// <exception cref="T:System.NotSupportedException">Method is not overridden in a derived class. </exception>
		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{
			var sortPropertyAttribute = property.Attributes.OfType<SortWithPropertyAttribute>().FirstOrDefault();
			var sortProperty = property.ComponentType.GetProperty(sortPropertyAttribute != null ? sortPropertyAttribute.SortablePropertyName : property.Name);

			var interfaceType = sortProperty.PropertyType.GetInterface(nameof(IComparable));
			if (interfaceType == null && sortProperty.PropertyType.IsValueType)
			{
				interfaceType = Nullable.GetUnderlyingType(sortProperty.PropertyType)?.GetInterface(nameof(IComparable));
			}

			if (interfaceType != null)
			{
				sortPropertyValue = property;
				sortDirectionValue = direction;

				var query = direction == ListSortDirection.Ascending
					? Items.OrderBy(i => sortProperty.GetValue(i))
					: Items.OrderByDescending(i => sortProperty.GetValue(i));

				var newIndex = 0;
				foreach (object item in query.ToList())
				{
					Items[newIndex] = (T)item;
					newIndex++;
				}

				isSortedValue = true;
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
			else
			{
				throw new NotSupportedException($"Cannot sort by {sortProperty.Name}: {sortProperty.PropertyType} does not implement IComparable");
			}
		}
	}
}
