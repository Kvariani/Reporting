﻿using System;
using DevExpress.ExpressApp;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Actions;

namespace Common.Win.General.DashBoard.Controllers
{
    public partial class WinDashboardNavigationController : DashboardNavigationController
    {
        NavBarNavigationControl _navBarNavigationControl;

        public WinDashboardNavigationController()
        {
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Window.TemplateChanged += window_TemplateChanged;
        }

        protected override void OnDeactivated()
        {
            Window.TemplateChanged -= window_TemplateChanged;
            if (_navBarNavigationControl != null)
            {
                _navBarNavigationControl.Items.CollectionChanged -= Items_CollectionChanged;
                _navBarNavigationControl = null;
            }
            base.OnDeactivated();
        }

        private void window_TemplateChanged(object sender, EventArgs e)
        {
            if (Window.Template != null)
                foreach (NavigationActionContainer actionContainer in Window.Template.GetContainers().OfType<NavigationActionContainer>())
                {
                    _navBarNavigationControl = actionContainer.NavigationControl as NavBarNavigationControl;
                    if (_navBarNavigationControl != null)
                    {
                        _navBarNavigationControl.Items.CollectionChanged += Items_CollectionChanged;
                        UpdateNavigationImages();
                    }
                }
        }

        void Items_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            UpdateNavigationImages();
        }

        public override void UpdateNavigationImages()
        {
            var dashboardActions = _navBarNavigationControl.ActionItemToItemLinkMap.Keys.Intersect(DashboardActions.Keys);
            foreach (var action in dashboardActions)
                UpdateActionIcon(action);
        }

        private void UpdateActionIcon(ChoiceActionItem action)
        {
            var icon = DashboardActions[action].Icon;
            if (icon != null)
            {
                var item = _navBarNavigationControl?.ActionItemToItemLinkMap[action]?.Item;
                if (item != null)
                {
                    int width = 32;

                    item.LargeImage = icon;
                    var smallImage = resizeImage(icon, width);
                    item.SmallImage = smallImage;
                }
            }
        }

        public static Image resizeImage(Image imgToResize, int maxWidth)
        {
            if (imgToResize.Width > maxWidth)
            {
                var scale = Convert.ToDecimal(imgToResize.Width) / maxWidth;
                var size = new Size(maxWidth, Convert.ToInt32(Convert.ToDecimal(imgToResize.Height) / scale));
                return (new Bitmap(imgToResize, size));
            }
            return imgToResize;
        }
    }
}
