﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System.Windows.Forms.Classes.Collections
{
   /// <summary>
   /// Base class for Ribbon items collections. Including Tab and Panel.
   /// Implements the IList interface because this is used by the visual studio designer to add and remove items.
   /// </summary>
   /// <typeparam name="T">typeof of ribbon item</typeparam>
   public abstract class RibbonCollectionBase<T> : List<T>, IList where T : IRibbonElement
   {
      private Ribbon _owner;

      /// <summary>
      /// Creates a new RibbonCollectionBase
      /// </summary>
      /// <param name="owner">the owner ribbon</param>
      protected RibbonCollectionBase(Ribbon owner)
      {
         _owner = owner;
      }

      /// <summary>
      /// Gets the Ribbon that owns this collection
      /// </summary>
      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public virtual Ribbon Owner
      {
         get { return _owner; }
      }

      /// <summary>
      /// Sets the value of the Owner Property
      /// </summary>
      internal virtual void SetOwner(Ribbon owner)
      {
         _owner = owner;
      }

      internal void SetOwner(IEnumerable<T> items)
      {
         if (items != null)
         {
            foreach (T item in items)
            {
               SetOwner(item);
            }
         }
      }

      internal abstract void SetOwner(T item);

      internal void ClearOwner(IEnumerable<T> items)
      {
         if (items != null)
         {
            foreach (T item in items)
            {
               ClearOwner(item);
            }
         }
      }

      internal abstract void ClearOwner(T item);

      internal abstract void UpdateRegions();

      #region virtual new List<T> overrides

      public virtual new T this[int index]
      {
         get { return base[index]; }
         set
         {
            SetOwner(value);
            base[index] = value;
            UpdateRegions();
         }
      }

      /// <summary>
      /// Adds the specified item to the collection
      /// </summary>
      /// <param name="item">Item to add to the collection</param>
      public virtual new void Add(T item)
      {
         SetOwner(item);
         base.Add(item);
         UpdateRegions();
      }

      /// <summary>
      /// Adds the specified items to the collection
      /// </summary>
      /// <param name="items">Items to add to the collection</param>
      public virtual new void AddRange(System.Collections.Generic.IEnumerable<T> items)
      {
         SetOwner(items);
         base.AddRange(items);
         UpdateRegions();
      }

      /// <summary>
      /// Inserts the specified item into the specified index
      /// </summary>
      /// <param name="index">Desired index of the item into the collection</param>
      /// <param name="item">item to be inserted</param>
      public virtual new void Insert(int index, T item)
      {
         SetOwner(item);
         base.Insert(index, item);
         UpdateRegions();
      }

      /// <summary>
      /// Removes the item from the collection.
      /// </summary>
      /// <param name="item">item to remove</param>
      public virtual new bool Remove(T item)
      {
         if (base.Remove(item))
         {
            ClearOwner(item);
            UpdateRegions();
            return true;
         }
         return false;
      }

      /// <summary>
      /// Removes all items matching the given predicate.
      /// </summary>
      /// <param name="predicate">defines the items to remove</param>
      /// <returns>number of elements removed</returns>
      public virtual new int RemoveAll(Predicate<T> predicate)
      {
         List<T> toRemove = this.FindAll(predicate);
         int ret = base.RemoveAll(predicate);
         if (toRemove.Count > 0)
         {
            ClearOwner(toRemove);
            UpdateRegions();
         }
         return ret;
      }

      /// <summary>
      /// Removes an item at the given index.
      /// </summary>
      /// <param name="index">index of item to remove</param>
      public virtual new void RemoveAt(int index)
      {
         T item = this[index];
         base.RemoveAt(index);
         ClearOwner(item);
         UpdateRegions();
      }

      /// <summary>
      /// Removes a given range from the collection
      /// </summary>
      /// <param name="index">start index</param>
      /// <param name="count">number of items to remove</param>
      public virtual new void RemoveRange(int index, int count)
      {
         List<T> toRemove = this.GetRange(index, count);
         base.RemoveRange(index, count);
         if (toRemove.Count > 0)
         {
            ClearOwner(toRemove);
            UpdateRegions();
         }
      }

      public virtual new void Clear()
      {
         List<T> allItems = new List<T>(this);
         base.Clear();
         if (allItems.Count > 0)
         {
            ClearOwner(allItems);
            UpdateRegions();
         }
      }

      #endregion virtual new List<T> overrides

      #region IList

      object IList.this[int index]
      {
         get { return this[index]; }
         set { this[index] = (T)value; }
      }

      int IList.Add(object item)
      {
         this.Add((T)item);
         return this.Count - 1;
      }

      void IList.Insert(int index, object item)
      {
         this.Insert(index, (T)item);
      }

      void IList.Remove(object value)
      {
         this.Remove((T)value);
      }

      void IList.RemoveAt(int index)
      {
         this.RemoveAt(index);
      }

      void IList.Clear()
      {
         this.Clear();
      }

      #endregion
   }
}
