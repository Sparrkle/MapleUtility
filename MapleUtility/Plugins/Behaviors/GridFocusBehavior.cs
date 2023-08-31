using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using System.Windows;

namespace MapleUtility.Plugins.Behaviors
{
    public class GridFocusBehavior
    {
        private RadGridView grid = null;

        public GridFocusBehavior(RadGridView grid)
        {
            this.grid = grid;
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(GridFocusBehavior), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        public static void SetIsEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(IsEnabledProperty, enabled);
        }

        public static bool GetIsEnabled(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsEnabledProperty);
        }

        private static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RadGridView grid = dependencyObject as RadGridView;
            if (grid != null)
            {
                if ((bool)e.NewValue)
                {
                    GridFocusBehavior behavior = new GridFocusBehavior(grid);
                    behavior.Attach();
                }
            }
        }

        private void Attach()
        {
            if (grid != null)
            {
                grid.PreparingCellForEdit += grid_PreparingCellForEdit;
            }
        }

        void grid_PreparingCellForEdit(object sender, GridViewPreparingCellForEditEventArgs e)
        {
            var editor = e.EditingElement;
            editor.LostFocus += new RoutedEventHandler(editor_LostFocus);
        }

        void editor_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).LostFocus -= new RoutedEventHandler(editor_LostFocus);
            var parentDataControl = (sender as FrameworkElement).ParentOfType<GridViewDataControl>();
            var parentCell = (sender as FrameworkElement).ParentOfType<GridViewCell>();

            if (parentDataControl == null || parentCell == null)
                return;

            UIElement focusedElement = FocusManagerHelper.GetFocusedElement((DependencyObject)parentDataControl) as UIElement;

            CommitEditForRowsOutOfFocus(focusedElement, parentCell);
        }

        private void CommitEditForRowsOutOfFocus(UIElement focusedElement, GridViewCell parentCell)
        {
            if (focusedElement == null)
                return;
            List<GridViewRow> list = focusedElement.GetParents().OfType<GridViewRow>().ToList<GridViewRow>();
            foreach (GridViewRow gridViewRow in parentCell.GetParents().OfType<GridViewRow>().ToList<GridViewRow>())
            {
                if (!list.Contains(gridViewRow))
                    gridViewRow.CommitEdit();
            }
        }
    }
}
