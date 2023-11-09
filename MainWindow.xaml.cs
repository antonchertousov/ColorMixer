using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ColorMixing.Annotations;
using ColorMixing.Data;
using ColorMixing.Helpers;
using WPF.ColorPicker;

namespace ColorMixing
{
    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Private fields

        /// <summary>
        /// Constant for the object dimensions
        /// </summary>
        private const double CircleRadius = 25;

        /// <summary>
        /// Collection of parent-child connections
        /// </summary>
        private List<Connection> connections = new List<Connection>();

        /// <summary>
        /// Current selected object
        /// </summary>
        private Ellipse currentObject;

        /// <summary>
        /// The string for current object statistics
        /// </summary>
        private string currentObjectStatistics = string.Empty;

        /// <summary>
        /// Parent identifier to connect entities
        /// Is not null if parent already selected
        /// </summary>
        private Guid? parentId;

        /// <summary>
        /// The point where to create the new object
        /// </summary>
        private Point? createObjectPoint;

        /// <summary>
        /// True if create menu item is enabled
        /// </summary>
        private bool isCreateEnabled;

        /// <summary>
        /// True if set color menu item is enabled
        /// </summary>
        private bool isColorEnabled;

        /// <summary>
        /// True if set parent menu item is enabled
        /// </summary>
        private bool isParentEnabled;

        /// <summary>
        /// True if set child menu item is enabled
        /// </summary>
        private bool isChildEnabled;
        
        #endregion

        #region Public properties

        /// <summary>
        /// Current selected object
        /// </summary>
        public Ellipse CurrentObject
        {
            get { return currentObject; }
            set
            {
                if (value != currentObject)
                {
                    currentObject = value;
                    OnPropertyChanged(nameof(CurrentObject));
                    if (currentObject == null)
                    {
                        CurrentObjectStatistics = string.Empty;
                        IsColorEnabled = false;
                        return;
                    }
                    var parents = GetParentsCount(currentObject.Tag);
                    var children = GetChildrenCount(currentObject.Tag);
                    CurrentObjectStatistics = $"Parents: {parents}, Children: {children}";
                    IsColorEnabled = parents == 0;
                }
            }
        }

        /// <summary>
        /// True if create menu item is enabled
        /// </summary>
        public bool IsCreateEnabled
        {
            get { return isCreateEnabled; }
            set
            {
                if (value != isCreateEnabled)
                {
                    isCreateEnabled = value;
                    OnPropertyChanged(nameof(IsCreateEnabled));
                }
            }
        }

        /// <summary>
        /// Enable the 'set color' menu item in case of no parents
        /// for the current object
        /// </summary>
        public bool IsColorEnabled
        {
            get { return isColorEnabled; }
            set
            {
                if (value != isColorEnabled)
                {
                    isColorEnabled = value;
                    OnPropertyChanged(nameof(IsColorEnabled));
                }
            }
        }

        /// <summary>
        /// True if set parent menu item is enabled
        /// </summary>
        public bool IsParentEnabled
        {
            get { return isParentEnabled; }
            set
            {
                if (value != isParentEnabled)
                {
                    isParentEnabled = value;
                    OnPropertyChanged(nameof(IsParentEnabled));
                }
            }
        }

        /// <summary>
        /// True if set child menu item is enabled
        /// </summary>
        public bool IsChildEnabled
        {
            get { return isChildEnabled; }
            set
            {
                if (value != isChildEnabled)
                {
                    isChildEnabled = value;
                    OnPropertyChanged(nameof(IsChildEnabled));
                }
            }
        }

        /// <summary>
        /// Statistics for the current object
        /// </summary>
        public string CurrentObjectStatistics
        {
            get { return currentObjectStatistics; }
            set
            {
                if (value != currentObjectStatistics)
                {
                    currentObjectStatistics = value;
                    OnPropertyChanged(nameof(CurrentObjectStatistics));
                }
            }
        }

        #endregion

        #region Mouse and button click actions

        /// <summary>
        /// Select object by left mouse click 
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = (Canvas) sender;
            var pos = e.GetPosition(canvas);
            canvas.CaptureMouse();

