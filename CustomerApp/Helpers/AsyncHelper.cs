using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace CustomerApp.Helpers
{
    public static class AsyncHelper
    {
        public static async Task MakeTaskBlocking(this Task task, ContentPage page)
        {
            var mtd = async () =>
            {
                await task;
                return 1;
            };
            await mtd().MakeTaskBlocking(page);
        }
        public static async Task<T> MakeTaskBlocking<T>(this Task<T> task, ContentPage page)
        {
            var original = ToggleBlockUIElements(page, true);
            try
            {
                return await task;
            }
            finally
            {
                Restore(original);
            }
        }
        public static Dictionary<VisualElement, bool> ToggleBlockUIElements(ContentPage page, bool isBlocking)
        {
            var original = new Dictionary<VisualElement, bool>();
            foreach (var element in GetAllVisualElements(page))
            {
                if (element is VisualElement visualElement)
                {
                    original[visualElement] = visualElement.IsEnabled;
                    visualElement.IsEnabled = !isBlocking;
                }
            }
            return original;
        }
        public static void Restore(Dictionary<VisualElement, bool> original)
        {
            foreach (var item in original)
            {
                item.Key.IsEnabled = item.Value;
            }
        }

        public static List<Microsoft.Maui.Controls.View> GetAllVisualElements(ContentPage currentPage)
        {
            var visualElements = new List<Microsoft.Maui.Controls.View>();

            if (currentPage.Content is Layout<Microsoft.Maui.Controls.View> layout)
            {
                AddChildren(layout, visualElements);
            }
            else if (currentPage.Content is not null)
            {
                visualElements.Add(currentPage.Content);
            }

            return visualElements;
        }

        private static void AddChildren(Layout<Microsoft.Maui.Controls.View> layout, List<Microsoft.Maui.Controls.View> buffer)
        {
            foreach (var child in layout.Children)
            {
                if (child is null)
                {
                    continue;
                }
                buffer.Add(child);

                // If the child is a layout, recurse to get its children
                if (child is Layout<Microsoft.Maui.Controls.View> childLayout)
                {
                    AddChildren(childLayout, buffer);
                }
            }
        }
    }
}
