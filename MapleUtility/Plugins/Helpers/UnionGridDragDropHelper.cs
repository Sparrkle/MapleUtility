using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;

namespace MapleUtility.Plugins.Helpers
{
    public class UnionGridDragDropHelper
    {
		private UniformGrid _associatedObject;
		/// <summary>
		/// AssociatedObject Property
		/// </summary>
		public UniformGrid AssociatedObject
		{
			get
			{
				return _associatedObject;
			}
			set
			{
				_associatedObject = value;
			}
		}

		private static Dictionary<UniformGrid, UnionGridDragDropHelper> instances;

		static UnionGridDragDropHelper()
		{
			instances = new Dictionary<UniformGrid, UnionGridDragDropHelper>();
		}

		public static bool GetIsEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value)
		{
			UnionGridDragDropHelper behavior = GetAttachedBehavior(obj as UniformGrid);

			behavior.AssociatedObject = obj as UniformGrid;

			if (value)
			{
				behavior.Initialize();
			}
			else
			{
				behavior.CleanUp();
			}
			obj.SetValue(IsEnabledProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(GridViewDragDropBehavior),
				new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

		public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			SetIsEnabled(dependencyObject, (bool)e.NewValue);
		}

		private static UnionGridDragDropHelper GetAttachedBehavior(UniformGrid gridview)
		{
			if (!instances.ContainsKey(gridview))
			{
				instances[gridview] = new UnionGridDragDropHelper();
				instances[gridview].AssociatedObject = gridview;
			}

			return instances[gridview];
		}

		protected virtual void Initialize()
		{
			this.UnsubscribeFromDragDropEvents();
			this.SubscribeToDragDropEvents();
		}

		protected virtual void CleanUp()
		{
			this.UnsubscribeFromDragDropEvents();
		}

		private void SubscribeToDragDropEvents()
		{
			DragDropManager.AddDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
			DragDropManager.AddGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
			DragDropManager.AddDropHandler(this.AssociatedObject, OnDrop);
			DragDropManager.AddDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
			DragDropManager.AddDragOverHandler(this.AssociatedObject, OnDragOver);
		}

		private void UnsubscribeFromDragDropEvents()
		{
			DragDropManager.RemoveDragInitializeHandler(this.AssociatedObject, OnDragInitialize);
			DragDropManager.RemoveGiveFeedbackHandler(this.AssociatedObject, OnGiveFeedback);
			DragDropManager.RemoveDropHandler(this.AssociatedObject, OnDrop);
			DragDropManager.RemoveDragDropCompletedHandler(this.AssociatedObject, OnDragDropCompleted);
			DragDropManager.RemoveDragOverHandler(this.AssociatedObject, OnDragOver);

		}

		private void OnDragInitialize(object sender, DragInitializeEventArgs e)
		{
		}

		private void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
		{
			e.SetCursor(Cursors.Arrow);
			e.Handled = true;
		}

		private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
		{
		}

		private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
		{

		}

		private void OnDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
		{

		}
	}
}