            CurrentObject = e.OriginalSource as Ellipse;
        }

        /// <summary>
        /// Release mouse handler
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = (Canvas) sender;
            canvas.ReleaseMouseCapture();
            CurrentObject = null;
        }

        /// <summary>
        /// Handler for move the entities with connections
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentObject != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // Move object
                var canvas = (Canvas) sender;
                var pos = e.GetPosition(canvas);
                Canvas.SetLeft(CurrentObject, pos.X);
                Canvas.SetTop(CurrentObject, pos.Y);

                // Move connection lines
                var connections = this.connections.Where(x =>
                    x.ChildEntityId == (Guid) CurrentObject.Tag || x.ParentEntityId == (Guid) currentObject.Tag);

                foreach (var conn in connections)
                {
                    MoveLine(conn, canvas, pos);
                }
            }
        }

        /// <summary>
        /// Clear all created objects and connections data
        /// </summary>
        private void OnResetAllClick(object sender, RoutedEventArgs e)
        {
            canvas_Object.Children.Clear();
            CurrentObject = null;
            connections.Clear();
            parentId = null;
        }

        /// <summary>
        /// Handler to show the context menu for the object
        /// </summary>
        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cm = ContextMenuService.GetContextMenu(sender as DependencyObject);
            if (cm == null)
            {
                return;
            }

            var elements = ((Canvas) sender).Children.OfType<UIElement>()
                .Where(x => x.IsMouseOver);

            CurrentObject = elements.Select(x => x as Ellipse).FirstOrDefault();

            createObjectPoint = e.GetPosition(sender as Canvas);

            UpdateContextMenuItemsIsEnabled();

            cm.PlacementTarget = sender as UIElement;
            cm.IsOpen = true;
        }
        #endregion
        
        #region Context menu handling methods

        /// <summary>
        /// Handler for 'Create' menu item click
        /// Creates the new object
        /// </summary>
        private void CreateMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && CurrentObject == null && createObjectPoint != null)
            {
                CreateNewObject(canvas_Object, createObjectPoint.Value);
                createObjectPoint = null;
            }
        }

        /// <summary>
        /// Handler to set the object color
        /// </summary>
        private void SetColorMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && CurrentObject != null && ColorPickerWindow.ShowDialog(out Color itemColor))
            {
                CurrentObject.Fill = new SolidColorBrush(itemColor);
                OnPropertyChanged(nameof(CurrentObject));
                // Update the tree on set parent color
                var canvas = CurrentObject.Parent as Canvas;
                UpdateChildColorsOnParentChange(canvas, (Guid) CurrentObject.Tag);
                CurrentObject = null;
            }
        }

        /// <summary>
        /// Handler to set the parent object
        /// </summary>
        private void SetParentMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null && CurrentObject != null && parentId == null)
            {
                parentId = (Guid) CurrentObject.Tag;
                CurrentObject = null;
            }
        }

        /// <summary>
        /// Handler for set the child object
        /// </summary>
        private void SetChildMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item == null || CurrentObject == null || parentId == null)
            {
                return;
            }

            // Check if no such parent-child relation present and vise versa to avoid circular reference
            if (connections.Any(x => x.ParentEntityId == (Guid) CurrentObject.Tag && x.ChildEntityId == parentId)
                || connections.Any(x => x.ParentEntityId == parentId && x.ChildEntityId == (Guid) CurrentObject.Tag))
            {
                parentId = null;
                return;
            }

            var canvas = CurrentObject.Parent as Canvas;
            var parent = canvas.Children.OfType<Ellipse>().FirstOrDefault(x => (Guid) x.Tag == parentId);

            // Create line between parent and child
            if (parent != null)
            {
                var startPoint = new Point(Canvas.GetLeft(parent), Canvas.GetTop(parent));
                var endPoint = new Point(Canvas.GetLeft(CurrentObject), Canvas.GetTop(CurrentObject));
                var lineStart = LineHelper.CalculateIntersection(startPoint,
                    CircleRadius, endPoint);
                var lineEnd = LineHelper.CalculateIntersection(endPoint,
                    CircleRadius, startPoint);
                if (!lineStart.HasValue || !lineEnd.HasValue)
                {
                    return;
                }

                var lineId = Guid.NewGuid();
                DrawTheLine(canvas, lineId, lineStart.Value, lineEnd.Value);
                connections.Add(new Connection()
                {
                    LineId = lineId,
                    ParentEntityId = (Guid)parent.Tag,
                    ChildEntityId = (Guid) CurrentObject.Tag
                });

                // Update colors of children
                UpdateChildColorsOnParentChange(canvas, (Guid)parent.Tag);
                // Reset parent
                parentId = null;
            }

            CurrentObject = null;
        }

        /// <summary>
        /// Enable or disable context menu items
        /// </summary>
        private void UpdateContextMenuItemsIsEnabled()
        {
            // Create is available if no existing object selected 
            if (CurrentObject == null)
            {
                IsCreateEnabled = true;
                IsParentEnabled = false;
                IsChildEnabled = false;
                return;
            }

            // Create disabled for the existing object
            IsCreateEnabled = false;

            // Set parent item is enabled
            IsParentEnabled = parentId == null;

            // Set child item is enabled
            // Also check it's not the same object as parent
            IsChildEnabled = parentId != null && parentId.Value != (Guid) CurrentObject.Tag;
        }

        #endregion

        #region Creating, moving objects and drawing connections

        /// <summary>
        /// Create new object and add to canvas
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="point">Point of creation</param>
        private void CreateNewObject(Canvas canvas, Point point)
        {
            CurrentObject = new Ellipse
            {
                Width = 2 * CircleRadius,
                Height = 2 * CircleRadius,
                Margin = new Thickness(-CircleRadius, -CircleRadius, 0, 0),
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Tag = Guid.NewGuid()
            };

            canvas.Children.Add(CurrentObject);
            Canvas.SetLeft(CurrentObject, point.X);
            Canvas.SetTop(CurrentObject, point.Y);
        }

        /// <summary>
        /// Draw the connection line and add to the canvas collection
        /// </summary>
        private void DrawTheLine(Canvas canvas, Guid lineId, Point startPoint, Point endPoint)
        {
            ArrowLine line = new ArrowLine
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Tag = lineId
            };

            canvas.Children.Add(line);
        }

        /// <summary>
        /// Move the connection line according to new position 
        /// </summary>
        /// <param name="conn">Connection object</param>
        /// <param name="canvas">Canvas</param>
        /// <param name="pos">New position</param>
        private void MoveLine(Connection conn, Canvas canvas, Point pos)
        {
            var line = canvas.Children.OfType<ArrowLine>()
                .FirstOrDefault(x => (Guid) x.Tag == conn.LineId);

            Point? lineStart = null;
            Point? lineEnd = null;

            if (line == null)
            {
                return;
            }

            if (conn.ChildEntityId == (Guid) CurrentObject.Tag)
            {
                var parent = canvas.Children.OfType<Ellipse>()
                    .FirstOrDefault(x => (Guid) x.Tag == conn.ParentEntityId);
                var result = RecalculateLine(canvas, pos, parent, true);
                lineStart = result.start;
                lineEnd = result.end;
            }

            if (conn.ParentEntityId == (Guid) CurrentObject.Tag)
            {
                var child = canvas.Children.OfType<Ellipse>()
                    .FirstOrDefault(x => (Guid) x.Tag == conn.ChildEntityId);
                var result = RecalculateLine(canvas, pos, child, false);
                lineStart = result.start;
                lineEnd = result.end;
            }

            if (lineStart.HasValue && lineEnd.HasValue)
            {
                line.X1 = lineStart.Value.X;
                line.Y1 = lineStart.Value.Y;
                line.X2 = lineEnd.Value.X;
                line.Y2 = lineEnd.Value.Y;
            }
        }

        /// <summary>
        /// Recalculate line start and end positions
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="pos">New position</param>
        /// <param name="entity">Moving entity</param>
        /// <param name="isParent">True if is parent, false otherwise</param>
        private (Point? start, Point? end) RecalculateLine(Canvas canvas, Point pos, Ellipse entity, bool isParent)
        {
            var point = new Point(Canvas.GetLeft(entity), Canvas.GetTop(entity));
            var lineStart = LineHelper.CalculateIntersection(point, CircleRadius, pos);
            var lineEnd = LineHelper.CalculateIntersection(pos, CircleRadius, point);
            return isParent
                ? (lineStart, lineEnd)
                : (lineEnd, lineStart);
        }

        #endregion

        #region Colors mixing

        /// <summary>
        /// Updates all the children colors (mixing based on their parents) for provided parent
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="objectId">Parent object identifier</param>
        private void UpdateChildColorsOnParentChange(Canvas canvas, Guid objectId)
        {
            // Find all 1st level children for this parent
            var childrenIds = connections.Where(x => x.ParentEntityId == objectId)
                .Select(x => x.ChildEntityId);

            // Find parents for each child
            foreach (var childId in childrenIds)
            {
                var parentIds = connections.Where(x => x.ChildEntityId == childId)
                    .Select(x => x.ParentEntityId);
                // Recalculate child color
                UpdateObjectColor(canvas, childId, parentIds);

                // Find children for the current child
                UpdateChildColorsOnParentChange(canvas, childId);
            }
        }

        /// <summary>
        /// Update object color based on parents colors
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="objectId">Object identifier to update</param>
        /// <param name="parentIds">Parent object identifiers</param>
        private void UpdateObjectColor(Canvas canvas, Guid objectId, IEnumerable<Guid> parentIds)
        {
            var brushColors = canvas.Children.OfType<Ellipse>()
                .Where(x => parentIds.Contains((Guid) x.Tag))
                .Select(x => x.Fill);
            var colors = brushColors.Select(x => ((SolidColorBrush) x).Color);
            // Calculate mixed color
            var mixedColor = ColorMixer.Mix(colors);
            
            // Assign mixed color to the object
            var objectToUpdate = canvas.Children.OfType<Ellipse>().FirstOrDefault(x => (Guid) x.Tag == objectId);
            if (objectToUpdate != null)
            {
                objectToUpdate.Fill = new SolidColorBrush(mixedColor);
            }
        }
        #endregion

        #region Statistics

        /// <summary>
        /// Gets the number of parents for selected object
        /// </summary>
        /// <param name="id">object id</param>
        private int GetParentsCount(object id)
        {
            return id is Guid
                ? connections.Count(x => x.ChildEntityId == (Guid) id)
                : 0;
        }

        /// <summary>
        /// Gets the number of children for selected object
        /// </summary>
        /// <param name="id">object id</param>
        private int GetChildrenCount(object id)
        {
            return id is Guid
                ? connections.Count(x => x.ParentEntityId == (Guid) id)
                : 0;
        }
        #endregion

        #region Public methods

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Property changed support

        /// <summary>
        /// Property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changed notification method
        /// </summary>
        /// <param name="propertyName">the name of changed property</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}